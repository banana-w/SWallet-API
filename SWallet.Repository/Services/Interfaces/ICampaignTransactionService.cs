using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.CampTransaction;
using SWallet.Repository.Payload.Response.CampTransaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface ICampaignTransactionService
    {
        Task<bool> AddCampaignTransaction(CampaignTransactionRequest campaignTransaction);
        Task<IPaginate<CampTransactionResponse>> GetCampaignTransaction (string brandId, int page, int size);
    }
}
