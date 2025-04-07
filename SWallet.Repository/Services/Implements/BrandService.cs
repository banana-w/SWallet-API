using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Enums;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Area;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Response.Area;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Services.Interfaces;
using System;
using System.IO.Enumeration;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BCryptNet = BCrypt.Net.BCrypt;

namespace SWallet.Repository.Services.Implements
{
    public class BrandService : BaseService<BrandService>, IBrandService
    {
        private readonly Mapper mapper;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly SwalletDbContext _swalletDB;
        public BrandService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<BrandService> logger, ICloudinaryService cloudinaryService, SwalletDbContext swalletDB) : base(unitOfWork, logger)
        {
            _cloudinaryService = cloudinaryService;
            var config = new MapperConfiguration(cfg
                =>
            {
                cfg.CreateMap<Account, CreateBrandModel>()
            .ReverseMap()
            .ForMember(p => p.Id, opt => opt.MapFrom(src => Ulid.NewUlid()))
            .ForMember(p => p.Role, opt => opt.MapFrom(src => Role.Brand))
            .ForMember(p => p.Password, opt => opt.MapFrom(src => BCryptNet.HashPassword(src.Password)))
            .ForMember(p => p.IsVerify, opt => opt.MapFrom(src => true))
            .ForMember(p => p.DateCreated, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(p => p.DateUpdated, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(p => p.DateVerified, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(p => p.Status, opt => opt.MapFrom(src => true));
            });
            mapper = new Mapper(config);
            _swalletDB = swalletDB;
        }

        public async Task<long> CountVoucherItemToday(string brandId, DateOnly date)
        {
            if (string.IsNullOrEmpty(brandId))
            {
                throw new ArgumentException("BrandId không được để trống", nameof(brandId));
            }
            if (date == default)
            {
                throw new ArgumentException("Ngày không được để trống", nameof(date));
            }

            try
            {
                return await _swalletDB.VoucherItems
                    .Where(vi => vi.Voucher.BrandId == brandId
                                && (bool)vi.Status
                                && vi.DateCreated.HasValue
                                && DateOnly.FromDateTime(vi.DateCreated.Value).Equals(date))
                    .CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đếm số lượng voucher item của thương hiệu {BrandId} vào ngày {Date}", brandId, date);
                throw new Exception($"Lỗi khi đếm số lượng voucher item: {ex.Message}", ex);
            }
        }

        public async Task<BrandResponse> CreateBrand(CreateBrandModel brand)
            {
                var existingAccount = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: b => b.UserName == brand.UserName);

                if (existingAccount != null)
                {
                    throw new ApiException("Username already exists", 400, "BAD_REQUEST");
                }
            var imageUri = string.Empty;
            
            if (brand.CoverPhoto != null && brand.CoverPhoto.Length > 0)
            {
                var uploadResult = await _cloudinaryService.UploadImageAsync(brand.CoverPhoto);
                imageUri = uploadResult.SecureUrl.AbsoluteUri;
            }

            Account account = mapper.Map<Account>(brand);
            account.Avatar = imageUri;
            await _unitOfWork.GetRepository<Account>().InsertAsync(account);
            await _unitOfWork.CommitAsync();

            
   
            var newBrand = new Brand
            {
                Id = Ulid.NewUlid().ToString(),
                BrandName = brand.BrandName,
                Acronym = brand.Acronym,
                Address = brand.Address,
                CoverPhoto = imageUri,
                CoverFileName = !string.IsNullOrEmpty(imageUri)
                          ? imageUri.Split('/')[imageUri.Split('/').Length - 1]
                          : "default_cover.jpg",
                Link = brand.Link,
                OpeningHours = brand.OpeningHours,
                ClosingHours = brand.ClosingHours,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Description = brand.Description,
                State = brand.State,
                AccountId = account.Id,
                Status = account.Status


            };

            await _unitOfWork.GetRepository<Brand>().InsertAsync(newBrand);
                var isSuccess = await _unitOfWork.CommitAsync() > 0;

                if (isSuccess)
                {
                    return new BrandResponse
                    {
                        Id = newBrand.Id,
                        AccountId = newBrand.AccountId,
                        BrandName = newBrand.BrandName,
                        Acronym = newBrand.Acronym,
                        Address = newBrand.Address,
                        CoverPhoto = newBrand.CoverPhoto,
                        CoverFileName = newBrand.CoverFileName,
                        Link = newBrand.Link,
                        OpeningHours = newBrand.OpeningHours,
                        ClosingHours = newBrand.ClosingHours,
                        DateCreated = newBrand.DateCreated,
                        Description = newBrand.Description,
                        State = newBrand.State,
                        Status = newBrand.Status

                    };
                }
                throw new ApiException("Create Brand Fail", 400, "BAD_REQUEST");
            }

        public async Task<BrandResponse> CreateBrandAsync(string accountId, CreateBrandByAccountId brand)
        {
            var imageUri = string.Empty;
            if (brand.CoverPhoto != null && brand.CoverPhoto.Length > 0)
            {
                var uploadResult = await _cloudinaryService.UploadImageAsync(brand.CoverPhoto);
                imageUri = uploadResult.SecureUrl.AbsoluteUri;
            }

            var newBrand = new Brand
            {
                Id = Ulid.NewUlid().ToString(),
                BrandName = brand.BrandName,
                Acronym = brand.Acronym,
                Address = brand.Address,
                CoverPhoto = imageUri,
                CoverFileName = !string.IsNullOrEmpty(imageUri)
                    ? imageUri.Split('/')[imageUri.Split('/').Length - 1]
                    : "default_cover.jpg",
                Link = brand.Link,
                OpeningHours = brand.OpeningHours,
                ClosingHours = brand.ClosingHours,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Description = brand.Description,
                State = brand.State,
                AccountId = accountId, // Use the provided accountId
                Status = true // Inherit status from the existing account
            };

            await _unitOfWork.GetRepository<Brand>().InsertAsync(newBrand);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;

            if (isSuccess)
            {
                return new BrandResponse
                {
                    Id = newBrand.Id,
                    AccountId = newBrand.AccountId,
                    BrandName = newBrand.BrandName,
                    Acronym = newBrand.Acronym,
                    Address = newBrand.Address,
                    CoverPhoto = newBrand.CoverPhoto,
                    CoverFileName = newBrand.CoverFileName,
                    Link = newBrand.Link,
                    OpeningHours = newBrand.OpeningHours,
                    ClosingHours = newBrand.ClosingHours,
                    DateCreated = newBrand.DateCreated,
                    Description = newBrand.Description,
                    State = newBrand.State,
                    Status = newBrand.Status
                };
            }

            throw new ApiException("Create Brand Fail", 400, "BAD_REQUEST");
        }

        public void Delete(string id)
        {
            
        }

        public async Task<BrandResponse> GetBrandById(string id)
        {
            var area = await _unitOfWork.GetRepository<Brand>().SingleOrDefaultAsync(
                selector: x => new BrandResponse
                {
                    Id = x.Id,
                    AccountId = x.AccountId,
                    Email = x.Account.Email,
                    PhoneNumber = x.Account.Phone,
                    Username = x.Account.UserName,
                    BrandName = x.BrandName,
                    Acronym = x.Acronym,
                    Address = x.Address,
                    CoverPhoto = x.CoverPhoto,
                    CoverFileName = x.CoverFileName,
                    Link = x.Link,
                    OpeningHours = x.OpeningHours,
                    ClosingHours = x.ClosingHours,
                    TotalIncome = x.TotalIncome,
                    TotalSpending = x.TotalSpending,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    State = x.State,
                    Status = x.Status,
                    NumberOfCampaigns = x.Campaigns.Where(c => c.BrandId == id).Count()
                },
                predicate: x => x.Id == id,
                include: x => x.Include(a => a.Account).Include(a => a.Campaigns));
            return area;
        }

        public async Task<IPaginate<BrandResponse>> GetBrands(string? searchName, int page, int size, bool status)
        {
            Expression<Func<Brand, bool>> filterQuery;

            // Kết hợp điều kiện lọc theo searchName và status
            if (string.IsNullOrEmpty(searchName))
            {
                // Nếu searchName rỗng, chỉ lọc theo status
                filterQuery = p => p.Status == status;
            }
            else
            {
                // Nếu searchName không rỗng, lọc theo cả searchName và status
                filterQuery = p => p.BrandName.Contains(searchName) && p.Status == status;
            }

            var areas = await _unitOfWork.GetRepository<Brand>().GetPagingListAsync(
                selector: x => new BrandResponse
                {
                    Id = x.Id,
                    AccountId = x.AccountId,
                    BrandName = x.BrandName,
                    Acronym = x.Acronym,
                    Address = x.Address,
                    CoverPhoto = x.CoverPhoto,
                    CoverFileName = x.CoverFileName,
                    Link = x.Link,
                    OpeningHours = x.OpeningHours,
                    ClosingHours = x.ClosingHours,
                    TotalIncome = x.TotalIncome,
                    TotalSpending = x.TotalSpending,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    State = x.State,
                    Status = x.Status,
                    NumberOfCampaigns = x.Campaigns.Count
                },
                predicate: filterQuery,
                page: page,
                size: size
            );

            return areas;
        }

        public async Task<List<Brand>> GetRanking(int limit)
        {
            if (limit <= 0)
            {
                throw new ArgumentException("Giới hạn phải lớn hơn 0", nameof(limit));
            }

            try
            {
                _logger.LogInformation("Bắt đầu lấy danh sách xếp hạng thương hiệu với giới hạn {Limit}", limit);

                var result = await _swalletDB.Brands
                    .Where(b => (bool)b.Status)
                    .OrderByDescending(b => b.TotalSpending)
                    .Take(limit)
                    .Include(b => b.Account)
                    .ToListAsync();

                _logger.LogInformation("Lấy thành công {Count} thương hiệu", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách xếp hạng thương hiệu với giới hạn {Limit}", limit);
                throw new Exception($"Lỗi khi lấy danh sách xếp hạng thương hiệu: {ex.Message}", ex);
            }
        }

        public long CountBrand()
        {
            long count = 0;
            try
            {
                var db = _swalletDB;
                count = db.Brands.Where(c => (bool)c.Status).Count();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return count;
        }

        public async Task<BrandResponse> UpdateBrand(string id, UpdateBrandModel brand)
        {
            var updateBrand = await _unitOfWork.GetRepository<Brand>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (updateBrand == null)
            {
                throw new ApiException("Brand not found", 404, "NOT_FOUND");
            }
            if (brand.CoverPhoto != null && brand.CoverPhoto.Length > 0)
            {

                var f = await _cloudinaryService.UploadImageAsync(brand.CoverPhoto);
               
            }
            updateBrand.BrandName = brand.BrandName;
            updateBrand.Acronym = brand.Acronym;    
            updateBrand.Address = brand.Address;
            updateBrand.Link = brand.Link;
            updateBrand.OpeningHours = brand.OpeningHours;
            updateBrand.ClosingHours = brand.ClosingHours;
            updateBrand.Description = brand.Description;
            updateBrand.State = brand.State;    
            updateBrand.DateUpdated = DateTime.Now;
            _unitOfWork.GetRepository<Brand>().UpdateAsync(updateBrand);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;
            if (isSuccess)
            {
                return new BrandResponse
                {
                    Id = updateBrand.Id,
                    AccountId = updateBrand.AccountId,
                    BrandName = updateBrand.BrandName,
                    Acronym = updateBrand.Acronym,
                    Address = updateBrand.Address,
                    CoverPhoto = updateBrand.CoverPhoto,
                    CoverFileName = updateBrand.CoverFileName,
                    Link = updateBrand.Link,
                    OpeningHours = updateBrand.OpeningHours,
                    ClosingHours = updateBrand.ClosingHours,
                    DateCreated = updateBrand.DateCreated,
                    DateUpdated = updateBrand.DateUpdated,
                    Description = updateBrand.Description,
                    State = updateBrand.State,
                    Status = updateBrand.Status
                };
            }
            throw new ApiException("Update Brand Fail", 400, "BAD_REQUEST");
        }
    }
}