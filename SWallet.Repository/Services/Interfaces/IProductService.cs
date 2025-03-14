using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Request.Product;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Payload.Response.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface IProductService
    {
        Task<IPaginate<ProductResponse>> GetProducts(string searchName, int page, int size);
        Task<ProductResponse> GetProductById(string id);
        Task<ProductResponse> CreateProduct(CreateProductModel product);
        Task<ProductResponse> UpdateProduct(string id, UpdateProductModel product);
        void Delete(string id);
    }
}
