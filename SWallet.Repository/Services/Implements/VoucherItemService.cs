using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Voucher;
using SWallet.Repository.Payload.Response.Voucher;
using SWallet.Repository.Services.Interfaces;
using SWallet.Repository.Utils;

namespace SWallet.Repository.Services.Implements
{
    public class VoucherItemService : BaseService<VoucherItemService>, IVoucherItemService
    {
        public VoucherItemService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<VoucherItemService> logger) : base(unitOfWork, logger)
        {
        }

        public async Task<bool> GenerateVoucherItemsAsync(VoucherItemRequest voucherItemRequest)
        {
            var campaign = await _unitOfWork.GetRepository<CampaignDetail>().AnyAsync(x => x.CampaignId.Equals(voucherItemRequest.CampaignDetailId));
            if (campaign)
                throw new ApiException("Campaign not found", 400);

            var voucherItems = new List<VoucherItem>();
            for (int i = 0; i < voucherItemRequest.Quantity; i++)
            {
                voucherItems.Add(new VoucherItem
                {
                    Id = Ulid.NewUlid().ToString(),
                    VoucherId = voucherItemRequest.VoucherId,
                    CampaignDetailId = voucherItemRequest.CampaignDetailId,
                    VoucherCode = "code",
                    IsBought = false,
                    IsLocked = false,
                    IsUsed = false,
                    DateCreated = DateTime.Now,
                    DateIssued = DateTime.Now,
                    ValidOn = voucherItemRequest.ValidOn,
                    ExpireOn = voucherItemRequest.ExpireOn,
                    State = true,
                    Status = true,
                });
            }

            await _unitOfWork.GetRepository<VoucherItem>().InsertRangeAsync(voucherItems);
            var result = await _unitOfWork.CommitAsync() > 0;
            return result;
        }

        public Task<VoucherItem> GetVoucherItemByIdAsync(string voucherId)
        {
            var result = _unitOfWork.GetRepository<VoucherItem>().SingleOrDefaultAsync(predicate: x => x.VoucherId.Equals(voucherId));
            return result;
        }

        public async Task<IEnumerable<VoucherItemResponse>> GetVoucherItemsByCampaignDetailIdAsync(IEnumerable<string> campaignDetailIds)
        {
            var result = await _unitOfWork.GetRepository<VoucherItem>().GetListAsync(
                 selector: x => new VoucherItemResponse
                 {
                     Id = x.Id,
                     VoucherId = x.VoucherId,
                     CampaignDetailId = x.CampaignDetailId,
                     VoucherCode = x.VoucherCode,
                     Index = x.Index,
                     IsLocked = x.IsLocked,
                     IsBought = x.IsBought,
                     IsUsed = x.IsUsed,
                     ValidOn = x.ValidOn,
                     ExpireOn = x.ExpireOn,
                     DateCreated = x.DateCreated,
                     DateIssued = x.DateIssued,
                     Status = x.Status,
                 },
                  predicate: x => campaignDetailIds.Contains(x.CampaignDetailId));
            return result;
        }

        public async Task<List<VoucherItem>> RedeemVoucherAsync(string voucherId, string campaignId, int quantity)
        {
            if (quantity <= 0)
            {
                throw new ApiException("Quantity must be greater than 0", 400);
            }

            var availableVouchers = await _unitOfWork.GetRepository<VoucherItem>()
                .GetListWithTakeAsync(
                    predicate: x => x.CampaignDetail.CampaignId.Equals(campaignId)
                        && x.VoucherId.Equals(voucherId)
                        && x.IsBought == false
                        && (x.IsLocked == false || x.IsLocked == null)
                        && (x.ExpireOn == null || x.ExpireOn >= DateOnly.FromDateTime(TimeUtils.GetVietnamToday())),
                    take: quantity
                );

            if (availableVouchers == null || availableVouchers.Count < quantity)
            {
                throw new ApiException($"Not enough available vouchers. Requested: {quantity}, Available: {availableVouchers?.Count ?? 0}", 400, "REDEEM_VOUCHER_FAIL");
            }

            foreach (var voucherItem in availableVouchers)
            {
                voucherItem.IsBought = true;
                voucherItem.DateIssued = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeUtils.GetVietnamTimeZone());
                _unitOfWork.GetRepository<VoucherItem>().UpdateAsync(voucherItem);
            }

            var result = await _unitOfWork.CommitAsync() > 0;
            if (!result)
            {
                throw new ApiException("Redeem vouchers failed", 500);
            }

            return availableVouchers.ToList(); // Trả về danh sách voucher đã redeem
        }


        public async Task<string> GetVoucherItemIdAvailable(string voucherId, string studentId)
        {
            var voucherItemId = await _unitOfWork.GetRepository<Activity>()
                    .SingleOrDefaultAsync(
                        selector: x => x.VoucherItem.Id,
                        predicate: x => x.VoucherItem.Voucher.Id == voucherId
                            && x.VoucherItem.IsBought == true
                            && x.VoucherItem.IsUsed == false
                            && x.StudentId == studentId,
                        include: q => q.Include(x => x.VoucherItem).ThenInclude(v => v.Voucher)

                    );
            return voucherItemId ?? throw new ApiException("Voucher item not found", 404);
        }
    }
}
