using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Enums;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Activity;
using SWallet.Repository.Payload.Response.Activity;
using SWallet.Repository.Payload.Response.ActivityTransaction;
using SWallet.Repository.Services.Interfaces;
using SWallet.Repository.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Implements
{
    public class ActivityService : BaseService<ActivityService>, IActivityService
    {
        private readonly IWalletService _walletService;
        private readonly IVoucherItemService _voucherItemService;
        public ActivityService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<ActivityService> logger, IWalletService walletService, IVoucherItemService voucherItemService) : base(unitOfWork, logger)
        {
            _walletService = walletService;
            _voucherItemService = voucherItemService;
        }

        public async Task<bool> RedeemVoucherActivityAsync(ActivityRequest activityRequest)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Kiểm tra wallet
                var wallet = await _walletService.GetWalletByStudentId(activityRequest.StudentId, (int)WalletType.Green);
                if (wallet == null || wallet.Balance < activityRequest.Cost)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return false;
                }

                // Redeem voucher và lấy danh sách voucher đã redeem
                var redeemedVouchers = await _voucherItemService.RedeemVoucherAsync(activityRequest.CampaignId, activityRequest.Quantity);
                if (redeemedVouchers == null || redeemedVouchers.Count < activityRequest.Quantity)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return false;
                }

                // Tạo activity cho từng voucher
                var activities = redeemedVouchers.Select(voucher => new Activity
                {
                    Id = Ulid.NewUlid().ToString(),
                    StoreId = null,
                    StudentId = activityRequest.StudentId,
                    VoucherItemId = voucher.Id,
                    Type = (int?)ActivityType.Buy,
                    Description = $"Buy Voucher {voucher.VoucherCode}",
                    State = true,
                    Status = true,
                    DateCreated = DateTime.UtcNow
                }).ToList();

                // Thêm tất cả activities
                await _unitOfWork.GetRepository<Activity>().InsertRangeAsync(activities);
                await _unitOfWork.CommitAsync();

                // Trừ điểm từ wallet
                await _walletService.UpdateWalletForRedeem(wallet.Id, -(decimal)activityRequest.Cost);

                // Thêm activity transaction
                await AddActivityTransactionAsync(
                    activities.First().Id,
                    wallet.Id,
                    -(decimal)activityRequest.Cost,
                    $"Redeem {activityRequest.Quantity} vouchers"
                );

                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
        public async Task<ActivityTransaction> AddActivityTransactionAsync(string activityId, string walletId, decimal amount, string description)
        {
            var transaction = new ActivityTransaction
            {
                Id = Ulid.NewUlid().ToString(),
                ActivityId = activityId,
                WalletId = walletId,
                Amount = amount,
                Description = description,
                State = true
            };

            await _unitOfWork.GetRepository<ActivityTransaction>().InsertAsync(transaction);
            var result = await _unitOfWork.CommitAsync() > 0;
            if (!result)
            {
                throw new ApiException("Add activity transaction failed");
            }
            return transaction;
        }

        public async Task<IPaginate<ActivityTransactionResponse>> GetAllActivityTransactionAsync(
                     string studentId,
                     string search,
                     int page,
                     int size)
        {
            if (page < 1)
                throw new ApiException("Page number must be greater than 0", 400, "INVALID_PAGE");
            if (size < 1)
                throw new ApiException("Page size must be greater than 0", 400, "INVALID_SIZE");
            if (size > 100) // Giới hạn kích thước tối đa
                size = 100;
            if (string.IsNullOrEmpty(studentId))
                throw new ApiException("Student ID is required", 400, "WALLET_ID_REQUIRED");

            var walletId = await _unitOfWork.GetRepository<Wallet>().SingleOrDefaultAsync(
                selector: x => x.Id, 
                predicate: x => x.StudentId == studentId && x.Type == (int)WalletType.Green);

            Expression<Func<ActivityTransaction, bool>> filter = x =>
                x.WalletId == walletId
                && (string.IsNullOrEmpty(search) || (x.Activity != null && x.Activity.VoucherItem != null && x.Activity.VoucherItem.Voucher.VoucherName.Contains(search)));

            var transactions = await _unitOfWork.GetRepository<ActivityTransaction>()
                .GetPagingListAsync(
                    selector: x => new ActivityTransactionResponse
                    {
                        Id = x.Id,
                        ActivityId = x.ActivityId,
                        WalletId = x.WalletId,
                        Amount = x.Amount,
                        Description = x.Description,
                        Status = x.Status ?? false,
                        VoucherName = x.Activity.VoucherItem.Voucher.VoucherName,
                        CreatedAt = x.Activity.DateCreated
                    },
                    predicate: filter,
                    include: q => q.Include(x => x.Activity).ThenInclude(a => a.VoucherItem).ThenInclude(v => v.Voucher),
                    orderBy: q => q.OrderByDescending(x => x.Activity.DateCreated),
                    page: page,
                    size: size
                );

            return transactions;
        }

        public Task<bool> DeleteActivityAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IPaginate<ActivityResponse>> GetActivityAsync(string search, bool? isAsc, int page, int size)
        {
            Expression<Func<Activity, bool>> filter = x => x.Status == true;

            if (!string.IsNullOrEmpty(search))
            {
                filter = filter.AndAlso(x => x.Description.Contains(search));
            }

            var activities = await _unitOfWork.GetRepository<Activity>()
                .GetPagingListAsync(
                    selector: x => new ActivityResponse
                    {
                        Id = x.Id,
                        StoreId = x.StoreId,
                        StudentId = x.StudentId,
                        VoucherItemId = x.VoucherItemId,
                        Type = x.Type.ToString(),
                        Description = x.Description,
                        State = x.State ?? false,
                        CreatedAt = x.DateCreated ?? DateTime.MinValue,
                        UpdatedAt = x.DateUpdated ?? DateTime.MinValue
                    },
                    predicate: filter,
                    orderBy: isAsc.HasValue && isAsc.Value
                        ? x => x.OrderBy(a => a.DateCreated)
                        : x => x.OrderByDescending(a => a.DateCreated),
                    page: page,
                    size: size
                );

            return activities;
        }

        public async Task<bool> UpdateActivityAsync(string id, ActivityRequest activityRequest)
        {
            if (activityRequest == null)
            {
                throw new ArgumentNullException(nameof(activityRequest));
            }

            var activity = await _unitOfWork.GetRepository<Activity>().SingleOrDefaultAsync(predicate: x => x.Id == id);

            if (activity == null)
            {
                return false;
            }

            //activity.Description = activityRequest.Description;
            activity.DateUpdated = DateTime.Now;
            //activity.ActivityTransactions = activityRequest.ActivityTransactions;

            _unitOfWork.GetRepository<Activity>().UpdateAsync(activity);
            var result = await _unitOfWork.CommitAsync();

            return result > 0;
        }
    }
}
