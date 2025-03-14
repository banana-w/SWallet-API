using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Product;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Payload.Response.Product;
using SWallet.Repository.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace SWallet.Repository.Services.Implements
{
    public class ProductService : BaseService<ProductService>, IProductService
    {
        private readonly Mapper mapper;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger logger;

        public ProductService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<ProductService> logger, ICloudinaryService cloudinaryService) : base(unitOfWork, logger)
        {
            _cloudinaryService = cloudinaryService;

        }

        public async Task<ProductResponse> CreateProduct(CreateProductModel product)
        {
            //var imageUris = new List<string>(); 
            //if (product.ProductImages != null && product.ProductImages.Count > 0)
            //{
            //    foreach (var image in product.ProductImages)
            //    {
            //        var uploadResult = await _cloudinaryService.UploadImageAsync(image); 
            //        if (uploadResult != null && !string.IsNullOrEmpty(uploadResult.SecureUrl?.AbsoluteUri))
            //        {
            //            imageUris.Add(uploadResult.SecureUrl.AbsoluteUri); 
            //        }
            //    }
            //}

            var newProduct = new Product
            {
                Id = Ulid.NewUlid().ToString(),
                CategoryId = product.CategoryId,
                ProductName = product.ProductName,
                Price = product.Price,
                Weight = product.Weight,
                Quantity = product.Quantity,
                Description = product.Description,
                State = product.State,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Status = true


                //ProductImages = imageUris.Select(x => new ProductImage { ImageUrl = x }).ToList()
            };

            await _unitOfWork.GetRepository<Product>().InsertAsync(newProduct);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccess)
            {
                throw new ApiException("Create Product Fail", 400, "BAD_REQUEST");
            }

            var result = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
                selector: x => new ProductResponse
                {
                    Id = x.Id,
                    CategoryId = x.CategoryId,
                    ProductName = x.ProductName,
                    Price = x.Price,
                    Weight = x.Weight,
                    Quantity = x.Quantity,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    State = x.State,
                    Status = x.Status,
                    CategoryName = x.Category.CategoryName
                },
                predicate: x => x.Id == newProduct.Id,
                include: x => x.Include(a => a.Category));

            return result;
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<ProductResponse> GetProductById(string id)
        {
            var area = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
               selector: x => new ProductResponse
               {
                   Id = x.Id,
                   CategoryId = x.CategoryId,
                   ProductName = x.ProductName,
                   Price = x.Price,
                   Weight = x.Weight,
                   Quantity = x.Quantity,
                   DateCreated = x.DateCreated,
                   DateUpdated = x.DateUpdated,
                   Description = x.Description,
                   State = x.State,
                   Status = x.Status,
                   CategoryName = x.Category.CategoryName
               },
               predicate: x => x.Id == id);
            return area;
        }

        public async Task<IPaginate<ProductResponse>> GetProducts(string searchName, int page, int size)
        {
            Expression<Func<Product, bool>> filterQuery;
            if (string.IsNullOrEmpty(searchName))
            {
                filterQuery = p => true;
            }
            else
            {
                filterQuery = p => p.ProductName.Contains(searchName);
            }

            var areas = await _unitOfWork.GetRepository<Product>().GetPagingListAsync(
                selector: x => new ProductResponse
                {
                    Id = x.Id,
                    CategoryId = x.CategoryId,
                    ProductName = x.ProductName,
                    Price = x.Price,
                    Weight = x.Weight,
                    Quantity = x.Quantity,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    State = x.State,
                    Status = x.Status,
                    CategoryName = x.Category.CategoryName

                },
                predicate: filterQuery,
                include: x => x.Include(a => a.Category),
                page: page,
                size: size);
            return areas;
        }

        public async Task<ProductResponse> UpdateProduct(string id, UpdateProductModel product)
        {
            var updateProduct = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(predicate: x => x.Id == id,
                include: x => x.Include(a => a.Category));

            if (updateProduct == null)
            {
                throw new ApiException("Product not found", 404, "NOT_FOUND");
            }
            updateProduct.CategoryId = product.CategoryId;
            updateProduct.ProductName = product.ProductName;
            updateProduct.Price = product.Price;
            updateProduct.Weight = product.Weight;
            updateProduct.Quantity = product.Quantity;
            updateProduct.Description = product.Description;
            updateProduct.State = product.State;
            updateProduct.DateUpdated = DateTime.Now;

            _unitOfWork.GetRepository<Product>().UpdateAsync(updateProduct);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccess)
            {
                throw new ApiException("Update product failed", 500, "INTERNAL_SERVER_ERROR");
            }

            var response = new ProductResponse
            {
                Id = updateProduct.Id,
                CategoryId = updateProduct.CategoryId,
                ProductName = updateProduct.ProductName,
                Price = updateProduct.Price,
                Weight = updateProduct.Weight,
                Quantity = updateProduct.Quantity,
                DateCreated = updateProduct.DateCreated,
                DateUpdated = updateProduct.DateUpdated,
                Description = updateProduct.Description,
                State = updateProduct.State,
                Status = updateProduct.Status,
                CategoryName = updateProduct.Category?.CategoryName // Kiểm tra null ở đây
            };

            return response;
        }
    }
}
