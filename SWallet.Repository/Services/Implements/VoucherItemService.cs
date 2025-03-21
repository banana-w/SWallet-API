using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Voucher;
using SWallet.Repository.Payload.Response.Voucher;
using SWallet.Repository.Services.Interfaces;

namespace SWallet.Repository.Services.Implements
{
    public class VoucherItemService : BaseService<VoucherItemService>, IVoucherItemService
    {
        public VoucherItemService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<VoucherItemService> logger) : base(unitOfWork, logger)
        {
        }

        public async Task<bool> GenerateVoucherItemsAsync(VoucherItemRequest voucherItemRequest)
        {
            var campaign = await _unitOfWork.GetRepository<CampaignDetail>().AnyAsync(x => x.CampaignId.Equals(voucherItemRequest.CampaignDetailId));
            if (campaign)
                throw new ApiException("Campaign not found", 400);

            var voucherItems = new List<VoucherItem>();
            for (int i = 0; i < voucherItemRequest.Quantity; i++)
            {
                voucherItems.Add(new VoucherItem
                {
                    Id = Ulid.NewUlid().ToString(),
                    VoucherId = voucherItemRequest.VoucherId,
                    CampaignDetailId = voucherItemRequest.CampaignDetailId,
                    VoucherCode = "code",
                    IsBought = false,
                    IsLocked = false,
                    IsUsed = false,
                    DateCreated = DateTime.Now,
                    DateIssued = DateTime.Now,
                    ValidOn = voucherItemRequest.ValidOn,
                    ExpireOn = voucherItemRequest.ExpireOn, 
                    State = true,
                    Status = true,
                });
            }

            await _unitOfWork.GetRepository<VoucherItem>().InsertRangeAsync(voucherItems);
            var result = await _unitOfWork.CommitAsync() > 0;
            return result;
        }

        public Task<VoucherItem> GetVoucherItemByIdAsync(string voucherId)
        {
            var result = _unitOfWork.GetRepository<VoucherItem>().SingleOrDefaultAsync(predicate: x => x.VoucherId.Equals(voucherId));
            return result;
        }

        public async Task<IEnumerable<VoucherItemResponse>> GetVoucherItemsByCampaignDetailIdAsync(IEnumerable<string> campaignDetailIds)
        {
               var result = await _unitOfWork.GetRepository<VoucherItem>().GetListAsync(
                    selector: x => new VoucherItemResponse
                    {
                        Id = x.Id,
                        VoucherId = x.VoucherId,
                        CampaignDetailId = x.CampaignDetailId,
                        VoucherCode = x.VoucherCode,
                        Index = x.Index,
                        IsLocked = x.IsLocked,
                        IsBought = x.IsBought,
                        IsUsed = x.IsUsed,
                        ValidOn = x.ValidOn,
                        ExpireOn = x.ExpireOn,
                        DateCreated = x.DateCreated,
                        DateIssued = x.DateIssued,
                        Status = x.Status,        
                    },
                     predicate: x => campaignDetailIds.Contains(x.CampaignDetailId));
            return result;
        }

        public async Task<bool> RedeemVoucherAsync(string voucherId)
        {
            var voucherItem = await _unitOfWork.GetRepository<VoucherItem>().SingleOrDefaultAsync(predicate: x => x.VoucherId.Equals(voucherId));
            if (voucherItem == null || (bool)voucherItem.IsBought)
            {
                throw new ApiException("Voucher not found or already redeemed", 400);
            }

            voucherItem.IsBought = true;
             _unitOfWork.GetRepository<VoucherItem>().UpdateAsync(voucherItem);
            var result = await _unitOfWork.CommitAsync() > 0;
            if (!result)
                throw new ApiException("Redeem voucher failed", 500);
            return result;
        }
    }
}
