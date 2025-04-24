using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Security;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Payload.Response.Campaign;
using SWallet.Repository.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Implements
{
    public class CampaignDetailService : BaseService<CampaignDetailService>, ICampaignDetailService
    {
        private readonly Mapper mapper;
        private readonly ILogger logger;

        public CampaignDetailService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<CampaignDetailService> logger) : base(unitOfWork, logger)
        {
            var config = new MapperConfiguration(cfg
                =>
            {
                // Map Campaign Detail Model
                cfg.CreateMap<CampaignDetail, CampaignDetailResponse>()
                .ForMember(c => c.VoucherName, opt => opt.MapFrom(src => src.Voucher.VoucherName))
                .ForMember(c => c.VoucherImage, opt => opt.MapFrom(src => src.Voucher.Image))
                .ForMember(c => c.CampaignName, opt => opt.MapFrom(src => src.Campaign.CampaignName))
                .ForMember(c => c.QuantityInStock, opt => opt.MapFrom(
                    src => src.VoucherItems.Where(
                       v => (bool)v.IsLocked && !(bool)v.IsBought && !(bool)v.IsUsed).Count()))
                .ForMember(c => c.QuantityInBought, opt => opt.MapFrom(
                    src => src.VoucherItems.Where(
                       v => (bool)v.IsLocked && (bool)v.IsBought).Count()))
                .ForMember(c => c.QuantityInUsed, opt => opt.MapFrom(
                    src => src.VoucherItems.Where(
                       v => (bool)v.IsLocked && (bool)v.IsUsed).Count()))
                .ReverseMap();
             
                cfg.CreateMap<CampaignDetail, CampaignDetailExtra>()
                .ForMember(c => c.VoucherName, opt => opt.MapFrom(src => src.Voucher.VoucherName))
                .ForMember(c => c.VoucherImage, opt => opt.MapFrom(src => src.Voucher.Image))
                .ForMember(c => c.VoucherCondition, opt => opt.MapFrom(src => src.Voucher.Condition))
                .ForMember(c => c.VoucherDescription, opt => opt.MapFrom(src => src.Voucher.Description))
                .ForMember(c => c.TypeId, opt => opt.MapFrom(src => src.Voucher.TypeId))
                .ForMember(c => c.TypeName, opt => opt.MapFrom(src => src.Voucher.Type.TypeName))
                .ForMember(c => c.CampaignName, opt => opt.MapFrom(src => src.Campaign.CampaignName))
                .ForMember(c => c.QuantityInStock, opt => opt.MapFrom(
                    src => src.VoucherItems.Where(
                       v => (bool)v.IsLocked && !(bool)v.IsBought && !(bool)v.IsUsed).Count()))
                .ForMember(c => c.QuantityInBought, opt => opt.MapFrom(
                    src => src.VoucherItems.Where(
                       v => (bool)v.IsLocked && (bool)v.IsBought).Count()))
                .ForMember(c => c.QuantityInUsed, opt => opt.MapFrom(
                    src => src.VoucherItems.Where(
                       v => (bool)v.IsLocked && (bool)v.IsUsed).Count()))
                .ReverseMap();
            });
            mapper = new Mapper(config);
        }

        public async Task<IPaginate<CampaignDetailResponse>> GetAllCampaignDetailByStore(
    string storeId, string searchName, int page, int size)
        {
            // Kiểm tra tham số đầu vào
            if (string.IsNullOrEmpty(storeId))
            {
                throw new ArgumentException("StoreId không được để trống", nameof(storeId));
            }
            if (page <= 0)
            {
                throw new ArgumentException("Số trang phải lớn hơn 0", nameof(page));
            }
            if (size <= 0)
            {
                throw new ArgumentException("Kích thước trang phải lớn hơn 0", nameof(size));
            }

            // Bước 1: Lấy danh sách CampaignId liên quan đến StoreId từ CampaignStore
            var campaignStoreRepo = _unitOfWork.GetRepository<CampaignStore>();
            Expression<Func<CampaignStore, bool>> campaignStoreFilter = cs => cs.StoreId == storeId;
            var campaignStores = await campaignStoreRepo.GetListAsync(
                predicate: campaignStoreFilter,
                selector: cs => cs.CampaignId);

            if (!campaignStores.Any())
            {
                // Nếu không tìm thấy CampaignStore nào liên quan đến StoreId, trả về danh sách rỗng
                return new Paginate<CampaignDetailResponse>
                {
                    Items = new List<CampaignDetailResponse>(),
                    Page = page,
                    Size = size,
                    Total = 0,
                    TotalPages = 0
                };
            }

            // Bước 2: Lấy danh sách CampaignDetail dựa trên CampaignId
            var campaignDetailRepo = _unitOfWork.GetRepository<CampaignDetail>();
            Expression<Func<CampaignDetail, bool>> filterQuery;
            if (string.IsNullOrEmpty(searchName))
            {
                filterQuery = cd => campaignStores.Contains(cd.CampaignId);
            }
            else
            {
                filterQuery = cd => campaignStores.Contains(cd.CampaignId) &&
                                   (cd.Campaign.CampaignName.Contains(searchName) ||
                                    cd.Description.Contains(searchName));
            }

            var campaignDetails = await campaignDetailRepo.GetPagingListAsync(
                selector: x => new CampaignDetailResponse
                {
                    Id = x.Id,
                    VoucherId = x.VoucherId,
                    CampaignId = x.CampaignId,
                    Price = x.Price,
                    Rate = x.Rate,
                    Quantity = x.Quantity,
                    FromIndex = x.FromIndex,
                    ToIndex = x.ToIndex,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    State = x.State,
                    Status = x.Status,
                    VoucherName = x.Voucher.VoucherName,
                    VoucherImage = x.Voucher.Image,
                    CampaignName = x.Campaign.CampaignName,
                    QuantityInStock = x.VoucherItems.Count(v => v.IsLocked != true && v.IsBought != true && v.IsUsed != true),
                    QuantityInBought = x.VoucherItems.Count(v =>  v.IsBought == true),
                    QuantityInUsed = x.VoucherItems.Count(v => v.IsBought == true && v.IsUsed == true)
                },
                predicate: filterQuery,
                include: source => source
                    .Include(x => x.Campaign)
                    .Include(x => x.Voucher)
                    .Include(x => x.VoucherItems),
                page: page,
                size: size);

            return campaignDetails;
        }

        public List<string> GetAllVoucherItemByCampaignDetail(string id)
        {
            var campaignDetail = _unitOfWork.GetRepository<CampaignDetail>().SingleOrDefaultAsync(selector: x => new CampaignDetail
            {
                Id = x.Id,
                VoucherId = x.VoucherId,
                CampaignId = x.CampaignId,
                Price = x.Price,
                Rate = x.Rate,
                Quantity = x.Quantity,
                FromIndex = x.FromIndex,
                ToIndex = x.ToIndex,
                DateCreated = x.DateCreated,
                DateUpdated = x.DateUpdated,
                Description = x.Description,
                State = x.State,
                Status = x.Status
            
            },
                predicate: x => x.Id == id);
            if (campaignDetail == null)
            {
                return new List<string>(); // Or throw an exception
            }

            var result = campaignDetail.Result.VoucherItems.Select(x => x.Id).ToList();
            return result;
        }

        public async Task<CampaignDetailExtra> GetById(string id)
        {
            var area = await _unitOfWork.GetRepository<CampaignDetail>().SingleOrDefaultAsync(
                selector: x => new CampaignDetailExtra
                {
                    Id = x.Id,
                    VoucherId = x.VoucherId,
                    CampaignId = x.CampaignId,
                    Price = x.Price,
                    Rate = x.Rate,
                    Quantity = x.Quantity,
                    FromIndex = x.FromIndex,
                    ToIndex = x.ToIndex,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    State = x.State,
                    Status = x.Status,
                    VoucherName = x.Voucher.VoucherName,
                    VoucherImage = x.Voucher.Image,
                    CampaignName = x.Campaign.CampaignName,
                    QuantityInStock = x.VoucherItems.Where(
                                              v => (bool)v.IsLocked && !(bool)v.IsBought && !(bool)v.IsUsed).Count(),
                    QuantityInBought = x.VoucherItems.Where(v => (bool)v.IsLocked && (bool)v.IsBought).Count(),
                    QuantityInUsed = x.VoucherItems.Where(v => (bool)v.IsLocked && (bool)v.IsUsed).Count()

                },
                predicate: x => x.Id == id);
            return area;
        }

        public async Task<IPaginate<CampaignDetailResponse>> GetCampaignDetails(string searchName, int page, int size)
        {
            Expression<Func<CampaignDetail, bool>> filterQuery;
            if (string.IsNullOrEmpty(searchName))
            {
                filterQuery = p => true;
            }
            else
            {
                filterQuery = p => p.Id.Contains(searchName);
            }

            var areas = await _unitOfWork.GetRepository<CampaignDetail>().GetPagingListAsync(
                selector: x => new CampaignDetailResponse
                {
                    Id = x.Id,
                    VoucherId = x.VoucherId,
                    CampaignId = x.CampaignId,
                    Price = x.Price,
                    Rate = x.Rate,
                    Quantity = x.Quantity,
                    FromIndex = x.FromIndex,    
                    ToIndex = x.ToIndex,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    State = x.State,    
                    Status = x.Status,
                    VoucherName = x.Voucher.VoucherName,
                    VoucherImage = x.Voucher.Image,
                    CampaignName = x.Campaign.CampaignName,
                    QuantityInStock = x.VoucherItems.Where(
                                              v => (bool)v.IsLocked && !(bool)v.IsBought && !(bool)v.IsUsed).Count(),
                    QuantityInBought = x.VoucherItems.Where(v => (bool)v.IsLocked && (bool)v.IsBought).Count(),
                    QuantityInUsed = x.VoucherItems.Where(v => (bool)v.IsLocked && (bool)v.IsUsed).Count()
                },
                predicate: filterQuery,
                page: page,
                size: size);
            return areas;
        }
    }
}
