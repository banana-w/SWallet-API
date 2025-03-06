using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.Voucher;
using SWallet.Repository.Payload.Response.Voucher;

namespace SWallet.Repository.Services.Interfaces
{
    public interface IVoucherTypeService
    {
        Task<bool> CreateVoucherType(VoucherTypeRequest request);
        Task<bool> UpdateVoucherType(string id, VoucherTypeRequest request);
        Task<bool> DeleteVoucherType(string id);
        Task<VoucherTypeResponse> GetVoucherTypeById(string id);
        Task<IPaginate<VoucherTypeResponse>> GetVoucherTypes(string search, int page, int size);
    }
}
