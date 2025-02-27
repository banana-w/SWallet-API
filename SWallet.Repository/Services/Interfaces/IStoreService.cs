using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Request.Store;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Payload.Response.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface IStoreService
    {
        Task<IPaginate<StoreResponse>> GetStores(string searchName, int page, int size);
        Task<StoreResponse> GetStoreById(string id);
        Task<StoreResponse> CreateStore(string accountId, CreateStoreModel store);
        Task<StoreResponse> UpdateStore(string id, UpdateStoreModel store);
        void Delete(string id);
        
    }
}
