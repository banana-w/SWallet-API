using AutoMapper;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Enums;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Activity;
using SWallet.Repository.Payload.Request.Campaign;
using SWallet.Repository.Payload.Request.CampTransaction;
using SWallet.Repository.Payload.Request.Voucher;
using SWallet.Repository.Payload.Response.Campaign;
using SWallet.Repository.Payload.Response.Store;
using SWallet.Repository.Payload.Response.Voucher;
using SWallet.Repository.Services.Interfaces;
using System.Linq.Expressions;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace SWallet.Repository.Services.Implements
{
    public class CampaignService : BaseService<CampaignService>, ICampaignService
    {
        private readonly Mapper mapper;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IVoucherItemService _voucherItemService;
        private readonly IWalletService _walletService;
        private readonly SwalletDbContext _swalletDB;
        private readonly IFirebaseService _firebaseService;
        private readonly ICampaignTransactionService _campaignTransactionService;

        public record ItemIndex
        {
            public int? FromIndex { get; set; }
            public int? ToIndex { get; set; }
        }

        public CampaignService(IUnitOfWork<SwalletDbContext> unitOfWork, SwalletDbContext swalletDB , ILogger<CampaignService> logger,
            ICloudinaryService cloudinaryService, IWalletService walletService,IVoucherItemService voucherItemService, IFirebaseService firebaseService, 
            ICampaignTransactionService campaignTransactionService, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, httpContextAccessor)
        {
            _cloudinaryService = cloudinaryService;
            _voucherItemService = voucherItemService;
            _swalletDB = swalletDB;
            _walletService = walletService;
            _firebaseService = firebaseService;
            _campaignTransactionService = campaignTransactionService;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateCampaignModel, Campaign>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Ulid.NewUlid().ToString()))
                    .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => ((DateOnly)src.EndOn).DayNumber - ((DateOnly)src.StartOn).DayNumber + 1))
                    .ForMember(dest => dest.TotalSpending, opt => opt.MapFrom(src => 0))
                    .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => DateTime.Now))
                    .ForMember(dest => dest.DateUpdated, opt => opt.MapFrom(src => DateTime.Now))
                    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => true));

                cfg.CreateMap<Campaign, CampaignResponse>()
                    .ForMember(c => c.BrandName, opt => opt.MapFrom(src => src.Brand.BrandName))
                    .ForMember(c => c.BrandAcronym, opt => opt.MapFrom(src => src.Brand.Acronym))
                    .ForMember(c => c.TypeName, opt => opt.MapFrom(src => src.Type.TypeName));

                cfg.CreateMap<CampaignStore, CreateCampaignStoreModel>()
                    .ReverseMap()
                    .ForMember(c => c.Id, opt => opt.MapFrom(src => Ulid.NewUlid()))
                    .ForMember(c => c.Status, opt => opt.MapFrom(src => true));

                cfg.CreateMap<CampaignCampus, CreateCampaignCampusModel>()
                    .ReverseMap()
                    .ForMember(c => c.Id, opt => opt.MapFrom(src => Ulid.NewUlid()))
                    .ForMember(c => c.Status, opt => opt.MapFrom(src => true));

                cfg.CreateMap<CampaignDetail, CreateCampaignDetailModel>()
                    .ReverseMap()
                    .ForMember(c => c.Id, opt => opt.MapFrom(src => Ulid.NewUlid()))
                    .ForMember(c => c.DateCreated, opt => opt.MapFrom(src => DateTime.Now))
                    .ForMember(c => c.DateUpdated, opt => opt.MapFrom(src => DateTime.Now))
                    .ForMember(c => c.Status, opt => opt.MapFrom(src => true));

                cfg.CreateMap<CampaignDetail, CampaignDetailResponse>();
            });

            mapper = new Mapper(config);
        }

        public long CountCampaign()
        {
            long count = 0;
            try
            {
                var db = _swalletDB;
                count = db.Campaigns.Where(c => c.Status == (int)CampaignStatus.Active).Count();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return count;
        }

        public async Task<CampaignResponse> UpdateCampaign(string id, UpdateCampaignModel campaign)
        {
            var updateCampaign = await _unitOfWork.GetRepository<Campaign>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (updateCampaign == null)
            {
                throw new ApiException("Brand not found", 404, "NOT_FOUND");
            }
            if (campaign.Image!= null && campaign.Image.Length > 0)
            {

                var f = await _cloudinaryService.UploadImageAsync(campaign.Image);

            }
            updateCampaign.TypeId = campaign.TypeId;
            updateCampaign.CampaignName = campaign.CampaignName;
            updateCampaign.Condition = campaign.Condition;
            updateCampaign.Link = campaign.Link;
            updateCampaign.Description = campaign.Description;
            updateCampaign.DateUpdated = DateTime.Now;
            updateCampaign.ImageName = !string.IsNullOrEmpty(updateCampaign.Image)
                ? updateCampaign.Image.Split('/')[^1]
                : "default_cover.jpg";
            _unitOfWork.GetRepository<Campaign>().UpdateAsync(updateCampaign);

            var isSuccess = await _unitOfWork.CommitAsync() > 0;
            if (isSuccess)
            {
                return new CampaignResponse
                {
                    Id = updateCampaign.Id,
                    BrandId = updateCampaign.BrandId,
                    TypeId = updateCampaign.TypeId,
                    CampaignName = updateCampaign.CampaignName,
                    Image = updateCampaign.Image,
                    ImageName = updateCampaign.ImageName,
                    Condition = updateCampaign.Condition,
                    Link = updateCampaign.Link,
                    StartOn = updateCampaign.StartOn,
                    EndOn = updateCampaign.EndOn,
                    Duration = updateCampaign.Duration,
                    TotalIncome = updateCampaign.TotalIncome,
                    TotalSpending = updateCampaign.TotalSpending,
                    DateCreated = updateCampaign.DateCreated,
                    DateUpdated = updateCampaign.DateUpdated,
                    Description = updateCampaign.Description,
                    Status = updateCampaign.Status == 1 ? true : false

                };
            }
            throw new ApiException("Update Campaign Fail", 400, "BAD_REQUEST");
        }

        public async Task<CampaignResponse> CreateCampaign(CreateCampaignModel campaignModel, List<CreateCampaignDetailModel> campaignDetails)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Kiểm tra BrandId
                var brand = await _unitOfWork.GetRepository<Brand>().SingleOrDefaultAsync(predicate: b => b.Id == campaignModel.BrandId);
                if (brand == null)
                {
                    throw new ApiException($"Brand with ID {campaignModel.BrandId} not found.", 400, "BAD_REQUEST");
                }

                // Kiểm tra TypeId
                var campaignTypeExists = await _unitOfWork.GetRepository<CampaignType>().SingleOrDefaultAsync(predicate: b => b.Id == campaignModel.TypeId);
                if (campaignTypeExists == null)
                {
                    throw new ApiException($"CampaignType with ID {campaignModel.TypeId} not found.", 400, "BAD_REQUEST");
                }

                // Tính tổng chi phí voucher
                decimal totalVoucherCost = 0m;
                foreach (var cd in campaignDetails)
                {
                    var voucher = await GetVoucherByIdAsync(cd.VoucherId);
                    if (voucher == null)
                    {
                        throw new ApiException($"Voucher with ID {cd.VoucherId} not found.", 400, "BAD_REQUEST");
                    }
                    totalVoucherCost += voucher.Price.GetValueOrDefault(0m) * cd.Quantity.GetValueOrDefault(0);
                }

                totalVoucherCost += (decimal)campaignTypeExists.Coin;

                // Kiểm tra số dư trong ví của Brand
                var brandWalletBalance = await _walletService.GetWalletByBrandId(campaignModel.BrandId, 1);
                if (brandWalletBalance.Balance < totalVoucherCost)
                {
                    throw new ApiException("Balance not enough for those quantity of vouchers", 400, "BAD_REQUEST");
                }

                // Tạo Campaign mới
                var newCampaign = new Campaign
                {
                    Id = Ulid.NewUlid().ToString(),
                    BrandId = campaignModel.BrandId,
                    TypeId = campaignModel.TypeId,
                    CampaignName = campaignModel.CampaignName,
                    Condition = campaignModel.Condition,
                    Link = campaignModel.Link,
                    File = "default_value_or_empty_string",
                    Image = "string",
                    ImageName = "string",
                    FileName = "default_value_or_empty_string",
                    StartOn = campaignModel.StartOn,
                    EndOn = campaignModel.EndOn,
                    Duration = ((DateOnly)campaignModel.EndOn).DayNumber - ((DateOnly)campaignModel.StartOn).DayNumber + 1,
                    TotalIncome = campaignModel.TotalIncome,
                    TotalSpending = totalVoucherCost,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Description = campaignModel.Description,
                    Status = (int)CampaignStatus.Pending,
                };

                // Thêm Campaign vào DbContext
                await _unitOfWork.GetRepository<Campaign>().InsertAsync(newCampaign);

                // Cập nhật TotalSpending của Brand
                brand.TotalSpending = (brand.TotalSpending ?? 0m) + totalVoucherCost;
                _unitOfWork.GetRepository<Brand>().UpdateAsync(brand);

                // Thêm CampaignStore
                foreach (var storeId in campaignModel.StoreIds)
                {
                    var storeExists = await _unitOfWork.GetRepository<Store>().AnyAsync(s => s.Id == storeId);
                    if (!storeExists)
                    {
                        throw new ApiException($"Store with ID {storeId} not found.", 400, "BAD_REQUEST");
                    }

                    var campaignStore = new CampaignStore
                    {
                        Id = Ulid.NewUlid().ToString(),
                        CampaignId = newCampaign.Id,
                        StoreId = storeId,
                        Description = "Campaign Store",
                        Status = true
                    };

                    newCampaign.CampaignStores.Add(campaignStore);
                }

                // Thêm CampaignDetail
                foreach (var cd in campaignDetails)
                {
                    var voucher = await GetVoucherByIdAsync(cd.VoucherId);
                    var campaignDetail = new CampaignDetail
                    {
                        Id = Ulid.NewUlid().ToString(),
                        VoucherId = cd.VoucherId,
                        Price = voucher.Price,
                        Rate = voucher.Rate,
                        Quantity = cd.Quantity,
                        FromIndex = cd.FromIndex,
                        ToIndex = 10,
                        DateCreated = DateTime.UtcNow,
                        DateUpdated = DateTime.UtcNow,
                        Description = cd.Description,
                        State = cd.State,
                        Status = true
                    };

                    newCampaign.CampaignDetails.Add(campaignDetail);

                    var isSuccess = await _unitOfWork.CommitAsync() > 0;
                    if (!isSuccess)
                    {
                        throw new ApiException("Create CampaignDetail Fail", 400, "BAD_REQUEST");
                    }

                    var voucherItemSuccess = await _voucherItemService.GenerateVoucherItemsAsync(new VoucherItemRequest
                    {
                        VoucherId = cd.VoucherId,
                        CampaignDetailId = campaignDetail.Id,
                        Quantity = (int)cd.Quantity,
                        ValidOn = campaignModel.StartOn,
                        ExpireOn = campaignModel.EndOn
                    });

                    if (!voucherItemSuccess)
                    {
                        throw new ApiException("Generate VoucherItem Fail", 400, "BAD_REQUEST");
                    }
                }

                // Upload hình ảnh
                var imageUri = string.Empty;
                if (campaignModel.Image != null && campaignModel.Image.Length > 0)
                {
                    var uploadResult = await _cloudinaryService.UploadImageAsync(campaignModel.Image);
                    imageUri = uploadResult.SecureUrl.AbsoluteUri;
                    newCampaign.Image = imageUri;
                    newCampaign.ImageName = !string.IsNullOrEmpty(imageUri)
                        ? imageUri.Split('/')[^1]
                        : "default_cover.jpg";
                }

                // Trừ số dư ví của Brand
                var deductSuccess = await _walletService.UpdateWallet(brandWalletBalance.Id, (decimal)(brandWalletBalance.Balance - totalVoucherCost));
                if (deductSuccess == null)
                {
                    throw new ApiException("Failed to deduct balance from Brand wallet", 400, "BAD_REQUEST");
                }

                // Thêm campaign transaction
                var campaignTransaction = new CampaignTransactionRequest
                {
                    CampaignId = newCampaign.Id,
                    WalletId = brandWalletBalance.Id,
                    Amount = -totalVoucherCost,
                    Rate = 1,
                    Description = $"Create Campaign"
                };

                await _campaignTransactionService.AddCampaignTransaction(campaignTransaction);

                // Commit transaction
                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                return new CampaignResponse
                {
                    Id = newCampaign.Id,
                    BrandId = newCampaign.BrandId,
                    BrandAcronym = brand.Acronym,
                    BrandName = brand.BrandName,
                    TypeId = newCampaign.TypeId,
                    TypeName = campaignTypeExists.TypeName,
                    CampaignName = newCampaign.CampaignName,
                    Image = newCampaign.Image,
                    ImageName = newCampaign.ImageName,
                    File = newCampaign.File,
                    FileName = newCampaign.FileName,
                    Condition = newCampaign.Condition,
                    Link = newCampaign.Link,
                    StartOn = newCampaign.StartOn,
                    EndOn = newCampaign.EndOn,
                    Duration = newCampaign.Duration,
                    TotalIncome = newCampaign.TotalIncome,
                    TotalSpending = newCampaign.TotalSpending,
                    DateCreated = newCampaign.DateCreated,
                    DateUpdated = newCampaign.DateUpdated,
                    Description = newCampaign.Description,
                    Status = newCampaign.Status == 1
                };
            }
            catch (ApiException ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                Console.WriteLine($"API Exception: {ex.Message}, Status Code: {ex.StatusCode}, Error Code: {ex.ErrorCode}");
                throw;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                Console.WriteLine($"Exception: {ex.Message}, Stack Trace: {ex.StackTrace}");
                throw new ApiException("Create Campaign Fail", 500, "INTERNAL_SERVER_ERROR");
            }
        }

        public async Task<CampaignResponse> ApproveOrRejectCampaign(string campaignId, bool isApproved, string? rejectionReason = null)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var campaign = await _unitOfWork.GetRepository<Campaign>()
                    .SingleOrDefaultAsync(
                        predicate: x => x.Id == campaignId && x.Status == (int)CampaignStatus.Pending,
                        include: x => x.Include(x => x.Brand).Include(x => x.Type)
                    );

                if (campaign == null)
                {
                    throw new ApiException("Campaign not found or not in pending status", 404, "NOT_FOUND");
                }

                if (isApproved)
                {
                    // Approve campaign
                    campaign.Status = (int)CampaignStatus.Active;
                    campaign.DateUpdated = DateTime.Now;
                    _unitOfWork.GetRepository<Campaign>().UpdateAsync(campaign);
                }
                else
                {
                    // Reject campaign
                    if (string.IsNullOrEmpty(rejectionReason))
                    {
                        throw new ApiException("Rejection reason is required", 400, "BAD_REQUEST");
                    }

                    campaign.Status = (int)CampaignStatus.Rejected;
                    campaign.File = $"{rejectionReason}";
                    campaign.DateUpdated = DateTime.Now;
                    _unitOfWork.GetRepository<Campaign>().UpdateAsync(campaign);
                    await _unitOfWork.CommitAsync();

                    // Refund wallet balance
                    var campaignDetails = await _unitOfWork.GetRepository<CampaignDetail>()
                        .GetListAsync(predicate: x => x.CampaignId == campaignId);

                    decimal totalRefund = campaignDetails
                        .Sum(cd => cd.Price.GetValueOrDefault(0m) * cd.Quantity.GetValueOrDefault(0));

                    var brandWallet = await _walletService.GetWalletByBrandId(campaign.BrandId, 1);
                    var refundSuccess = await _walletService.UpdateWallet(
                        brandWallet.Id,
                        (decimal)(brandWallet.Balance + totalRefund)
                    );

                    if (refundSuccess == null)
                    {
                        throw new ApiException("Failed to refund wallet balance", 400, "BAD_REQUEST");
                    }

                    // Thêm campaign transaction
                    var campaignTransaction = new CampaignTransactionRequest
                    {
                        CampaignId = campaignId,
                        WalletId = brandWallet.Id,
                        Amount = totalRefund,
                        Rate = 1,
                        Description = $"Refund coin for rejected campaign"
                    };

                    await _campaignTransactionService.AddCampaignTransaction(campaignTransaction);

                    //// Update brand total spending
                    //var brand = await _unitOfWork.GetRepository<Brand>()
                    //    .SingleOrDefaultAsync(predicate: b => b.Id == campaign.BrandId);
                    //brand.TotalSpending = (brand.TotalSpending ?? 0m) - totalRefund;
                    //_unitOfWork.GetRepository<Brand>().UpdateAsync(brand);
                    campaign.Brand.TotalSpending = (campaign.Brand.TotalSpending ?? 0m) - totalRefund;
                    _unitOfWork.GetRepository<Campaign>().UpdateAsync(campaign);
                }

                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                if (isApproved)
                {
                    // Send notification to wishlist subscribers
                    _firebaseService.PushNotificationToStudent(new Message
                    {
                        Data = new Dictionary<string, string>()
                        {
                            { "brandId", campaign.BrandId },
                            { "campaignId", campaign.Id },
                            { "image", campaign.Image },
                        },
                        Topic = campaign.BrandId,
                        Notification = new Notification()
                        {
                            Title = campaign.Brand.BrandName + " tạo chiến dịch mới!",
                            Body = "Chiến dịch " + campaign.CampaignName,
                            ImageUrl = campaign.Image
                        }
                    });
                }

                return new CampaignResponse
                {
                    Id = campaign.Id,
                    BrandId = campaign.BrandId,
                    BrandName = campaign.Brand.BrandName,
                    BrandAcronym = campaign.Brand.Acronym,
                    TypeId = campaign.TypeId,
                    TypeName = campaign.Type.TypeName,
                    CampaignName = campaign.CampaignName,
                    Image = campaign.Image,
                    ImageName = campaign.ImageName,
                    File = campaign.File,
                    Condition = campaign.Condition,
                    Link = campaign.Link,
                    StartOn = campaign.StartOn,
                    EndOn = campaign.EndOn,
                    Duration = campaign.Duration,
                    TotalIncome = campaign.TotalIncome,
                    TotalSpending = campaign.TotalSpending,
                    DateCreated = campaign.DateCreated,
                    DateUpdated = campaign.DateUpdated,
                    Description = campaign.Description,
                    Status = campaign.Status == 1
                };
            }
            catch (ApiException ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new ApiException("Failed to process campaign approval/rejection", 500, "INTERNAL_SERVER_ERROR");
            }
        }



        public async Task<VoucherResponse> GetVoucherByIdAsync(string voucherId)
        {
            var voucher = await _unitOfWork.GetRepository<Voucher>().SingleOrDefaultAsync(
                selector: x => new VoucherResponse
                {
                    Id = x.Id,
                    VoucherName = x.VoucherName,
                    Price = x.Price,
                    Rate = x.Rate,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    State = x.State,
                    Status = x.Status
                },
                predicate: x => x.Id == voucherId && (bool)x.State && (bool)x.Status
            );

            return voucher;
        }

        public async Task<ItemIndex> GetIndexAsync(string voucherId, int quantity, int fromIndex)
        {
            try
            {
                var list = (await _unitOfWork.GetRepository<VoucherItem>()
                    .GetListAsync(
                        selector: x => x,
                        predicate: x => x.VoucherId.Equals(voucherId)
                            && (bool)x.State && (bool)x.Status
                            && !(bool)x.IsLocked && !(bool)x.IsBought && !(bool)x.IsUsed
                            && (fromIndex == 0 || x.Index >= fromIndex)
                            && x.CampaignDetail == null
                    ))
                    .Take(quantity)
                    .ToList();

                if (list == null || !list.Any())
                {
                    throw new Exception("Không tìm thấy voucher item phù hợp.");
                }

                return new ItemIndex
                {
                    FromIndex = list.First().Index,
                    ToIndex = list.Last().Index
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateListAsync(
            string voucherId,
            string campaignDetailId,
            int quantity,
            DateOnly startOn,
            DateOnly endOn,
            ItemIndex index)
        {
            try
            {
                var list = await _unitOfWork.GetRepository<VoucherItem>()
                    .GetQueryable()
                    .Where(x => x.VoucherId.Equals(voucherId)
                        && (bool)x.State && (bool)x.Status
                        && x.Index >= index.FromIndex && x.Index <= index.ToIndex
                        && !(bool)x.IsLocked && !(bool)x.IsBought && !(bool)x.IsUsed
                        && x.CampaignDetail == null)
                    .Take(quantity)
                    .ToListAsync();

                var updatedItems = list.Select(i =>
                {
                    i.CampaignDetailId = campaignDetailId;
                    i.IsLocked = true;
                    i.ValidOn = startOn;
                    i.ExpireOn = endOn;
                    i.DateIssued = DateTime.Now;
                    return i;
                }).ToList();

                _unitOfWork.GetRepository<VoucherItem>().UpdateRange(updatedItems);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<CampaignResponseExtra> GetCampaignById(string id)
        {
            var campaign = await _unitOfWork.GetRepository<Campaign>().SingleOrDefaultAsync(
                selector: x => new CampaignResponseExtra
                {
                    Id = x.Id,
                    BrandId = x.BrandId,
                    BrandName = x.Brand.BrandName,
                    BrandAcronym = x.Brand.Acronym,
                    BrandLogo = x.Brand.Account.Avatar,
                    TypeId = x.TypeId,
                    TypeName = x.Type.TypeName,
                    CampaignName = x.CampaignName,
                    Image = x.Image,
                    ImageName = x.ImageName,
                    Condition = x.Condition,
                    File = x.File,
                    Link = x.Link,
                    StartOn = x.StartOn,
                    EndOn = x.EndOn,
                    Duration = x.Duration,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    Status = x.Status == 1,
                    TotalIncome = x.TotalIncome,
                    TotalSpending = x.TotalSpending,
                    CampaignDetailId = x.CampaignDetails.Select(cd => cd.Id)
                },
                
                predicate: x => x.Id == id,
                include: x => x.Include(x => x.CampaignDetails).Include(x => x.Type)
                               .Include(x => x.Brand).ThenInclude(x => x.Account));
            ;
            if (campaign == null)
            {
                throw new ApiException("Campaign not found", 404, "NOT_FOUND");
            }
            return campaign;
        }
        
        public async Task<CampaignResponseExtraAllStatus> GetCampaignByIdAllStatus(string id)
        {
            var campaign = await _unitOfWork.GetRepository<Campaign>().SingleOrDefaultAsync(
                selector: x => new CampaignResponseExtraAllStatus
                {
                    Id = x.Id,
                    BrandId = x.BrandId,
                    BrandName = x.Brand.BrandName,
                    BrandAcronym = x.Brand.Acronym,
                    BrandLogo = x.Brand.Account.Avatar,
                    TypeId = x.TypeId,
                    TypeName = x.Type.TypeName,
                    CampaignName = x.CampaignName,
                    Image = x.Image,
                    ImageName = x.ImageName,
                    File = x.File,
                    Condition = x.Condition,
                    Link = x.Link,
                    StartOn = x.StartOn,
                    EndOn = x.EndOn,
                    Duration = x.Duration,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    Status = x.Status,
                    TotalIncome = x.TotalIncome,
                    TotalSpending = x.TotalSpending,
                    CampaignDetailId = x.CampaignDetails.Select(cd => cd.Id)
                },
                
                predicate: x => x.Id == id,
                include: x => x.Include(x => x.CampaignDetails).Include(x => x.Type)
                               .Include(x => x.Brand).ThenInclude(x => x.Account));
            ;
            if (campaign == null)
            {
                throw new ApiException("Campaign not found", 404, "NOT_FOUND");
            }
            return campaign;
        }

        public async Task<IPaginate<CampaignResponse>> GetCampaignsInBrand(string? searchName, int page, int size)
        {
            var brandId = GetBrandIdFromJwt();
            Expression<Func<Campaign, bool>> filterQuery;
            if (string.IsNullOrEmpty(searchName))
            {
                filterQuery = p => p.BrandId == brandId;
            }
            else
            {
                filterQuery = p => p.BrandId == brandId && p.CampaignName.Contains(searchName);
            }         

            var campaigns = await _unitOfWork.GetRepository<Campaign>().GetPagingListAsync(
                selector: x => new CampaignResponse
                {
                    Id = x.Id,
                    BrandId = x.BrandId,
                    BrandName = x.Brand.BrandName,
                    BrandAcronym = x.Brand.Acronym,
                    TypeId = x.TypeId,
                    TypeName = x.Type.TypeName,
                    CampaignName = x.CampaignName,
                    Image = x.Image,
                    ImageName = x.ImageName,
                    Condition = x.Condition,
                    Link = x.Link,
                    StartOn = x.StartOn,
                    EndOn = x.EndOn,
                    Duration = x.Duration,
                    TotalIncome = x.TotalIncome,
                    TotalSpending = x.TotalSpending,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    Status = x.Status == 1
                },
                predicate: filterQuery,
                page: page,
                include: query => query.Include(x => x.Brand).Include(x => x.Type),
                size: size);
            return campaigns;
        }


        public async Task<IEnumerable<CampaignDetailResponse>> GetAllCampaignDetails()
        {
            // Lấy toàn bộ dữ liệu từ cơ sở dữ liệu
            var campaignDetails = await _unitOfWork.GetRepository<CampaignDetail>()
                .GetListAsync(
                    selector: x => x, // Chọn toàn bộ đối tượng CampaignDetail
                    predicate: null, // Không có điều kiện lọc
                    include: null // Không include thêm dữ liệu liên quan
                );

            // Ánh xạ dữ liệu sang CampaignDetailResponse
            var result = mapper.Map<IEnumerable<CampaignDetailResponse>>(campaignDetails);

            return result;
        }

        public async Task<IPaginate<StoreResponse>> GetStoresByCampaignId(string campaignId, string searchName, int page, int size)
        {
            Expression<Func<CampaignStore, bool>> filterQuery;

            if (string.IsNullOrEmpty(searchName))
            {
                filterQuery = p => p.CampaignId == campaignId;
            }
            else
            {
                filterQuery = p => p.CampaignId == campaignId && p.CampaignId.Contains(searchName);
            }

            var stores = await _unitOfWork.GetRepository<CampaignStore>().GetPagingListAsync(
                selector: x => new StoreResponse
                {
                    Id = x.Store.Id,
                    AccountId = x.Store.AccountId,
                    BrandId = x.Store.BrandId,
                    BrandName = x.Store.Brand.BrandName,
                    AreaId = x.Store.AreaId,
                    AreaName = x.Store.Area.AreaName,
                    StoreName = x.Store.StoreName,
                    Address = x.Store.Address,
                    OpeningHours = x.Store.OpeningHours,
                    ClosingHours = x.Store.ClosingHours,
                    DateCreated = x.Store.DateCreated,
                    DateUpdated = x.Store.DateUpdated,
                    Description = x.Store.Description,
                    State = x.Store.State,
                    Status = x.Store.Account.Status,
                    UserName = x.Store.Account.UserName,
                    Email = x.Store.Account.Email,
                    Phone = x.Store.Account.Phone,
                    File = x.Store.File,
                },
                predicate: filterQuery,
                include: x => x.Include(a => a.Store).ThenInclude(a => a.Account),
                page: page,
                size: size);

            return stores;
        }

        public async Task<IPaginate<CampaignResponse>> GetCampaigns(string? searchName, int page, int size)
        {
            Expression<Func<Campaign, bool>> filterQuery;
            if (string.IsNullOrEmpty(searchName))
            {
                filterQuery = p => p.Status == (int)CampaignStatus.Active;
            }
            else
            {
                filterQuery = p => p.CampaignName.Contains(searchName) && p.Status == (int)CampaignStatus.Active;
            }

            var campaigns = await _unitOfWork.GetRepository<Campaign>().GetPagingListAsync(
                selector: x => new CampaignResponse
                {
                    Id = x.Id,
                    BrandId = x.BrandId,
                    BrandName = x.Brand.BrandName,
                    BrandAcronym = x.Brand.Acronym,
                    TypeId = x.TypeId,
                    TypeName = x.Type.TypeName,
                    CampaignName = x.CampaignName,
                    Image = x.Image,
                    ImageName = x.ImageName,
                    Condition = x.Condition,
                    Link = x.Link,
                    StartOn = x.StartOn,
                    EndOn = x.EndOn,
                    Duration = x.Duration,
                    TotalIncome = x.TotalIncome,
                    TotalSpending = x.TotalSpending,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    Status = x.Status == 1
                },
                predicate: filterQuery,
                page: page,
                include: query => query.Include(x => x.Brand).Include(x => x.Type),
                size: size);
            return campaigns;
        }

        public async Task<IPaginate<CampaignResponseAllStatus>> GetCampaignsAll(string? searchName, int page, int size)
        {
            Expression<Func<Campaign, bool>> filterQuery;
            if (string.IsNullOrEmpty(searchName))
            {
                filterQuery = p => true;
            }
            else
            {
                filterQuery = p => p.CampaignName.Contains(searchName);
            }

            var campaigns = await _unitOfWork.GetRepository<Campaign>().GetPagingListAsync(
                selector: x => new CampaignResponseAllStatus
                {
                    Id = x.Id,
                    BrandId = x.BrandId,
                    BrandName = x.Brand.BrandName,
                    BrandAcronym = x.Brand.Acronym,
                    TypeId = x.TypeId,
                    TypeName = x.Type.TypeName,
                    CampaignName = x.CampaignName,
                    Image = x.Image,
                    ImageName = x.ImageName,
                    Condition = x.Condition,
                    Link = x.Link,
                    StartOn = x.StartOn,
                    EndOn = x.EndOn,
                    Duration = x.Duration,
                    TotalIncome = x.TotalIncome,
                    TotalSpending = x.TotalSpending,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    Status = x.Status
                },
                predicate: filterQuery,
                page: page,
                include: query => query.Include(x => x.Brand).Include(x => x.Type),
                size: size,
                orderBy: x => x.OrderByDescending(x => x.DateCreated));
            return campaigns;
        }

        public async Task<IPaginate<CampaignResponse>> GetCampaignsByBrandId(string brandId, string? searchName, int page, int size)
        {
            Expression<Func<Campaign, bool>> filterQuery;
            if (string.IsNullOrEmpty(searchName))
            {
                filterQuery = p => p.BrandId == brandId;
            }
            else
            {
                filterQuery = p => p.BrandId == brandId && p.CampaignName.Contains(searchName);
            }

            var campaigns = await _unitOfWork.GetRepository<Campaign>().GetPagingListAsync(
                selector: x => new CampaignResponse
                {
                    Id = x.Id,
                    BrandId = x.BrandId,
                    BrandName = x.Brand.BrandName,
                    BrandAcronym = x.Brand.Acronym,
                    TypeId = x.TypeId,
                    TypeName = x.Type.TypeName,
                    CampaignName = x.CampaignName,
                    Image = x.Image,
                    ImageName = x.ImageName,
                    Condition = x.Condition,
                    Link = x.Link,
                    StartOn = x.StartOn,
                    EndOn = x.EndOn,
                    Duration = x.Duration,
                    TotalIncome = x.TotalIncome,
                    TotalSpending = x.TotalSpending,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    Status = x.Status == 1
                },
                predicate: filterQuery,
                page: page,
                include: query => query.Include(x => x.Brand).Include(x => x.Type),
                size: size);
            return campaigns;
        }
        public async Task<IPaginate<CampaignResponseAllStatus>> GetCampaignsByBrandIdAllStatus(string brandId, string? searchName, int page, int size)
        {
            Expression<Func<Campaign, bool>> filterQuery;
            if (string.IsNullOrEmpty(searchName))
            {
                filterQuery = p => p.BrandId == brandId;
            }
            else
            {
                filterQuery = p => p.BrandId == brandId && p.CampaignName.Contains(searchName);
            }

            var campaigns = await _unitOfWork.GetRepository<Campaign>().GetPagingListAsync(
                selector: x => new CampaignResponseAllStatus
                {
                    Id = x.Id,
                    BrandId = x.BrandId,
                    BrandName = x.Brand.BrandName,
                    BrandAcronym = x.Brand.Acronym,
                    TypeId = x.TypeId,
                    TypeName = x.Type.TypeName,
                    CampaignName = x.CampaignName,
                    Image = x.Image,
                    ImageName = x.ImageName,
                    Condition = x.Condition,
                    Link = x.Link,
                    StartOn = x.StartOn,
                    EndOn = x.EndOn,
                    Duration = x.Duration,
                    TotalIncome = x.TotalIncome,
                    TotalSpending = x.TotalSpending,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    Status = x.Status
                },
                predicate: filterQuery,
                page: page,
                include: query => query.Include(x => x.Brand).Include(x => x.Type),
                size: size,
                orderBy: x => x.OrderByDescending(x => x.DateCreated));
            return campaigns;
        }

        public async Task<List<Campaign>> GetRanking(string brandId, int limit)
        {
            if (string.IsNullOrEmpty(brandId))
                throw new ArgumentException("BrandId không được để trống", nameof(brandId));
            if (limit <= 0)
                throw new ArgumentException("Giới hạn phải lớn hơn 0", nameof(limit));

            try
            {
                return await _swalletDB.Campaigns
                    .Where(c => c.BrandId.Equals(brandId) && c.Status == (int)CampaignStatus.Active)
                    .OrderByDescending(c => c.TotalSpending)
                    .Take(limit)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách xếp hạng chiến dịch: {ex.Message}", ex);
            }
        }

        //public async Task<IPaginate<CampaignResponse>> GetStoresByCampaignId(string campaignId, int page, int size)
        //{
        //    var stores = await _unitOfWork.GetRepository<Store>().GetPagingListAsync(
        //        selector: x => new StoreResponse
        //        {
        //            Id = x.Id,
        //            AccountId = x.AccountId,
        //            BrandId = x.BrandId,
        //            BrandName = x.Brand.BrandName,
        //            AreaId = x.AreaId,
        //            AreaName = x.Area.AreaName,
        //            StoreName = x.StoreName,
        //            Address = x.Address,
        //            OpeningHours = x.OpeningHours,
        //            ClosingHours = x.ClosingHours,
        //            DateCreated = x.DateCreated,
        //            DateUpdated = x.DateUpdated,
        //            Description = x.Description,
        //            State = x.State,
        //            Status = x.Account.Status,
        //            UserName = x.Account.UserName,
        //            Email = x.Account.Email,
        //            Phone = x.Account.Phone,
        //            Avatar = x.Account.Avatar,
        //            AvatarFileName = x.Account.FileName,
        //        },
        //        predicate: filterQuery,
        //        include: x => x.Include(a => a.Account).Include(b => b.Brand).Include(c => c.Area),
        //        page: page,
        //        size: size);

        //    return stores;
        //}

    }
}