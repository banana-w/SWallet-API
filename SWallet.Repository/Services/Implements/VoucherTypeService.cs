using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Voucher;
using SWallet.Repository.Payload.Response.Voucher;
using SWallet.Repository.Services.Interfaces;
using System.Linq.Expressions;


namespace SWallet.Repository.Services.Implements
{
    public class VoucherTypeService : BaseService<VoucherTypeService>, IVoucherTypeService
    {
        private readonly ICloudinaryService _cloudinaryService;
        public VoucherTypeService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<VoucherTypeService> logger, ICloudinaryService cloudinaryService) : base(unitOfWork, logger)
        {
            _cloudinaryService = cloudinaryService;
        }

        public async Task<bool> CreateVoucherType(VoucherTypeRequest request)
        {
            var voucherType = new VoucherType
            {
                Id = Ulid.NewUlid().ToString(),
                TypeName = request.TypeName,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Description = request.Description,
                State = request.State,
                Status = true
            };
            if(request.Image != null && request.Image.Length > 0)
            {
                var image = await _cloudinaryService.UploadImageAsync(request.Image);
                if (image != null)
                {
                    voucherType.Image = image.SecureUrl.AbsoluteUri;
                    voucherType.FileName = image.PublicId;
                }
            }
            await _unitOfWork.GetRepository<VoucherType>().InsertAsync(voucherType);
            var result = await _unitOfWork.CommitAsync();
            if (result > 0)
            {
                return true;
            }
            throw new ApiException("Create voucher type fail", 400, "VOUCHER_TYPE_FAIL");
        }

        public Task<bool> DeleteVoucherType(string id)
        {
            throw new NotImplementedException();
        }

        public Task<VoucherTypeResponse> GetVoucherTypeById(string id)
        {
            var voucherType = _unitOfWork.GetRepository<VoucherType>().SingleOrDefaultAsync(
                selector: x => new VoucherTypeResponse
                {
                    Id = x.Id,
                    TypeName = x.TypeName,
                    Image = x.Image,
                    FileName = x.FileName,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    State = x.State,
                    Status = x.Status
                },
                predicate: x => x.Id == id);
            if (voucherType == null)
            {
                throw new ApiException("Voucher type not found", 404, "VOUCHER_TYPE_NOT_FOUND");
            }
            return voucherType;
        }

        public Task<IPaginate<VoucherTypeResponse>> GetVoucherTypes(string search, int page, int size)
        {
            Expression<Func<VoucherType, bool>> filterquery;
            if (!string.IsNullOrEmpty(search))
            {
                filterquery = x => x.TypeName.Contains(search);
            }
            else
            {
                filterquery = x => true;
            }
            var voucherTypes = _unitOfWork.GetRepository<VoucherType>().GetPagingListAsync(
                selector: x => new VoucherTypeResponse
                {
                    Id = x.Id,
                    TypeName = x.TypeName,
                    Image = x.Image,
                    FileName = x.FileName,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    State = x.State,
                    Status = x.Status
                },
                predicate: filterquery,
                page: page,
                size: size,
                orderBy: x => x.OrderBy(x => x.DateCreated)    
                );
            return voucherTypes;
        }

        public Task<bool> UpdateVoucherType(VoucherTypeRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
