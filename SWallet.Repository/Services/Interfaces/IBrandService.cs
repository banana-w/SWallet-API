using SWallet.Domain.Models;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Response.Brand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
  
        public interface IBrandService
        {
            Task<BrandResponse> Add(CreateBrandModel creation);

            void Delete(string id);

            //PagedResultModel<BrandModel> GetAll
            //    (bool? state, string propertySort, bool isAsc, string search,
            //    int page, int limit, JwtRequestModel request);

            //BrandExtraModel GetById(string id, JwtRequestModel request);

            //PagedResultModel<CampaignModel> GetCampaignListByBrandId
            //    (string id, List<string> typeIds, List<string> storeIds, List<string> majorIds, List<string> campusIds,
            //    List<CampaignState> stateIds, string propertySort, bool isAsc, string search, int page, int limit);

            //PagedResultModel<TransactionModel> GetHistoryTransactionListByBrandId
            //    (string id, bool? state, string propertySort,
            //    bool isAsc, string search, int page, int limit);

            //PagedResultModel<StoreModel> GetStoreListByBrandId
            //    (string id, List<string> areaIds, bool? state, string propertySort, bool isAsc,
            //    string search, int page, int limit);

            //PagedResultModel<VoucherModel> GetVoucherListByBrandId
            //    (string id, List<string> typeIds, bool? state, string propertySort, bool isAsc,
            //    string search, int page, int limit);

            //Task<BrandExtraModel> Update(string id, UpdateBrandModel update);
        }
}
