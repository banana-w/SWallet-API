using AutoMapper;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Category;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Payload.Response.Category;
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
    public class CategoryService : BaseService<CategoryService>, ICategoryService
    {
        private readonly Mapper mapper;
        private readonly ICloudinaryService _cloudinaryService;

        public CategoryService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<CategoryService> logger, ICloudinaryService cloudinaryService) : base(unitOfWork, logger)
        {
            _cloudinaryService = cloudinaryService;
            var config = new MapperConfiguration(cfg
               =>
            {
                cfg.CreateMap<Category, CategoryResponse>()
                .ReverseMap();
                cfg.CreateMap<Category, UpdateCategoryModel>()
                .ReverseMap()
                .ForMember(t => t.Image, opt => opt.Ignore())
                .ForMember(t => t.DateUpdated, opt => opt.MapFrom(src => DateTime.Now));
                cfg.CreateMap<Category, CreateCategoryModel>()
                .ReverseMap()
                .ForMember(t => t.Id, opt => opt.MapFrom(src => Ulid.NewUlid()))
                .ForMember(t => t.DateCreated, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(t => t.DateUpdated, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(t => t.Status, opt => opt.MapFrom(src => true));
            });
            mapper = new Mapper(config);
        }

        public async Task<CategoryResponse> CreateCategory(CreateCategoryModel category)
        {
            
            var imageUri = string.Empty;
            if (category.Image != null && category.Image.Length > 0)
            {
                var uploadResult = await _cloudinaryService.UploadImageAsync(category.Image);
                imageUri = uploadResult.SecureUrl.AbsoluteUri;
            }

            var newCategory = new Category
            {
                Id = Ulid.NewUlid().ToString(),
                CategoryName = category.CategoryName,
                Image = imageUri,
                FileName = !string.IsNullOrEmpty(imageUri)
                          ? imageUri.Split('/')[imageUri.Split('/').Length - 1]
                          : "default_cover.jpg",
                Description = category.Description,
                State = category.State,
                Status = true,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now

            };
            await _unitOfWork.GetRepository<Category>().InsertAsync(newCategory);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;

            if (isSuccess)
            {
                return new CategoryResponse
                {
                    Id = newCategory.Id,
                    CategoryName = newCategory.CategoryName,
                    Image = imageUri,
                    FileName = newCategory.FileName,
                    Description = newCategory.Description,
                    State = newCategory.State,
                    Status = newCategory.Status,
                    DateCreated = newCategory.DateCreated,
                    DateUpdated = newCategory.DateUpdated
                };
            }
            throw new ApiException("Create Category Fail", 400, "BAD_REQUEST");
        }

    

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IPaginate<CategoryResponse>> GetCategory(string? searchName, int page, int size)
        {
            Expression<Func<Category, bool>> filterQuery;
            if (string.IsNullOrEmpty(searchName))
            {
                filterQuery = p => true;
            }
            else
            {
                filterQuery = p => p.CategoryName.Contains(searchName);
            }

            var areas = await _unitOfWork.GetRepository<Category>().GetPagingListAsync(
                selector: x => new CategoryResponse
                {
                    Id = x.Id,
                    CategoryName = x.CategoryName,
                    Image = x.Image,
                    Description = x.Description,
                    State = x.State,
                    Status = x.Status,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    FileName = x.FileName



                },
                predicate: filterQuery,
                page: page,
                size: size);
            return areas;
        }

        public async Task<CategoryResponse> GetCateogoryById(string id)
        {
            var area = await _unitOfWork.GetRepository<Category>().SingleOrDefaultAsync(
                selector: x => new CategoryResponse
                {
                    Id = x.Id,
                    CategoryName = x.CategoryName,
                    Image = x.Image,
                    Description = x.Description,
                    State = x.State,
                    Status = x.Status,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    FileName = x.FileName


                },
                predicate: x => x.Id == id);
            return area;
        }

        public async Task<CategoryResponse> UpdateCategory(string id, UpdateCategoryModel category)
        {
            var updateCategory = await _unitOfWork.GetRepository<Category>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (updateCategory == null)
            {
                throw new ApiException("Category not found", 404, "NOT_FOUND");
            }
            if (category.Image != null && category.Image.Length > 0)
            {

                var f = await _cloudinaryService.UploadImageAsync(category.Image);


            }
            var categoryEntity = mapper.Map(category, updateCategory);
            _unitOfWork.GetRepository<Category>().UpdateAsync(categoryEntity);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;
            if (isSuccess)
            {
                return new CategoryResponse
                {
                    Id = categoryEntity.Id,
                    CategoryName = categoryEntity.CategoryName,
                    Image = categoryEntity.Image,
                    Description = categoryEntity.Description,
                    State = categoryEntity.State,
                    Status = categoryEntity.Status,
                    DateUpdated = DateTime.Now
                    
                };
            }

            throw new ApiException("Update Brand Fail", 400, "BAD_REQUEST");
        }
    }
}
