using SWallet.Domain.Models;
using SWallet.Repository.Payload.Request.Voucher;
using SWallet.Repository.Payload.Response.Voucher;
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
        Task<List<VoucherItem>> RedeemVoucherAsync(string campaignId, int quantity);
        Task<VoucherItem> GetVoucherItemByIdAsync(string voucherId);
        Task<IEnumerable<VoucherItemResponse>> GetVoucherItemsByCampaignDetailIdAsync(IEnumerable<string> campaignDetailId);
    }
}
