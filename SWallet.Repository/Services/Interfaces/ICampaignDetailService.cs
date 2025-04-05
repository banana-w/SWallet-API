using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Payload.Response.Campaign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface ICampaignDetailService
    {
        Task<IPaginate<CampaignDetailResponse>> GetCampaignDetails(string searchName, int page, int size);
        Task<IPaginate<CampaignDetailResponse>> GetAllCampaignDetailByStore(string storeId, string searchName, int page, int size);
        List<string> GetAllVoucherItemByCampaignDetail(string id);
        Task<CampaignDetailExtra> GetById(string id);

    }
}
