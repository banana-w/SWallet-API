using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Request.Campaign;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Payload.Response.Campaign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Implements
{
    public interface ICampaignService
    {
        Task<IPaginate<CampaignResponse>> GetCampaigns(string searchName, int page, int size);
        Task<CampaignResponse> GetCampaignById(string id);
        Task<CampaignResponse> CreateCampaign(CreateCampaignModel campaign);
        Task<CampaignResponse> UpdateCampaign(string id, UpdateCampaignModel campaign);
        void Delete(string id);

    }
}
