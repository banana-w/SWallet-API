using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.CampTransaction;
using SWallet.Repository.Payload.Response.CampTransaction;
using SWallet.Repository.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Implements
{
    public class CampaignTransactionService : BaseService<CampaignDetailService>, ICampaignTransactionService
    {
        public CampaignTransactionService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<CampaignDetailService> logger, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, httpContextAccessor)
        {
        }

        public async Task<bool> AddCampaignTransaction(CampaignTransactionRequest campaignTransaction)
        {

            var transaction = new CampaignTransaction
            {
                Id = Ulid.NewUlid().ToString(),
                CampaignId = campaignTransaction.CampaignId,
                WalletId = campaignTransaction.WalletId,
                Amount = campaignTransaction.Amount,
                Rate = campaignTransaction.Rate,
                DateCreated = DateTime.UtcNow,
                Description = campaignTransaction.Description
            };

            await _unitOfWork.GetRepository<CampaignTransaction>().InsertAsync(transaction);
            var result = await _unitOfWork.CommitAsync() > 0;
            if (!result)
            {
                throw new ApiException("Add activity transaction failed");
            }
            return true;
        }

        public async Task<IPaginate<CampTransactionResponse>> GetCampaignTransaction(string brandId, int page, int size)
        {
            var  trans = await _unitOfWork.GetRepository<CampaignTransaction>().GetPagingListAsync(
                selector: x => new CampTransactionResponse
                {
                    CampaignId = x.CampaignId,
                    WalletId = x.WalletId,
                    Amount = x.Amount,
                    Rate = x.Rate,
                    DateCreated = x.DateCreated,
                    Description = x.Description
                },
                predicate: x => x.Wallet.BrandId == brandId,
                orderBy: x => x.OrderByDescending(x => x.DateCreated),
                page: page,
                size: size
                );
            return trans;
        }
    }

}
