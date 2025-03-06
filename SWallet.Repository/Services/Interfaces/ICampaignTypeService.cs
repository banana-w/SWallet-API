using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.Campaign;
using SWallet.Repository.Payload.Response.Campaign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface ICampaignTypeService
    {
        Task<IPaginate<CampaignTypeResponse>> GetCampaignType(string searchName, int page, int size);
        Task<CampaignTypeResponse> GetCampaignTypeById(string id);
        Task<CampaignTypeResponse> CreateCampaignType(CreateCampaignTypeModel type);
        Task<CampaignTypeResponse> UpdateCampaignType(string id, UpdateCampaignTypeModel type);

        void Delete(string id);
    }
}
