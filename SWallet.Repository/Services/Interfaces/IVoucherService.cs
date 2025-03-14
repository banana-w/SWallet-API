using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.Voucher;
using SWallet.Repository.Payload.Response.Voucher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface IVoucherService
    {
        Task<bool> CreateVoucher(VoucherRequest request);
        Task<bool> UpdateVoucher(VoucherRequest request);
        Task<bool> DeleteVoucher(string id);
        Task<VoucherResponse> GetVoucherById(string id);
        Task<IPaginate<VoucherResponse>> GetVouchers(string brandId, string? search, bool? state, bool? isAsc, int page, int size);
    }
}
