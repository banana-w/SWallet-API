using CloudinaryDotNet;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Enums;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Activity;
using SWallet.Repository.Payload.Request.CampTransaction;
using SWallet.Repository.Payload.Request.Voucher;
using SWallet.Repository.Payload.Response.Activity;
using SWallet.Repository.Payload.Response.ActivityTransaction;
using SWallet.Repository.Payload.Response.Voucher;
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
        private const decimal PointBackRate = 0.9M;
        private readonly IWalletService _walletService;
        private readonly IVoucherItemService _voucherItemService;
        private readonly ICampaignTransactionService _campaignTransactionService;
        private readonly IChallengeService _challengeService;
        private readonly IFirebaseService _firebaseService;
        public ActivityService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<ActivityService> logger, IWalletService walletService, 
            IVoucherItemService voucherItemService, IFirebaseService firebaseService, 
            ICampaignTransactionService campaignTransactionService, IChallengeService challengeService,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, httpContextAccessor)
        {
            _walletService = walletService;
            _voucherItemService = voucherItemService;
            _firebaseService = firebaseService;
            _campaignTransactionService = campaignTransactionService;
            _challengeService = challengeService;
        }

        public async Task<IPaginate<VoucherStorageResponse>> GetRedeemedVouchersByStudentAsync(string search, string studentId, bool? isUsed, int page, int size)
        {
            if (string.IsNullOrEmpty(studentId))
                throw new ApiException("Student ID is required", 400, "STUDENT_ID_REQUIRED");
            if (page < 1)
                throw new ApiException("Page number must be greater than 0", 400, "INVALID_PAGE");
            if (size < 1)
                throw new ApiException("Page size must be greater than 0", 400, "INVALID_SIZE");
            if (size > 100)
                size = 100;

            Expression<Func<Activity, bool>> filter = x => x.StudentId == studentId
                && x.VoucherItem.IsBought == true
                && (isUsed == null || x.VoucherItem.IsUsed == isUsed) // Lọc theo isUsed nếu có
                && (string.IsNullOrEmpty(search) || x.VoucherItem.VoucherCode.Contains(search)) // Tìm kiếm theo VoucherCode
                && x.Type == (int?)ActivityType.Buy;

            var vouchers = await _unitOfWork.GetRepository<Activity>()
                .GetPagingListAsync(
                    selector: x => new VoucherStorageResponse
                    {
                        Id = x.VoucherItem.Id,
                        VoucherId = x.VoucherItem.VoucherId,
                        VoucherName = x.VoucherItem.Voucher.VoucherName,
                        VoucherImage = x.VoucherItem.Voucher != null ? x.VoucherItem.Voucher.Image : null,
                        VoucherCode = x.VoucherItem.VoucherCode,
                        TypeName = x.VoucherItem.Voucher != null && x.VoucherItem.Voucher.Type != null ? x.VoucherItem.Voucher.Type.TypeName : null,
                        StudentId = x.StudentId,
                        StudentName = x.Student != null ? x.Student.FullName : null,
                        //BrandId = null,
                        //BrandName = null,
                        //BrandImage = null,
                        CampaignDetailId = x.VoucherItem.CampaignDetailId,
                        CampaignId = x.VoucherItem.CampaignDetail != null ? x.VoucherItem.CampaignDetail.CampaignId : null,
                        CampaignName = x.VoucherItem.CampaignDetail != null && x.VoucherItem.CampaignDetail.Campaign != null ? x.VoucherItem.CampaignDetail.Campaign.CampaignName : null,
                        Price = x.VoucherItem.Voucher != null ? x.VoucherItem.Voucher.Price : null,
                        //Rate = null,
                        IsLocked = x.VoucherItem.IsLocked,
                        IsBought = x.VoucherItem.IsBought,
                        IsUsed = x.VoucherItem.IsUsed,
                        ValidOn = x.VoucherItem.ValidOn,
                        ExpireOn = x.VoucherItem.ExpireOn,
                        //DateCreated = x.DateCreated,
                        //DateLocked = x.VoucherItem.DateIssued,
                        DateBought = x.VoucherItem.DateIssued,
                        //DateUsed = null,
                        Description = x.Description,
                        State = x.State,
                        Status = x.Status
                    },
                    predicate: filter,
                    orderBy: q => q.OrderByDescending(x => x.VoucherItem.DateIssued),
                    include: q => q.Include(x => x.VoucherItem)
                                   .ThenInclude(v => v.Voucher)
                                   .ThenInclude(v => v.Type)
                                   .Include(x => x.VoucherItem)
                                   .ThenInclude(v => v.CampaignDetail)
                                   .ThenInclude(c => c.Campaign)
                                   .Include(x => x.Student),
                    page: page,
                    size: size
                );

            return vouchers;
        }

        public async Task<IPaginate<VoucherStorageGroupByBrandResponse>> GetRedeemedVouchersByStudentGroupedByBrandAsync(
    string search, string studentId, bool? isUsed, int page, int size)
        {
            if (string.IsNullOrEmpty(studentId)) throw new ApiException("Student ID is required", 400, "STUDENT_ID_REQUIRED");
            if (page < 1) throw new ApiException("Page number must be greater than 0", 400, "INVALID_PAGE");
            if (size < 1) throw new ApiException("Page size must be greater than 0", 400, "INVALID_SIZE");
            if (size > 100) size = 100;

            var today = DateOnly.FromDateTime(TimeUtils.GetVietnamToday().AddDays(-10));

            Expression<Func<Activity, bool>> filter = x => x.StudentId == studentId
                && x.VoucherItem.IsBought == true
                && (isUsed == null || x.VoucherItem.IsUsed == isUsed)
                && (string.IsNullOrEmpty(search) || x.VoucherItem.Voucher.VoucherName.Contains(search))
                && x.Type == (int?)ActivityType.Buy
                && x.VoucherItem.ExpireOn >= today;

            // Bước 1: Lấy danh sách Brand với Max(DateIssued)
            var brandQuery = await _unitOfWork.GetRepository<Activity>()
                .GetGroupedPagingListAsync(
                    groupByKey: x => new
                    {
                        x.VoucherItem.Voucher.BrandId,
                        BrandName = x.VoucherItem.Voucher.Brand != null ? x.VoucherItem.Voucher.Brand.BrandName : null,
                        BrandImage = x.VoucherItem.Voucher.Brand != null ? x.VoucherItem.Voucher.Brand.CoverPhoto : null
                    },
                    groupSelector: g => new
                    {
                        BrandId = g.Key.BrandId,
                        BrandName = g.Key.BrandName,
                        BrandImage = g.Key.BrandImage,
                    },
                    predicate: filter,
                    orderBy: q => q.OrderByDescending(g => g.Max(x => x.VoucherItem.DateIssued)),
                    include: q => q.Include(x => x.VoucherItem)
                                   .ThenInclude(v => v.Voucher)
                                   .ThenInclude(v => v.Brand),
                    page: page,
                    size: size
                );

            var brandIds = brandQuery.Items.Select(b => b.BrandId).ToList();

            // Bước 2: Lấy tất cả voucher thuộc các Brand trong trang hiện tại
            var vouchers = await _unitOfWork.GetRepository<Activity>()
                .GetListAsync(
                    selector: x => new
                    {
                        BrandId = x.VoucherItem.Voucher.BrandId,
                        BrandName = x.VoucherItem.Voucher.Brand != null ? x.VoucherItem.Voucher.Brand.BrandName : null,
                        BrandImage = x.VoucherItem.Voucher.Brand != null ? x.VoucherItem.Voucher.Brand.CoverPhoto : null,
                        VoucherId = x.VoucherItem.VoucherId,
                        VoucherName = x.VoucherItem.Voucher.VoucherName,
                        VoucherImage = x.VoucherItem.Voucher.Image,
                        IsUsed = x.VoucherItem.IsUsed,
                        ExpireOn = x.VoucherItem.ExpireOn,
                        CampaignId = x.VoucherItem.CampaignDetail.CampaignId
                    },
                    predicate: filter.AndAlso(x => brandIds.Contains(x.VoucherItem.Voucher.BrandId)),
                    include: q => q.Include(x => x.VoucherItem)
                                   .ThenInclude(v => v.Voucher)
                                   .ThenInclude(v => v.Brand)
                                   .Include(x => x.VoucherItem)
                                   .ThenInclude(v => v.CampaignDetail)
                                   .ThenInclude(c => c.Campaign)
                );

            // Bước 3: Nhóm dữ liệu và giữ thứ tự từ brandQuery
            var groupedVouchers = brandQuery.Items
                .Select(b => new VoucherStorageGroupByBrandResponse
                {
                    BrandId = b.BrandId,
                    BrandName = b.BrandName,
                    BrandImage = b.BrandImage,
                    VoucherGroups = vouchers
                        .Where(v => v.BrandId == b.BrandId)
                        .GroupBy(v => new { v.VoucherId, v.VoucherName, v.VoucherImage, v.ExpireOn, v.CampaignId })
                        .Select(vg => new VoucherGroup
                        {
                            VoucherId = vg.Key.VoucherId,
                            VoucherName = vg.Key.VoucherName,
                            VoucherImage = vg.Key.VoucherImage,
                            ExpireOn = vg.Key.ExpireOn,
                            CampaignId = vg.Key.CampaignId,
                            Quantity = vg.Count(v => v.IsUsed == false),
                            TotalQuantity = vg.Count()
                        }).ToList()
                }).ToList();

            // Trả về kết quả phân trang
            return new Paginate<VoucherStorageGroupByBrandResponse>
            {
                Items = groupedVouchers,
                Page = page,
                Size = size,
                Total = brandQuery.Total,
                TotalPages = (int)Math.Ceiling(brandQuery.Total / (double)size)
            };
        }

        public async Task<bool> RedeemVoucherActivityAsync(ActivityRequest activityRequest)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var brandId = await _unitOfWork.GetRepository<VoucherItem>()
                    .SingleOrDefaultAsync(selector: x => x.Voucher.BrandId,
                                          predicate: x => x.CampaignDetail.CampaignId == activityRequest.CampaignId);

                var redeemedVoucherCount = await _unitOfWork.GetRepository<Activity>()
                    .CountAsync(x => x.StudentId == activityRequest.StudentId
                                     && x.VoucherItem.VoucherId.Equals(activityRequest.VoucherId)
                                     && x.VoucherItem.CampaignDetail.CampaignId == activityRequest.CampaignId
                                     && x.Type == (int?)ActivityType.Buy);

                var usedVoucherCount = await _unitOfWork.GetRepository<Activity>()
                    .CountAsync(x => x.StudentId == activityRequest.StudentId
                                     && x.VoucherItem.VoucherId.Equals(activityRequest.VoucherId)
                                     && x.VoucherItem.CampaignDetail.CampaignId == activityRequest.CampaignId
                                     && x.Type == (int?)ActivityType.Use);

                var storedVoucherCount = redeemedVoucherCount - usedVoucherCount;

                // Check if the student has already redeemed 2 or more vouchers
                if (storedVoucherCount + activityRequest.Quantity > 2)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw new ApiException($"Bạn chỉ có thể lưu trữ tối đa 2 ưu đãi. Hiện tại bạn đang có {storedVoucherCount} ưu đãi.", 400, "REDEEM_LIMIT_EXCEEDED");
                }

                // Kiểm tra wallet
                var wallet = await _walletService.GetWalletByStudentId(activityRequest.StudentId, (int)WalletType.Green);
                var brandWallet = await _walletService.GetWalletByBrandId(brandId, (int)WalletType.Green);
                if (wallet == null || wallet.Balance < activityRequest.Cost)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return false;
                }

                // Redeem voucher và lấy danh sách voucher đã redeem
                var redeemedVouchers = await _voucherItemService.RedeemVoucherAsync(activityRequest.VoucherId, activityRequest.CampaignId, activityRequest.Quantity);
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
                    DateCreated = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.TimeUtils.GetVietnamTimeZone())
                }).ToList();

                // Thêm tất cả activities
                await _unitOfWork.GetRepository<Activity>().InsertRangeAsync(activities);
                await _unitOfWork.CommitAsync();

                // Trừ điểm từ wallet
                await _walletService.UpdateWalletForRedeem(wallet.Id, -(decimal)activityRequest.Cost);

                await _walletService.UpdateWalletForRedeem(brandWallet.Id, (decimal)activityRequest.Cost * PointBackRate);

                // Thêm campaign transaction
                var campaignTransaction = new CampaignTransactionRequest
                {
                    CampaignId = activityRequest.CampaignId,
                    WalletId = brandWallet.Id,
                    Amount = (decimal)activityRequest.Cost * PointBackRate,
                    Rate = PointBackRate,
                    Description = $"Đổi {activityRequest.Quantity} ưu đãi"
                };

                await _campaignTransactionService.AddCampaignTransaction(campaignTransaction);

                // Thêm activity transaction
                await AddActivityTransactionAsync(
                    activities.First().Id,
                    wallet.Id,
                    -(decimal)activityRequest.Cost,
                    $"Đổi {activityRequest.Quantity} ưu đãi"
                );

                var challengeId = await _unitOfWork.GetRepository<Challenge>().SingleOrDefaultAsync(
                            selector: x => x.Id,
                            predicate: x => x.Category!.Contains("Tiêu sài") && x.Type == (int)ChallengeType.Daily);

                var transaction = new ChallengeTransaction
                {
                    Id = Ulid.NewUlid().ToString(),
                    StudentId = activityRequest.StudentId,
                    WalletId = wallet.Id,
                    ChallengeId = challengeId,
                    Amount = activityRequest.Cost,
                    DateCreated = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.TimeUtils.GetVietnamTimeZone()),
                    Type = 0,
                    Description = $"Tiêu sài",
                };

                await _challengeService.AddChallengeTransaction(transaction, (int)ChallengeType.Daily);

                var challenges = await _unitOfWork.GetRepository<Challenge>()
                        .GetListAsync(predicate: x => x.Category.Contains("Tiêu sài") && x.Type == (int)ChallengeType.Achievement);

                if (challenges.Count != 0)
                {
                    await _challengeService.UpdateAchievementProgress(activityRequest.StudentId, challenges, (decimal)activityRequest.Cost);
                }

                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
        public async Task<UseVoucherResponse> UseVoucherActivityAsync(UseVoucherRequest request)
        {
            if (string.IsNullOrEmpty(request.StudentId))
                throw new ApiException("Student ID is required", 400, "STUDENT_ID_REQUIRED");
            if (string.IsNullOrEmpty(request.VoucherId))
                throw new ApiException("Voucher ID is required", 400, "VOUCHER_ID_REQUIRED");
            if (string.IsNullOrEmpty(request.StoreId))
                throw new ApiException("Store ID is required", 400, "STORE_ID_REQUIRED");
            if (string.IsNullOrEmpty(request.VoucherItemId))
                throw new ApiException("Voucher Item ID is required", 400, "VOUCHER_ITEM_ID_REQUIRED");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Lấy VoucherItem chưa dùng
                //var voucherItemId = await _unitOfWork.GetRepository<Activity>()
                //    .SingleOrDefaultAsync(
                //        selector: x => x.VoucherItem.Id,
                //        predicate: x => x.VoucherItem.Voucher.Id == request.VoucherId
                //            && x.VoucherItem.IsBought == true
                //            && x.VoucherItem.IsUsed == false
                //            && x.StudentId == request.StudentId,
                //        include: q => q.Include(x => x.VoucherItem).ThenInclude(v => v.Voucher)

                //);
                var voucherItem = await _unitOfWork.GetRepository<VoucherItem>()
                    .SingleOrDefaultAsync(
                        selector: x => x,
                        predicate: x => x.Id == request.VoucherItemId
                                                && x.IsBought == true
                                                && x.IsUsed == false,
                        include: q => q.Include(x => x.Voucher)
                                       .Include(x => x.CampaignDetail)
                                           .ThenInclude(c => c.Campaign)
                                           .ThenInclude(c => c.CampaignStores)
                    );

                if (voucherItem == null)
                    throw new ApiException("No available voucher found", 404, "VOUCHER_NOT_FOUND");

                // Kiểm tra ExpireOn
                if (voucherItem.ExpireOn.HasValue && voucherItem.ExpireOn.Value < DateOnly.FromDateTime(DateTime.UtcNow))
                    throw new ApiException("Voucher has expired", 400, "VOUCHER_EXPIRED");

                // Kiểm tra StoreId
                var campaign = voucherItem.CampaignDetail?.Campaign;
                if (campaign == null || !campaign.CampaignStores.Any(cs => cs.StoreId == request.StoreId))
                    throw new ApiException($"Voucher cannot be used at store {request.StoreId}", 400, "INVALID_STORE");

                var storeName = await _unitOfWork.GetRepository<Store>()
                    .SingleOrDefaultAsync(
                        selector: x => x.StoreName,
                        predicate: x => x.Id == request.StoreId
                    );

                // Cập nhật VoucherItem
                voucherItem.IsUsed = true;
                _unitOfWork.GetRepository<VoucherItem>().UpdateAsync(voucherItem);

                // Tạo Activity mới cho hành động "Use"
                var useActivity = new Activity
                {
                    Id = Ulid.NewUlid().ToString(),
                    StoreId = request.StoreId,
                    StudentId = request.StudentId,
                    VoucherItemId = voucherItem.Id,
                    Type = (int?)ActivityType.Use,
                    Description = $"Use Voucher {voucherItem.Id} at store {storeName}",
                    State = true,
                    Status = true,
                    DateCreated = DateTime.UtcNow
                };
                await _unitOfWork.GetRepository<Activity>().InsertAsync(useActivity);
                await _unitOfWork.CommitAsync();


                // Thêm ActivityTransaction cho hành động "Use"
                var activityTransaction = await AddActivityTransactionAsync(
                    useActivity.Id,
                    null,
                    0,
                    $"Mã: {voucherItem.Id} \nSử dụng tại cửa hàng {storeName}"
                );
                // Commit transaction
                await _unitOfWork.CommitTransactionAsync();

                var accIdStu = await _unitOfWork.GetRepository<Student>()
                    .SingleOrDefaultAsync(
                        selector: x => x.AccountId,
                        predicate: x => x.Id == request.StudentId
                    );

                var topic = accIdStu;

                if (activityTransaction != null && !topic.IsNullOrEmpty())
                {
                    // Push notification to mobile app
                    _firebaseService.PushNotificationToStudent(new Message
                    {
                        Data = new Dictionary<string, string>()
                                    {
                                        { "brandId", "" },
                                        { "campaignId", "" },
                                        { "image", "" },
                                    },
                        //Token = registrationToken,
                        Topic = topic,
                        Notification = new Notification()
                        {
                            Title = storeName + " đã quét thành công khuyến mãi \"" + voucherItem.Voucher.VoucherName + "\"",
                            Body = voucherItem.Voucher.VoucherName + "\" được sử dụng tại cửa hàng "
                            + storeName,
                        }
                    });
                }

                return new UseVoucherResponse
                {
                    VoucherItemId = voucherItem.Id,
                    VoucherCode = voucherItem.VoucherCode,
                    DateUsed = DateTime.UtcNow,
                };
            }
            catch (Exception ex)
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
                Status = true
            };

            await _unitOfWork.GetRepository<ActivityTransaction>().InsertAsync(transaction);
            var result = await _unitOfWork.CommitAsync() > 0;
            if (!result)
            {
                throw new ApiException("Add activity transaction failed");
            }
            return transaction;
        }

        public async Task<IPaginate<TransactionResponse>> GetAllActivityTransactionAsync(
                                                        string studentId,
                                                        string search,
                                                        int type,
                                                        int page,
                                                        int size)
        {
            // Validate input
            if (page < 1)
                throw new ApiException("Page number must be greater than 0", 400, "INVALID_PAGE");
            if (size < 1)
                throw new ApiException("Page size must be greater than 0", 400, "INVALID_SIZE");
            if (size > 100)
                size = 100;
            if (string.IsNullOrEmpty(studentId))
                throw new ApiException("Student ID is required", 400, "WALLET_ID_REQUIRED");

            // Get wallet ID
            var walletId = await _unitOfWork.GetRepository<Wallet>().SingleOrDefaultAsync(
                selector: x => x.Id,
                predicate: x => x.StudentId == studentId && x.Type == (int)WalletType.Green);

            // Define filters
            Expression<Func<ActivityTransaction, bool>> activityFilter = x =>
                x.WalletId == walletId
                && (string.IsNullOrEmpty(search) || (x.Activity != null && x.Activity.VoucherItem != null));

            Expression<Func<ChallengeTransaction, bool>> challengeFilter = x =>
                x.WalletId == walletId
                && (string.IsNullOrEmpty(search) || (x.StudentChallenge != null && x.StudentChallenge.Challenge != null))
                && x.Type != 0;

            // Handle based on type
            if (type == 1) // Activity Transactions only
            {
                return await _unitOfWork.GetRepository<ActivityTransaction>()
                    .GetPagingListAsync(
                        selector: x => new TransactionResponse
                        {
                            Id = x.Id,
                            TransId = x.ActivityId,
                            WalletId = x.WalletId,
                            Amount = x.Amount,
                            Description = x.Description,
                            Status = x.Status ?? false,
                            Name = x.Activity.VoucherItem.Voucher.VoucherName,
                            CreatedAt = x.Activity.DateCreated
                        },
                        predicate: activityFilter,
                        include: q => q.Include(x => x.Activity).ThenInclude(a => a.VoucherItem).ThenInclude(v => v.Voucher),
                        orderBy: q => q.OrderByDescending(x => x.Activity.DateCreated),
                        page: page,
                        size: size
                    );
            }
            else if (type == 2) // Challenge Transactions only
            {
                return await _unitOfWork.GetRepository<ChallengeTransaction>()
                    .GetPagingListAsync(
                        selector: x => new TransactionResponse
                        {
                            Id = x.Id,
                            TransId = x.ChallengeId,
                            WalletId = x.WalletId,
                            Amount = x.Amount,
                            Description = x.Description,
                            Status = x.Status ?? false,
                            Name = x.StudentChallenge.Challenge.ChallengeName,
                            CreatedAt = x.DateCreated
                        },
                        predicate: challengeFilter,
                        include: q => q.Include(x => x.StudentChallenge).ThenInclude(x => x.Challenge),
                        orderBy: q => q.OrderByDescending(x => x.DateCreated),
                        page: page,
                        size: size
                    );
            }
            else if (type == 0) // All Transactions
            {
                // Fetch Activity Transactions with pagination
                var activityPaged = await _unitOfWork.GetRepository<ActivityTransaction>()
                    .GetPagingListAsync(
                        selector: x => new TransactionResponse
                        {
                            Id = x.Id,
                            TransId = x.ActivityId,
                            WalletId = x.WalletId,
                            Amount = x.Amount,
                            Description = x.Description,
                            Status = x.Status ?? false,
                            Name = x.Activity.VoucherItem.Voucher.VoucherName,
                            CreatedAt = x.Activity.DateCreated
                        },
                        predicate: activityFilter,
                        include: q => q.Include(x => x.Activity).ThenInclude(a => a.VoucherItem).ThenInclude(v => v.Voucher),
                        orderBy: q => q.OrderByDescending(x => x.Activity.DateCreated),
                        page: page,
                        size: size
                    );

                // Fetch Challenge Transactions with pagination
                var challengePaged = await _unitOfWork.GetRepository<ChallengeTransaction>()
                    .GetPagingListAsync(
                        selector: x => new TransactionResponse
                        {
                            Id = x.Id,
                            TransId = x.ChallengeId,
                            WalletId = x.WalletId,
                            Amount = x.Amount,
                            Description = x.Description,
                            Status = x.Status ?? false,
                            Name = x.StudentChallenge.Challenge.ChallengeName,
                            CreatedAt = x.DateCreated
                        },
                        predicate: challengeFilter,
                        include: q => q.Include(x => x.StudentChallenge).ThenInclude(x => x.Challenge),
                        orderBy: q => q.OrderByDescending(x => x.DateCreated),
                        page: page,
                        size: size
                    );

                // Combine results
                var combinedTransactions = activityPaged.Items
                    .Concat(challengePaged.Items)
                    .OrderByDescending(x => x.CreatedAt)
                    .Take(size)
                    .ToList();

                // Calculate total count
                var activityCount = await _unitOfWork.GetRepository<ActivityTransaction>()
                    .CountAsync(activityFilter);
                var challengeCount = await _unitOfWork.GetRepository<ChallengeTransaction>()
                    .CountAsync(challengeFilter);
                var totalCount = activityCount + challengeCount;

                // Create paginated result
                return new Paginate<TransactionResponse>
                {
                    Items = combinedTransactions,
                    Page = page,
                    Size = size,
                    Total = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)size)
                };
            }
            else
            {
                throw new ApiException("Invalid transaction type", 400, "INVALID_TYPE");
            }
        }
        public async Task<IPaginate<TransactionResponse>> GetAllUseVoucherTransactionAsync(
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

            Expression<Func<ActivityTransaction, bool>> filter = x =>
                x.Activity.StudentId == studentId
                && string.IsNullOrEmpty(x.WalletId)
                && x.Amount == 0
                && (string.IsNullOrEmpty(search) || (x.Activity != null && x.Activity.VoucherItem != null && x.Activity.VoucherItem.Voucher.VoucherName.Contains(search)));

            var transactions = await _unitOfWork.GetRepository<ActivityTransaction>()
                .GetPagingListAsync(
                    selector: x => new TransactionResponse
                    {
                        Id = x.Id,
                        TransId = x.ActivityId,
                        WalletId = x.WalletId,
                        Amount = x.Amount,
                        Description = x.Description,
                        Status = x.Status ?? false,
                        Name = x.Activity.VoucherItem.Voucher.VoucherName,
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

        public async Task<IPaginate<TransactionResponse>> GetAllUseVoucherStoreTransactionAsync(string storeId, string search, int page, int size)
        {
            if (page < 1)
                throw new ApiException("Page number must be greater than 0", 400, "INVALID_PAGE");
            if (size < 1)
                throw new ApiException("Page size must be greater than 0", 400, "INVALID_SIZE");
            if (size > 100) // Giới hạn kích thước tối đa
                size = 100;
            if (string.IsNullOrEmpty(storeId))
                throw new ApiException("ID is required", 400, "WALLET_ID_REQUIRED");

            Expression<Func<ActivityTransaction, bool>> filter = x =>
                x.Activity.StoreId == storeId
                && string.IsNullOrEmpty(x.WalletId)
                && x.Amount == 0
                && (string.IsNullOrEmpty(search) || (x.Activity != null && x.Activity.VoucherItem != null && x.Activity.VoucherItem.Voucher.VoucherName.Contains(search)));

            var transactions = await _unitOfWork.GetRepository<ActivityTransaction>()
                .GetPagingListAsync(
                    selector: x => new TransactionResponse
                    {
                        Id = x.Id,
                        TransId = x.ActivityId,
                        WalletId = x.WalletId,
                        Amount = x.Amount,
                        Description = x.Description,
                        Status = x.Status ?? false,
                        Name = x.Activity.VoucherItem.Voucher.VoucherName,
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
