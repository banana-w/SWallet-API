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
            if (request.Image != null && request.Image.Length > 0)
            {
                var image = await _cloudinaryService.UploadImageAsync(request.Image);
                if (image != null)
                {
                    var voucher = new Voucher
                    {
                        BrandId = request.BrandId,
                        TypeId = request.TypeId,
                        Price = request.Price,
                        Rate = request.Rate,
                        Condition = request.Condition,
                        VoucherName = request.VoucherName,
                        Description = request.Description,
                        Image = image.SecureUrl.AbsoluteUri,
                        ImageName = image.PublicId,
                        State = request.State,
                        DateCreated = DateTime.Now,
                        DateUpdated = DateTime.Now,
                        Status = true
                    };

                    // Add voucher to the database
                    await _unitOfWork.GetRepository<Voucher>().InsertAsync(voucher);
                    var result = await _unitOfWork.CommitAsync();
                    if (result > 0)
                    {
                        return true;
                    }
                }
            }
            throw new ApiException("Create voucher fail", 400, "VOUCHER_FAIL");
        }

        public Task<bool> DeleteVoucher(string id)
        {
            throw new NotImplementedException();
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
                    NumberOfItemsAvailable = v.VoucherItems.Where(i => !(bool)i.IsLocked && !(bool)i.IsBought && !(bool)i.IsUsed && i.CampaignDetailId.IsNullOrEmpty()).Count(),
                    NumberOfItems = v.VoucherItems.Count()
                },
                predicate: x => x.Id == id);
            if (voucher != null)
            {
                return voucher;
            }
            throw new ApiException("Voucher not found", 404, "NOT_FOUND");
        }

        public async Task<IPaginate<VoucherResponse>> GetVouchers(string search, bool? isAsc, bool? state, int page, int size)
        {
            Expression<Func<Voucher, bool>> filterQuery = x =>
                x.Status == true &&
                x.State == state &&
                (x.VoucherName.Contains(search) ||
                 x.Brand.BrandName.Contains(search) ||
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
                    NumberOfItemsAvailable = x.VoucherItems.Where(i => !(bool)i.IsLocked && !(bool)i.IsBought && !(bool)i.IsUsed && i.CampaignDetailId.IsNullOrEmpty()).Count(),
                    NumberOfItems = x.VoucherItems.Count()
                },
                predicate: filterQuery,
                page: page,
                size: size,
                orderBy: x => isAsc.HasValue && isAsc.Value ? x.OrderBy(v => v.DateCreated) : x.OrderByDescending(v => v.DateCreated)
                );
            return vouchers;
        }

        public Task<bool> UpdateVoucher(VoucherRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
