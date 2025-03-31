using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Voucher;
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
    public class VoucherService : BaseService<VoucherService>, IVoucherService
    {
        private readonly ICloudinaryService _cloudinaryService;
        public VoucherService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<VoucherService> logger, ICloudinaryService cloudinaryService) : base(unitOfWork, logger)
        {
            _cloudinaryService = cloudinaryService;
        }

        public async Task<bool> CreateVoucher(VoucherRequest request)
        {

            var voucher = new Voucher
            {
                Id = Ulid.NewUlid().ToString(),
                BrandId = request.BrandId,
                TypeId = request.TypeId,
                Price = request.Price,
                Rate = request.Rate,
                Condition = request.Condition,
                VoucherName = request.VoucherName,
                Description = request.Description,
                Image = "NULL",
                File = "Null",
                FileName = "Null",
                ImageName = "NUll",
                State = request.State,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Status = true
            };

            // Add voucher to the database
            await _unitOfWork.GetRepository<Voucher>().InsertAsync(voucher);

            if (request.Image != null && request.Image.Length > 0)
            {
                var image = await _cloudinaryService.UploadImageAsync(request.Image);
                if (image != null)
                {
                    voucher.Image = image.SecureUrl.AbsoluteUri;
                    voucher.ImageName = image.PublicId;
                }
            }
                var result = await _unitOfWork.CommitAsync();
            if (result > 0)
            {
                return true;
            }
            throw new ApiException("Create voucher fail", 400, "VOUCHER_FAIL");
        }

        public Task<bool> DeleteVoucher(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IPaginate<VoucherResponse>> GetAllVouchers(string? search, int page, int size)
        {
            Expression<Func<Voucher, bool>> filterQuery = x =>
                            x.Status == true &&
                            (string.IsNullOrEmpty(search) || // search null
                            x.VoucherName.Contains(search) ||
                            x.Type.TypeName.Contains(search) ||
                            x.Condition.Contains(search));


            var vouchers = await _unitOfWork.GetRepository<Voucher>().GetPagingListAsync(
                selector: x => new VoucherResponse
                {
                    Id = x.Id,
                    BrandId = x.BrandId,
                    BrandName = x.Brand.BrandName,
                    TypeId = x.TypeId,
                    TypeName = x.Type.TypeName,
                    VoucherName = x.VoucherName,
                    Price = x.Price,
                    Rate = x.Rate,
                    Condition = x.Condition,
                    Image = x.Image,
                    ImageName = x.ImageName,
                    File = x.File,
                    FileName = x.FileName,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    State = x.State,
                    Status = x.Status,
                    NumberOfItemsAvailable = null,
                    //x.VoucherItems.Where(i => !(bool)i.IsLocked && !(bool)i.IsBought && !(bool)i.IsUsed && i.CampaignDetailId.IsNullOrEmpty()).Count(),
                    NumberOfItems = x.VoucherItems.Count()
                },
                predicate: filterQuery,
                page: page,
                size: size
                );
            return vouchers;
        }

        public async Task<IEnumerable<VoucherResponse>> GetVouchersByCampaignId(string campaignId)
        {
            var vouchers = await _unitOfWork.GetRepository<Voucher>().GetListAsync(
                selector: x => new VoucherResponse
                {
                    Id = x.Id,
                    BrandId = x.BrandId,
                    BrandName = x.Brand.BrandName,
                    TypeId = x.TypeId,
                    TypeName = x.Type.TypeName,
                    VoucherName = x.VoucherName,
                    Price = x.Price,
                    Rate = x.Rate,
                    Condition = x.Condition,
                    Image = x.Image,
                    ImageName = x.ImageName,
                    File = x.File,
                    FileName = x.FileName,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    State = x.State,
                    Status = x.Status,
                    NumberOfItemsAvailable = x.VoucherItems != null
                                                            ? x.VoucherItems
                                                                .Where(i => !(bool)i.IsLocked && !(bool)i.IsBought && !(bool)i.IsUsed && i.CampaignDetail.CampaignId == campaignId)
                                                                .Count()
                                                            : 0,
                    NumberOfItems = x.VoucherItems.Where(i => i.CampaignDetail.CampaignId == campaignId).Count()
                },
                predicate: x => x.CampaignDetails.Any(c => c.CampaignId == campaignId),
                include: x => x.Include(x => x.CampaignDetails));
            return vouchers;
        }

        public async Task<VoucherResponse> GetVoucherById(string id)
        {
            var voucher = await _unitOfWork.GetRepository<Voucher>().SingleOrDefaultAsync(
                selector: v => new VoucherResponse
                {
                    Id = v.Id,
                    BrandId = v.BrandId,
                    BrandName = v.Brand.BrandName,
                    TypeId = v.TypeId,
                    TypeName = v.Type.TypeName,
                    VoucherName = v.VoucherName,
                    Price = v.Price,
                    Rate = v.Rate,
                    Condition = v.Condition,
                    Image = v.Image,
                    ImageName = v.ImageName,
                    File = v.File,
                    FileName = v.FileName,
                    DateCreated = v.DateCreated,
                    DateUpdated = v.DateUpdated,
                    Description = v.Description,
                    State = v.State,
                    Status = v.Status,
                    NumberOfItemsAvailable = null,
                    NumberOfItems = v.VoucherItems.Count()
                },
                predicate: x => x.Id == id,
                include: x => x.Include(x => x.VoucherItems));
            if (voucher != null)
            {
                return voucher;
            }
            throw new ApiException("Voucher not found", 404, "NOT_FOUND");
        }
        
        public async Task<VoucherResponse> GetVoucherWithCampaignDetailId(string id, string campaignDetailId)
        {
            var voucher = await _unitOfWork.GetRepository<Voucher>().SingleOrDefaultAsync(
                selector: v => new VoucherResponse
                {
                    Id = v.Id,
                    BrandId = v.BrandId,
                    BrandName = v.Brand.BrandName,
                    TypeId = v.TypeId,
                    TypeName = v.Type.TypeName,
                    VoucherName = v.VoucherName,
                    Price = v.Price,
                    Rate = v.Rate,
                    Condition = v.Condition,
                    Image = v.Image,
                    ImageName = v.ImageName,
                    File = v.File,
                    FileName = v.FileName,
                    DateCreated = v.DateCreated,
                    DateUpdated = v.DateUpdated,
                    Description = v.Description,
                    State = v.State,
                    Status = v.Status,
                    NumberOfItemsAvailable = v.VoucherItems != null 
                                                            ? v.VoucherItems
                                                                .Where(i => !(bool)i.IsLocked && !(bool)i.IsBought && !(bool)i.IsUsed && i.CampaignDetailId == campaignDetailId)
                                                                .Count() 
                                                            : 0,
                    NumberOfItems = v.VoucherItems.Where(i => i.CampaignDetailId == campaignDetailId).Count()
                },
                predicate: x => x.Id == id,
                include: x => x.Include(x => x.VoucherItems));
            if (voucher != null)
            {
                return voucher;
            }
            throw new ApiException("Voucher not found", 404, "NOT_FOUND");
        }

        public async Task<IPaginate<VoucherResponse>> GetVouchers(string brandId, string? search, bool? isAsc, bool? state, int page, int size)
        {

            Expression<Func<Voucher, bool>> filterQuery = x =>
                            x.Status == true &&
                            x.State == state && x.BrandId == brandId &&
                            (string.IsNullOrEmpty(search) || // search null
                            x.VoucherName.Contains(search) ||
                            x.Type.TypeName.Contains(search) ||
                            x.Condition.Contains(search));


            var vouchers = await _unitOfWork.GetRepository<Voucher>().GetPagingListAsync(
                selector: x => new VoucherResponse
                {
                    Id = x.Id,
                    BrandId = x.BrandId,
                    BrandName = x.Brand.BrandName,
                    TypeId = x.TypeId,
                    TypeName = x.Type.TypeName,
                    VoucherName = x.VoucherName,
                    Price = x.Price,
                    Rate = x.Rate,
                    Condition = x.Condition,
                    Image = x.Image,
                    ImageName = x.ImageName,
                    File = x.File,
                    FileName = x.FileName,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    State = x.State,
                    Status = x.Status,
                    NumberOfItemsAvailable = null,
                    //x.VoucherItems.Where(i => !(bool)i.IsLocked && !(bool)i.IsBought && !(bool)i.IsUsed && i.CampaignDetailId.IsNullOrEmpty()).Count(),
                    NumberOfItems = x.VoucherItems.Count()
                },
                predicate: filterQuery,
                page: page,
                size: size,
                orderBy: x => isAsc.HasValue && isAsc.Value ? x.OrderBy(v => v.DateCreated) : x.OrderByDescending(v => v.DateCreated)
                );
            return vouchers;
        }

        public async Task<bool> UpdateVoucher(string id, VoucherRequest request)
        {
            // Lấy voucher từ cơ sở dữ liệu
            var voucher = await _unitOfWork.GetRepository<Voucher>().SingleOrDefaultAsync(predicate: x => x.Id.Equals(id));
            if (voucher == null)
            {
                throw new ApiException("Voucher not found", 404, "VOUCHER_NOT_FOUND");
            }

            // Nếu có ảnh mới, upload lên Cloudinary
            if (request.Image != null && request.Image.Length > 0)
            {
                var image = await _cloudinaryService.UploadImageAsync(request.Image);
                if (image != null)
                {
                    // Xóa ảnh cũ trên Cloudinary
                    if (!string.IsNullOrEmpty(voucher.ImageName))
                    {
                        await _cloudinaryService.RemoveImageAsync(voucher.ImageName);
                    }

                    voucher.Image = image.SecureUrl.AbsoluteUri;
                    voucher.ImageName = image.PublicId;
                }
            }

            // Cập nhật các thông tin khác của voucher
            voucher.BrandId = request.BrandId;
            voucher.TypeId = request.TypeId;
            voucher.Price = request.Price;
            voucher.Rate = request.Rate;
            voucher.Condition = request.Condition;
            voucher.VoucherName = request.VoucherName;
            voucher.Description = request.Description;
            voucher.State = request.State;
            voucher.DateUpdated = DateTime.Now;

            // Cập nhật vào cơ sở dữ liệu
            _unitOfWork.GetRepository<Voucher>().UpdateAsync(voucher);
            var result = await _unitOfWork.CommitAsync();
            if (result > 0)
            {
                return true;
            }

            throw new ApiException("Update voucher failed", 400, "VOUCHER_UPDATE_FAIL");
        }

    }
}
