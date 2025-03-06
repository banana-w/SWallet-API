using SWallet.Repository.Payload.Request.Voucher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface IVoucherItemService
    {
        Task<bool> GenerateVoucherItemsAsync(VoucherItemRequest voucherItemRequest);
        Task<bool> RedeemVoucherAsync(int voucherId, int studentId);
    }
}
