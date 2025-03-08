using AutoMapper;
using CloudinaryDotNet.Core;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Campaign;
using SWallet.Repository.Payload.Response.Campaign;
using SWallet.Repository.Payload.Response.Voucher;
using SWallet.Repository.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Implements
{
    public class CampaignService : BaseService<CampaignService>, ICampaignService
    {
        private readonly Mapper mapper;
        private readonly ICloudinaryService _cloudinaryService;

        public record ItemIndex
        {
            public int? FromIndex { get; set; }
            public int? ToIndex { get; set; }
        }

        public CampaignService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<CampaignService> logger, ICloudinaryService cloudinaryService) : base(unitOfWork, logger)
        {
            _cloudinaryService = cloudinaryService;

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

        public async Task<CampaignResponse> CreateCampaign(CreateCampaignModel campaignModel)
        {
            // Upload hình ảnh
            var imageUri = string.Empty;
            if (campaignModel.Image != null && campaignModel.Image.Length > 0)
            {
                var uploadResult = await _cloudinaryService.UploadImageAsync(campaignModel.Image);
                imageUri = uploadResult.SecureUrl.AbsoluteUri;
            }

            // Sử dụng Mapper để tạo mới Campaign từ CreateCampaignModel
            var newCampaign = mapper.Map<Campaign>(campaignModel);
            newCampaign.Link = campaignModel.Link;
            newCampaign.File = "abc"; 
            newCampaign.FileName = "abc";
            newCampaign.Image = imageUri;
            newCampaign.ImageName = !string.IsNullOrEmpty(imageUri)
                ? imageUri.Split('/')[^1]
                : "default_cover.jpg";

            // Xử lý CampaignDetails riêng
            newCampaign.CampaignDetails = campaignModel.CampaignDetails.Select(d =>
            {
                var voucher = GetVoucherByIdAsync(d.VoucherId).Result; // Lấy thông tin voucher
                var detail = mapper.Map<CampaignDetail>(d);
                detail.CampaignId = newCampaign.Id;
                detail.Price = voucher.Price;
                detail.Rate = voucher.Rate;
                var index = GetIndexAsync(d.VoucherId, (int)d.Quantity, (int)d.FromIndex).Result;
                detail.FromIndex = index.FromIndex;
                detail.ToIndex = index.ToIndex;
                return detail;
            }).ToList();

            // Lưu Campaign vào cơ sở dữ liệu
            await _unitOfWork.GetRepository<Campaign>().InsertAsync(newCampaign);

            foreach (var detail in newCampaign.CampaignDetails)
            {
                await _unitOfWork.GetRepository<CampaignDetail>().InsertAsync(detail);
            }

            _logger.LogInformation("CampaignDetails after saving: {@CampaignDetails}", newCampaign.CampaignDetails);

            // Cập nhật VoucherItems
            foreach (var d in newCampaign.CampaignDetails)
            {
                await UpdateListAsync(
                    d.VoucherId,
                    d.Id,
                    (int)d.Quantity,
                    (DateOnly)newCampaign.StartOn,
                    (DateOnly)newCampaign.EndOn,
                    new ItemIndex
                    {
                        FromIndex = d.FromIndex,
                        ToIndex = d.ToIndex
                    });
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            var isSuccess = await _unitOfWork.CommitAsync() > 0;

            if (isSuccess)
            {
                return mapper.Map<CampaignResponse>(newCampaign);
            }
            throw new ApiException("Create Campaign Fail", 400, "BAD_REQUEST");
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

        public Task<CampaignResponse> GetCampaignById(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IPaginate<CampaignResponse>> GetCampaigns(string searchName, int page, int size)
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

            var areas = await _unitOfWork.GetRepository<Campaign>().GetPagingListAsync(
                selector: x => new CampaignResponse
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
                    TotalIncome = x.TotalIncome,
                    TotalSpending = x.TotalSpending,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    Status = x.Status
                },
                predicate: filterQuery,
                page: page,
                size: size);
            return areas;
        }

        public Task<CampaignResponse> UpdateCampaign(string id, UpdateCampaignModel campaign)
        {
            throw new NotImplementedException();
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
    }
}