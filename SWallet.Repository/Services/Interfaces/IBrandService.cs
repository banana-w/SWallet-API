using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.Area;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Response.Area;
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
        Task<IPaginate<BrandResponse>> GetBrands(string searchName, int page, int size, bool status, string? studentId);
        Task<BrandResponse> GetBrandById(string id, string? studentId);
        Task<BrandResponse> CreateBrand(CreateBrandModel brand);
        Task<BrandResponse> UpdateBrand(string id, UpdateBrandModel brand);
        Task<BrandResponse> GetBrandbyAccountId(string accountId);
        void Delete(string id);
        Task<BrandResponse> CreateBrandAsync(string accountId, CreateBrandByAccountId brand);

        Task<long> CountVoucherItemToday(string brandId, DateOnly date);

        long CountBrand();

        Task<List<Brand>> GetRanking(int limit);

    }
}
