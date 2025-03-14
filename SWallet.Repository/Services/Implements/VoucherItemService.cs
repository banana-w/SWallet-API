using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Voucher;
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

        public async Task<bool> RedeemVoucherAsync(int voucherId)
        {
            var voucherItem = await _unitOfWork.GetRepository<VoucherItem>().SingleOrDefaultAsync(predicate: x => x.VoucherId.Equals(voucherId));
            if (voucherItem == null || (bool)voucherItem.IsBought)
            {
                return false; 
            }

            voucherItem.IsBought = true;
             _unitOfWork.GetRepository<VoucherItem>().UpdateAsync(voucherItem);
            var result = await _unitOfWork.CommitAsync() > 0;
            return result;
        }
    }
}
