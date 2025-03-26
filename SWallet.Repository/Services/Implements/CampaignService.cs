using AutoMapper;
using CloudinaryDotNet.Core;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Campaign;
using SWallet.Repository.Payload.Request.Voucher;
using SWallet.Repository.Payload.Response.Campaign;
using SWallet.Repository.Payload.Response.Store;
using SWallet.Repository.Payload.Response.Voucher;
using SWallet.Repository.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace SWallet.Repository.Services.Implements
{
    public class CampaignService : BaseService<CampaignService>, ICampaignService
    {
        private readonly Mapper mapper;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IVoucherItemService _voucherItemService;

        public record ItemIndex
        {
            public int? FromIndex { get; set; }
            public int? ToIndex { get; set; }
        }

        public CampaignService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<CampaignService> logger, ICloudinaryService cloudinaryService, IVoucherItemService voucherItemService, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, httpContextAccessor)
        {
            _cloudinaryService = cloudinaryService;
            _voucherItemService = voucherItemService;

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
                    Status = updateCampaign.Status

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
                var brandExists = await _unitOfWork.GetRepository<Brand>().SingleOrDefaultAsync(predicate: b => b.Id == campaignModel.BrandId);
                if (brandExists == null)
                {
                    throw new ApiException($"Brand with ID {campaignModel.BrandId} not found.", 400, "BAD_REQUEST");
                }

                // Kiểm tra TypeId
                var typeExists = await _unitOfWork.GetRepository<CampaignType>().SingleOrDefaultAsync(predicate: b => b.Id == campaignModel.TypeId);
                if (typeExists == null)
                {
                    throw new ApiException($"CampaignType with ID {campaignModel.TypeId} not found.", 400, "BAD_REQUEST");
                }

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
                    TotalSpending = 0,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Description = campaignModel.Description,
                    Status = true,
                };

                // Thêm Campaign vào DbContext
                await _unitOfWork.GetRepository<Campaign>().InsertAsync(newCampaign);

                // Thêm CampaignStore
                foreach (var storeId in campaignModel.StoreIds)
                {
                    // Kiểm tra StoreId
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
                    // Lấy thông tin Voucher
                    var voucher = await GetVoucherByIdAsync(cd.VoucherId);
                    if (voucher == null)
                    {
                        throw new ApiException($"Voucher with ID {cd.VoucherId} not found.", 400, "BAD_REQUEST");
                    }

                    // Tạo CampaignDetail
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

                    // Cập nhật danh sách VoucherItem
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

                // Upload hình ảnh (chỉ sau khi tất cả các thao tác trên thành công)
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

                // Commit transaction
                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                return new CampaignResponse
                {
                    Id = newCampaign.Id,
                    BrandId = newCampaign.BrandId,
                    BrandAcronym = brandExists.Acronym,
                    BrandName = brandExists.BrandName,
                    TypeId = newCampaign.TypeId,
                    TypeName = typeExists.TypeName,
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
                    Status = newCampaign.Status
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

        //public async Task<CampaignResponse> CreateCampaign(CreateCampaignModel campaignModel, List<CreateCampaignDetailModel> campaignDetails)
        //{
        //    // Kiểm tra BrandId
        //    var brandExists = await _unitOfWork.GetRepository<Brand>().SingleOrDefaultAsync(predicate: b => b.Id == campaignModel.BrandId);
        //    if (brandExists == null)
        //    {
        //        throw new ApiException($"Brand with ID {campaignModel.BrandId} not found.", 400, "BAD_REQUEST");
        //    }

        //    // Kiểm tra TypeId
        //    var typeExists = await _unitOfWork.GetRepository<CampaignType>().SingleOrDefaultAsync(predicate: b => b.Id == campaignModel.TypeId);
        //    if (typeExists == null)
        //    {
        //        throw new ApiException($"CampaignType with ID {campaignModel.TypeId} not found.", 400, "BAD_REQUEST");
        //    }

        //    // Upload hình ảnh
        //    var imageUri = string.Empty;
        //    if (campaignModel.Image != null && campaignModel.Image.Length > 0)
        //    {
        //        var uploadResult = await _cloudinaryService.UploadImageAsync(campaignModel.Image);
        //        imageUri = uploadResult.SecureUrl.AbsoluteUri;
        //    }

        //    var newCampaign = new Campaign
        //    {
        //        Id = Ulid.NewUlid().ToString(),

        //        BrandId = campaignModel.BrandId,
        //        TypeId = campaignModel.TypeId,
        //        CampaignName = campaignModel.CampaignName,
        //        Image = imageUri,
        //        ImageName = !string.IsNullOrEmpty(imageUri)
        //            ? imageUri.Split('/')[^1]
        //            : "default_cover.jpg",
        //        Condition = campaignModel.Condition,
        //        Link = campaignModel.Link,
        //        File = "default_value_or_empty_string",
        //        FileName = "default_value_or_empty_string",
        //        StartOn = campaignModel.StartOn,
        //        EndOn = campaignModel.EndOn,
        //        Duration = ((DateOnly)campaignModel.EndOn).DayNumber - ((DateOnly)campaignModel.StartOn).DayNumber + 1,
        //        TotalIncome = campaignModel.TotalIncome,
        //        TotalSpending = 0,
        //        DateCreated = DateTime.Now,
        //        DateUpdated = DateTime.Now,
        //        Description = campaignModel.Description,
        //        Status = true,
        //    };

        //    // Thêm Campaign vào DbContext
        //    await _unitOfWork.GetRepository<Campaign>().InsertAsync(newCampaign);

        //    // Thêm CampaignStore
        //    foreach (var storeId in campaignModel.StoreIds)
        //    {
        //        var campaignStore = new CampaignStore
        //        {
        //            Id = Ulid.NewUlid().ToString(),
        //            CampaignId = newCampaign.Id,
        //            StoreId = storeId,
        //            Description = "Campaign Store",
        //            State = true,
        //            Status = true
        //        };

        //        newCampaign.CampaignStores.Add(campaignStore);
        //    }

        //    // Thêm CampaignDetail
        //    foreach (var cd in campaignDetails)
        //    {
        //        // Lấy thông tin Voucher
        //        var voucher = await GetVoucherByIdAsync(cd.VoucherId);
        //        if (voucher == null)
        //        {
        //            throw new Exception($"Voucher with ID {cd.VoucherId} not found.");
        //        }

        //        // Tạo CampaignDetail
        //        var campaignDetail = new CampaignDetail
        //        {
        //            Id = Ulid.NewUlid().ToString(),
        //            VoucherId = cd.VoucherId,
        //            Price = voucher.Price,
        //            Rate = voucher.Rate,
        //            Quantity = cd.Quantity,
        //            FromIndex = cd.FromIndex,
        //            ToIndex = 10,
        //            DateCreated = DateTime.UtcNow,
        //            DateUpdated = DateTime.UtcNow,
        //            Description = cd.Description,
        //            State = cd.State,
        //            Status = true
        //        };

        //        newCampaign.CampaignDetails.Add(campaignDetail);

        //        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        //        if (!isSuccess)
        //        {
        //            throw new ApiException("Create CampaignDetail Fail", 400, "BAD_REQUEST");
        //        }

        //        // Cập nhật danh sách VoucherItem
        //        var voucherItem = await _voucherItemService.GenerateVoucherItemsAsync(new VoucherItemRequest
        //        {
        //            VoucherId = cd.VoucherId,
        //            CampaignDetailId = campaignDetail.Id,
        //            Quantity = (int)cd.Quantity,
        //            ValidOn = campaignModel.StartOn,
        //            ExpireOn = campaignModel.EndOn
        //        });

        //        if (!voucherItem)
        //        {
        //            throw new ApiException("Generate VoucherItem Fail", 400, "BAD_REQUEST");
        //        }
        //    }

        //    return new CampaignResponse
        //    {
        //        Id = newCampaign.Id,
        //        BrandId = newCampaign.BrandId,
        //        BrandAcronym = brandExists.Acronym,
        //        BrandName = brandExists.BrandName,
        //        TypeId = newCampaign.TypeId,
        //        TypeName = typeExists.TypeName,
        //        CampaignName = newCampaign.CampaignName,
        //        Image = newCampaign.Image,
        //        ImageName = newCampaign.ImageName,
        //        File = newCampaign.File,
        //        FileName = newCampaign.FileName,
        //        Condition = newCampaign.Condition,
        //        Link = newCampaign.Link,
        //        StartOn = newCampaign.StartOn,
        //        EndOn = newCampaign.EndOn,
        //        Duration = newCampaign.Duration,
        //        TotalIncome = newCampaign.TotalIncome,
        //        TotalSpending = newCampaign.TotalSpending,
        //        DateCreated = newCampaign.DateCreated,
        //        DateUpdated = newCampaign.DateUpdated,
        //        Description = newCampaign.Description,
        //        Status = newCampaign.Status
        //    };

        //    throw new ApiException("Create Campaign Fail", 400, "BAD_REQUEST");
        //}


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
            var area = await _unitOfWork.GetRepository<Campaign>().SingleOrDefaultAsync(
                selector: x => new CampaignResponseExtra
                {
                    Id = x.Id,
                    BrandId = x.BrandId,
                    TypeId = x.TypeId,
                    CampaignName = x.CampaignName,
                    Image = x.Image,
                    ImageName = x.ImageName,
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
                include: x => x.Include(x => x.CampaignDetails));
            ;
            return area;
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
                    Status = x.Status
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

        public async Task<IPaginate<CampaignStore>> GetStoresByCampaignId(string campaignId, string searchName, int page, int size)
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
                selector: x => new CampaignStore
                {
                    Id = x.Id,
                    CampaignId = x.CampaignId,
                    StoreId = x.StoreId,
                    Description = x.Description,
                    Status = x.Status,
                    Store = x.Store,
                    Campaign = x.Campaign

                },
                predicate: filterQuery,
                include: x => x.Include(a => a.Store).Include(b => b.Campaign),
                page: page,
                size: size);

            return stores;
        }

        public async Task<IPaginate<CampaignResponse>> GetCampaigns(string? searchName, int page, int size)
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
                    Status = x.Status
                },
                predicate: filterQuery,
                page: page,
                include: query => query.Include(x => x.Brand).Include(x => x.Type),
                size: size);
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
                    Status = x.Status
                },
                predicate: filterQuery,
                page: page,
                include: query => query.Include(x => x.Brand).Include(x => x.Type),
                size: size);
            return campaigns;
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