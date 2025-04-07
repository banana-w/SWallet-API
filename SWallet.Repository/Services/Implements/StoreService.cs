using AutoMapper;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using Microsoft.EntityFrameworkCore;
using SWallet.Domain.Paginate;
using SWallet.Repository.Enums;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Request.Store;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Payload.Response.Store;
using SWallet.Repository.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;
using BCryptNet = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;

namespace SWallet.Repository.Services.Implements
{
    public class StoreService : BaseService<StoreService>, IStoreService
    {
        private readonly Mapper mapper;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly SwalletDbContext _swalletDB;

        public StoreService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<StoreService> logger, ICloudinaryService cloudinaryService, IHttpContextAccessor httpContextAccessor, SwalletDbContext swalletDB) : base(unitOfWork, logger, httpContextAccessor)
        {
            _cloudinaryService = cloudinaryService;
            var config = new MapperConfiguration(cfg
                =>
            {
                cfg.CreateMap<Store, StoreResponse>()
            .ForMember(s => s.BrandName, opt => opt.MapFrom(src => src.Brand.BrandName))
            .ForMember(s => s.BrandLogo, opt => opt.MapFrom(src => src.Brand.Account.Avatar))
            .ForMember(s => s.AreaName, opt => opt.MapFrom(src => src.Area.AreaName))
            .ForMember(s => s.AreaImage, opt => opt.MapFrom(src => src.Area.Image))
            .ForMember(s => s.UserName, opt => opt.MapFrom(src => src.Account.UserName))
            .ForMember(s => s.Avatar, opt => opt.MapFrom(src => src.Account.Avatar))
            .ForMember(s => s.AvatarFileName, opt => opt.MapFrom(src => src.Account.FileName))
            .ForMember(s => s.Email, opt => opt.MapFrom(src => src.Account.Email))
            .ForMember(s => s.Phone, opt => opt.MapFrom(src => src.Account.Phone))
            .ForMember(s => s.NumberOfCampaigns, opt => opt.MapFrom(src => src.CampaignStores.Count))
            .ForMember(s => s.NumberOfVouchers, opt => opt.MapFrom(src => src.Activities.Count))
            .ReverseMap();
                // Map Create Store Model
                cfg.CreateMap<Store, CreateStoreModel>()
                .ReverseMap()
                .ForMember(s => s.Id, opt => opt.MapFrom(src => Ulid.NewUlid()))
                .ForMember(s => s.DateCreated, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(s => s.DateUpdated, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(s => s.Status, opt => opt.MapFrom(src => true));
                cfg.CreateMap<Account, CreateStoreModel>()
                .ReverseMap()
                .ForMember(s => s.Id, opt => opt.MapFrom(src => Ulid.NewUlid()))
                .ForMember(s => s.Role, opt => opt.MapFrom(src => Role.Store))
                .ForMember(s => s.IsVerify, opt => opt.MapFrom(src => true))
                .ForMember(s => s.DateCreated, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(s => s.DateUpdated, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(s => s.DateVerified, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(s => s.Status, opt => opt.MapFrom(src => true));
                cfg.CreateMap<Store, UpdateStoreModel>()
                .ReverseMap()
                .ForMember(s => s.DateUpdated, opt => opt.MapFrom(src => DateTime.Now))
                .ForPath(s => s.Account.DateUpdated, opt => opt.MapFrom(src => DateTime.Now))
                .ForPath(s => s.Account.Description, opt => opt.MapFrom(src => src.Description))
                .ForPath(s => s.Account.State, opt => opt.MapFrom(src => src.State));

            });
            mapper = new Mapper(config);
            _swalletDB = swalletDB;
        }

        public async Task<long> CountParticipantToday(string storeId, DateOnly date)
        {
            if (string.IsNullOrEmpty(storeId))
            {
                throw new ArgumentException("StoreId không được để trống", nameof(storeId));
            }
            if (date == default)
            {
                throw new ArgumentException("Ngày không được để trống", nameof(date));
            }

            try
            {
                return await _swalletDB.Activities
                    .Where(c => (bool)c.Status
                                && c.StoreId.Equals(storeId)
                                && c.DateCreated.HasValue
                                && DateOnly.FromDateTime(c.DateCreated.Value).Equals(date))
                    .Select(a => a.StudentId)
                    .Distinct()
                    .CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đếm số lượng người tham gia tại cửa hàng {StoreId} vào ngày {Date}", storeId, date);
                throw new Exception($"Lỗi khi đếm số lượng người tham gia: {ex.Message}", ex);
            }
        }


        public async Task<StoreResponse> CreateStore(string accountId, CreateStoreModel store)
        {
            var existingAccount = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: b => b.Id == accountId);

            if (existingAccount == null)
            {
                throw new ApiException("Account not found", 404, "NOT_FOUND");
            }

            var existingBrand = await _unitOfWork.GetRepository<Brand>().SingleOrDefaultAsync(
                predicate: b => b.Id == store.BrandId);

            if (existingBrand == null)
            {
                throw new ApiException("Brand not found", 404, "NOT_FOUND");
            }

            var existingArea = await _unitOfWork.GetRepository<Area>().SingleOrDefaultAsync(
               predicate: b => b.Id == store.AreaId );

            if (existingArea == null)
            {
                throw new ApiException("Area not found", 404, "NOT_FOUND");
            }

            var imageUri = string.Empty;
            if (store.Avatar != null && store.Avatar.Length > 0)
            {
                var uploadResult = await _cloudinaryService.UploadImageAsync(store.Avatar);
                imageUri = uploadResult.SecureUrl.AbsoluteUri;
            }

            var newStore = new Store
            {
                Id = Ulid.NewUlid().ToString(),
                BrandId = store.BrandId,
                AreaId = store.AreaId,
                AccountId = accountId,
                StoreName = store.StoreName,
                Address = store.Address,
                OpeningHours = store.OpeningHours,    
                ClosingHours = store.ClosingHours,
                Description = store.Description,
                State = store.State,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                File = imageUri,
                FileName = !string.IsNullOrEmpty(imageUri)
                          ? imageUri.Split('/')[imageUri.Split('/').Length - 1]
                          : "default_cover.jpg"
            };
            await _unitOfWork.GetRepository<Store>().InsertAsync(newStore);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;

            if (isSuccess)
            {
                return new StoreResponse
                {
                    Id = newStore.Id,
                    BrandId = newStore.BrandId,
                    AreaId = newStore.AreaId,
                    AccountId = newStore.AccountId,
                    StoreName = newStore.StoreName,
                    Address = newStore.Address,
                    OpeningHours = newStore.OpeningHours,
                    ClosingHours = newStore.ClosingHours,
                    Description = newStore.Description,
                    State = newStore.State,
                    DateCreated = newStore.DateCreated,
                    DateUpdated = newStore.DateUpdated,
                    BrandName = existingBrand.BrandName,
                    BrandLogo = existingBrand.CoverPhoto,
                    AreaName = existingArea.AreaName,
                    AreaImage = existingArea.Image,
                    UserName = existingAccount.UserName,
                    Email = existingAccount.Email,
                    Phone = existingAccount.Phone,
                    Avatar = existingAccount.Avatar,
                    AvatarFileName = existingAccount.FileName,
                    Status = newStore.Status

                };
            }
            throw new ApiException("Create Store Fail", 400, "BAD_REQUEST");
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }


        public async Task<IPaginate<StoreResponse>> GetStoreByBrandId(string searchName, int page, int size)
        {
            var brandId = GetBrandIdFromJwt();
            Expression<Func<Store, bool>> filterQuery;

            if (string.IsNullOrEmpty(searchName))
            {
                filterQuery = p => p.BrandId == brandId;
            }
            else
            {
                filterQuery = p => p.BrandId == brandId && p.StoreName.Contains(searchName);
            }

            var stores = await _unitOfWork.GetRepository<Store>().GetPagingListAsync(
                selector: x => new StoreResponse
                {
                    Id = x.Id,
                    AccountId = x.AccountId,
                    BrandId = x.BrandId,
                    BrandName = x.Brand.BrandName,
                    AreaId = x.AreaId,
                    AreaName = x.Area.AreaName,
                    StoreName = x.StoreName,
                    Address = x.Address,
                    OpeningHours = x.OpeningHours,
                    ClosingHours = x.ClosingHours,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    State = x.State,
                    Status = x.Account.Status,
                    UserName = x.Account.UserName,
                    Email = x.Account.Email,
                    Phone = x.Account.Phone,
                    Avatar = x.Account.Avatar,
                    AvatarFileName = x.Account.FileName,
                    NumberOfCampaigns = x.CampaignStores.Count,
                    NumberOfVouchers = x.Activities.Count,

                },
                predicate: filterQuery,
                include: x => x.Include(a => a.Account).Include(b => b.Brand).Include(c => c.Area),
                page: page,
                size: size);

            return stores;
        }
        public async Task<IPaginate<StoreResponse>> GetStoreInBrand(string searchName, int page, int size)
        {
            var brandId = GetBrandIdFromJwt();
            Expression<Func<Store, bool>> filterQuery;

            if (string.IsNullOrEmpty(searchName))
            {
                filterQuery = p => p.BrandId == brandId;
            }
            else
            {
                filterQuery = p => p.BrandId == brandId && p.StoreName.Contains(searchName);
            }

            var stores = await _unitOfWork.GetRepository<Store>().GetPagingListAsync(
                selector: x => new StoreResponse
                {
                    Id = x.Id,
                    AccountId = x.AccountId,
                    BrandId = x.BrandId,
                    BrandName = x.Brand.BrandName,
                    AreaId = x.AreaId,
                    AreaName = x.Area.AreaName,
                    StoreName = x.StoreName,
                    Address = x.Address,
                    OpeningHours = x.OpeningHours,
                    ClosingHours = x.ClosingHours,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    State = x.State,
                    Status = x.Account.Status,
                    UserName = x.Account.UserName,
                    Email = x.Account.Email,
                    Phone = x.Account.Phone,
                    Avatar = x.Account.Avatar,
                    AvatarFileName = x.Account.FileName,
                    NumberOfCampaigns = x.CampaignStores.Count,
                    NumberOfVouchers = x.Activities.Count,
                },
                predicate: filterQuery,
                include: x => x.Include(a => a.Account).Include(b => b.Brand).Include(c => c.Area),
                page: page,
                size: size);

            return stores;
        }
        
        public async Task<IPaginate<StoreResponse>> GetStoreByBrandId(string brandId, string searchName, int page, int size)
        {
            Expression<Func<Store, bool>> filterQuery;

            if (string.IsNullOrEmpty(searchName))
            {
                filterQuery = p => p.BrandId == brandId;
            }
            else
            {
                filterQuery = p => p.BrandId == brandId && p.StoreName.Contains(searchName);
            }

            var stores = await _unitOfWork.GetRepository<Store>().GetPagingListAsync(
                selector: x => new StoreResponse
                {
                    Id = x.Id,
                    AccountId = x.AccountId,
                    BrandId = x.BrandId,
                    BrandName = x.Brand.BrandName,
                    AreaId = x.AreaId,
                    AreaName = x.Area.AreaName,
                    StoreName = x.StoreName,
                    Address = x.Address,
                    OpeningHours = x.OpeningHours,
                    ClosingHours = x.ClosingHours,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    State = x.State,
                    Status = x.Account.Status,
                    UserName = x.Account.UserName,
                    Email = x.Account.Email,
                    Phone = x.Account.Phone,
                    Avatar = x.Account.Avatar,
                    AvatarFileName = x.Account.FileName,
                    NumberOfCampaigns = x.CampaignStores.Count,
                    NumberOfVouchers = x.Activities.Count,
                },
                predicate: filterQuery,
                include: x => x.Include(a => a.Account).Include(b => b.Brand).Include(c => c.Area),
                page: page,
                size: size);

            return stores;
        }

        public async Task<StoreResponse> GetStoreById(string id)
        {
            var area = await _unitOfWork.GetRepository<Store>().SingleOrDefaultAsync(
               selector: x => new StoreResponse
               {
                   Id = x.Id,
                   AccountId = x.AccountId,
                   BrandId = x.BrandId,
                   BrandName = x.Brand.BrandName,
                   AreaId = x.AreaId,
                   AreaName = x.Area.AreaName,
                   StoreName = x.StoreName,
                   Address = x.Address,
                   OpeningHours = x.OpeningHours,
                   ClosingHours = x.ClosingHours,
                   DateCreated = x.DateCreated,
                   DateUpdated = x.DateUpdated,
                   Description = x.Description,
                   State = x.State,
                   Status = x.Account.Status,
                   UserName = x.Account.UserName,
                   Email = x.Account.Email,
                   Phone = x.Account.Phone,
                   Avatar = x.Account.Avatar,
                   AvatarFileName = x.Account.FileName,
                   NumberOfCampaigns = x.CampaignStores.Count,
                   NumberOfVouchers = x.Activities.Count,
               },
               predicate: x => x.Id == id);
            return area;
        }

        public async Task<IPaginate<StoreResponse>> GetStores(string searchName, int page, int size)
        {
            Expression<Func<Store, bool>> filterQuery;
            if (string.IsNullOrEmpty(searchName))
            {
                filterQuery = p => true;
            }
            else
            {
                filterQuery = p => p.StoreName.Contains(searchName);
            }

            var areas = await _unitOfWork.GetRepository<Store>().GetPagingListAsync(
                selector: x => new StoreResponse
                {
                    Id = x.Id,
                    AccountId = x.AccountId,
                    BrandId = x.BrandId,
                    BrandName = x.Brand.BrandName,
                    AreaId = x.AreaId,
                    AreaName = x.Area.AreaName,
                    StoreName = x.StoreName,
                    Address = x.Address,
                    OpeningHours = x.OpeningHours,
                    ClosingHours = x.ClosingHours,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    State = x.State,
                    Status = x.Account.Status,
                    UserName = x.Account.UserName,
                    Email = x.Account.Email,
                    Phone = x.Account.Phone,
                    Avatar = x.Account.Avatar,
                    AvatarFileName = x.Account.FileName,
                    NumberOfCampaigns = x.CampaignStores.Count,
                    NumberOfVouchers = x.Activities.Count,




                },
                predicate: filterQuery,
                include: x => x.Include(a => a.Account),
                page: page,
                size: size);
            return areas;
        }

        public async Task<StoreResponse> UpdateStore(string id, UpdateStoreModel store)
        {
            var updateStore = await _unitOfWork.GetRepository<Store>().SingleOrDefaultAsync(
                                                                                predicate: x => x.Id == id,
                                                                                include: x => x
                                                                                    .Include(a => a.Brand)
                                                                                    .Include(a => a.Area)
                                                                                    .Include(a => a.Account)
);


            if (updateStore == null)
            {
                throw new ApiException("Store not found", 404, "NOT_FOUND");
            }
            if (store.Avatar != null && store.Avatar.Length > 0)
            {

                var f = await _cloudinaryService.UploadImageAsync(store.Avatar);

            }
            updateStore.AreaId = store.AreaId;
            updateStore.StoreName = store.StoreName;
            updateStore.Address = store.Address;
            updateStore.OpeningHours = store.OpeningHours;
            updateStore.ClosingHours = store.ClosingHours;
            updateStore.Description = store.Description;
            updateStore.State = store.State;
            updateStore.DateUpdated = DateTime.Now;
            _unitOfWork.GetRepository<Store>().UpdateAsync(updateStore);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;

            if (isSuccess)
            {
                return new StoreResponse
                {
                    Id = updateStore.Id,
                    BrandId = updateStore.BrandId,
                    AreaId = updateStore.AreaId,
                    AccountId = updateStore.AccountId,
                    StoreName = updateStore.StoreName,
                    Address = updateStore.Address,
                    OpeningHours = updateStore.OpeningHours,
                    ClosingHours = updateStore.ClosingHours,
                    Description = updateStore.Description,
                    State = updateStore.State,
                    DateCreated = updateStore.DateCreated,
                    DateUpdated = updateStore.DateUpdated,
                    BrandName = updateStore.Brand.BrandName,
                    BrandLogo = updateStore.Brand.CoverPhoto,
                    AreaName = updateStore.Area.AreaName,
                    AreaImage = updateStore.Area.Image,
                    UserName = updateStore.Account.UserName,
                    Email = updateStore.Account.Email,
                    Phone = updateStore.Account.Phone,
                    Avatar = updateStore.Account.Avatar,
                    AvatarFileName = updateStore.Account.FileName,
                    Status = updateStore.Status

                };
            }
            throw new ApiException("Update Store Fail", 400, "BAD_REQUEST");
        }

        public async Task<StoreResponse> GetStoreByAccountId(string id)
        {
            var area = await _unitOfWork.GetRepository<Store>().SingleOrDefaultAsync(
                selector: x => new StoreResponse
                {
                    Id = x.Id,
                    AccountId = x.AccountId,
                    BrandLogo = x.Brand.CoverPhoto,
                    AreaImage = x.Area.Image,
                    Avatar = x.Account.Avatar,
                    AvatarFileName = x.Account.FileName,
                    File = x.File,
                    FileName = x.FileName,
                    BrandId = x.BrandId,
                    BrandName = x.Brand.BrandName,
                    AreaId = x.AreaId,
                    AreaName = x.Area.AreaName,
                    StoreName = x.StoreName,
                    Address = x.Address,
                    OpeningHours = x.OpeningHours,
                    ClosingHours = x.ClosingHours,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    State = x.State,
                    Status = x.Account.Status,
                    UserName = x.Account.UserName,
                    Email = x.Account.Email,
                    Phone = x.Account.Phone,
                    NumberOfCampaigns = x.CampaignStores.Count,
                    NumberOfVouchers = x.Activities.Count,

                },
                predicate: x => x.AccountId == id,
                include: x => x.Include(a => a.Account)
                              .Include(a => a.Brand)
                              .Include(a => a.Area));
            return area;
        }

        public async Task<IPaginate<StoreResponse>> GetStoresInCampaign(string campaignId, string searchName, int page, int size)
        {
            Expression<Func<Store, bool>> filterQuery;

            if (string.IsNullOrEmpty(searchName))
            {
                filterQuery = p => p.CampaignStores.Any(cs => cs.CampaignId  == campaignId);
            }
            else
            {
                filterQuery = p => p.CampaignStores.Any(cs => cs.CampaignId == campaignId) && p.StoreName.Contains(searchName);
            }
            var stores = await _unitOfWork.GetRepository<Store>().GetPagingListAsync(
                selector: x => new StoreResponse
                {
                    Id = x.Id,
                    AccountId = x.AccountId,
                    BrandId = x.BrandId,
                    BrandName = x.Brand.BrandName,
                    AreaId = x.AreaId,
                    AreaName = x.Area.AreaName,
                    StoreName = x.StoreName,
                    Address = x.Address,
                    OpeningHours = x.OpeningHours,
                    ClosingHours = x.ClosingHours,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    State = x.State,
                    Status = x.Account.Status,
                    UserName = x.Account.UserName,
                    Email = x.Account.Email,
                    Phone = x.Account.Phone,
                    Avatar = x.Account.Avatar,
                    AvatarFileName = x.Account.FileName,

                },
                predicate: filterQuery,
                include: x => x.Include(a => a.Account).Include(b => b.Brand).Include(c => c.Area),
                page: page,
                size: size);
            if (stores == null)
            {
                throw new ApiException("Store not found", 404, "NOT_FOUND");
            }
            return stores;
        }
    }
}
