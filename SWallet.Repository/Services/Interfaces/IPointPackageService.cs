using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.PointPackage;
using SWallet.Repository.Payload.Request.Product;
using SWallet.Repository.Payload.Response.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface IPointPackageService
    {
        Task<IPaginate<PointPackage>> GetPointPackage(string searchName, int page, int size);
        Task<PointPackage> GetPointPackageById(string id);
        Task<PointPackage> CreatePointPackage(PointPackageModel package);
        Task<PointPackage> UpdatePointPackage(string id, PointPackageModel package);
        void Delete(string id);
    }
}
