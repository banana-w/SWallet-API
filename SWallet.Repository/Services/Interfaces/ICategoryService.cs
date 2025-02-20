using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Request.Category;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Payload.Response.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IPaginate<CategoryResponse>> GetCategory(string searchName, int page, int size);
        Task<CategoryResponse> GetCateogoryById(string id);
        Task<CategoryResponse> CreateCategory(CreateCategoryModel category);
        Task<CategoryResponse> UpdateCategory(string id, UpdateCategoryModel category);
        void Delete(string id);
    }
}
