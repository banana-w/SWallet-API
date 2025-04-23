using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.WishList;
using SWallet.Repository.Payload.Response.Station;
using SWallet.Repository.Payload.Response.Wishlist;
using SWallet.Repository.Services.Interfaces;
using System.Linq.Expressions;

namespace SWallet.Repository.Services.Implements
{

    public class WishlistService : BaseService<WishlistService>, IWishlistService
    {
        private readonly Mapper mapper;


        public WishlistService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<WishlistService> logger) : base(unitOfWork, logger)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Wishlist, WishlListModel>()
                    .ForMember(c => c.StudentName, opt => opt.MapFrom(src => src.Student.FullName))
                    .ForMember(c => c.StudentImage, opt => opt.MapFrom(src => src.Student.Account.Avatar))
                    .ForMember(c => c.BrandName, opt => opt.MapFrom(src => src.Brand.BrandName))
                    .ForMember(c => c.BrandImage, opt => opt.MapFrom(src => src.Brand.Account.Avatar))
                    .ReverseMap();

                cfg.CreateMap<Wishlist, WishListUpdateModel>()
                    .ReverseMap()
                    .AfterMap((src, dest) => dest.Status = !dest.Status);
            });
            mapper = new Mapper(config);
            _unitOfWork = unitOfWork;
        }

        public async Task<IPaginate<WishlListModel>> GetAll(List<string> studentIds, List<string> brandIds, string? search, int page, int size)
        {
            Expression<Func<Wishlist, bool>> filterQuery = p => true; // Điều kiện mặc định

            // Lọc theo studentIds nếu danh sách không rỗng
            if (studentIds != null && studentIds.Any())
            {
                filterQuery = CombinePredicates(filterQuery, p => studentIds.Contains(p.StudentId));
            }

            // Lọc theo brandIds nếu danh sách không rỗng
            if (brandIds != null && brandIds.Any())
            {
                filterQuery = CombinePredicates(filterQuery, p => brandIds.Contains(p.BrandId));
            }

            // Lọc theo search (tìm kiếm theo BrandName hoặc Description)
            if (!string.IsNullOrEmpty(search))
            {
                filterQuery = CombinePredicates(filterQuery, p =>
                    p.Brand.BrandName.Contains(search) ||
                    (p.Description != null && p.Description.Contains(search)));
            }

            var wishlist = await _unitOfWork.GetRepository<Wishlist>().GetPagingListAsync(
                selector: x => new WishlListModel
                {
                    Id = x.Id,
                    StudentId = x.StudentId,
                    BrandId = x.BrandId,
                    StudentName = x.Student.FullName,
                    StudentImage = x.Student.Account.Avatar,
                    BrandName = x.Brand.BrandName,
                    BrandImage = x.Brand.Account.Avatar,
                    Description = x.Description,
                    Status = x.Status,
                    State = x.State
                },
                predicate: filterQuery,
                include: source => source
                    .Include(s => s.Student)
                        .ThenInclude(a => a.Account)
                    .Include(b => b.Brand)
                        .ThenInclude(a => a.Account),
                page: page,
                size: size
            );

            if (wishlist == null || !wishlist.Items.Any())
            {
                throw new ApiException("Wishlist not found", 404, "NOT_FOUND");
            }

            return wishlist;
        }
        // Helper method để kết hợp predicates
        private Expression<Func<T, bool>> CombinePredicates<T>(Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            var parameter = first.Parameters[0];
            var body = Expression.AndAlso(first.Body, Expression.Invoke(second, parameter));
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        public async Task<List<string>> GetWishlishBrandIdByStudentId(string studentId)
        {
            if (string.IsNullOrEmpty(studentId))
            {
                throw new ApiException("Student ID is required", 400, "BAD_REQUEST");
            }
            var wishlist = await _unitOfWork.GetRepository<Wishlist>()
                .GetListAsync(predicate: w => w.StudentId == studentId);
            if (wishlist == null || !wishlist.Any())
            {
                throw new ApiException("No wishlist found for the given student ID", 404, "NOT_FOUND");
            }
            return wishlist.Select(w => w.BrandId).ToList();
        }

        public async Task<WishlListModel> UpdateWishlist(WishListUpdateModel update)
        {
            if (update == null || string.IsNullOrEmpty(update.StudentId) || string.IsNullOrEmpty(update.BrandId))
            {
                throw new ApiException("Invalid update data", 400, "BAD_REQUEST");
            }

            // Kiểm tra StudentId và BrandId tồn tại
            var studentExists = await _unitOfWork.GetRepository<Student>().AnyAsync(s => s.Id == update.StudentId);
            if (!studentExists)
            {
                throw new ApiException("Student not found", 404, "NOT_FOUND");
            }

            var brandExists = await _unitOfWork.GetRepository<Brand>().AnyAsync(b => b.Id == update.BrandId);
            if (!brandExists)
            {
                throw new ApiException("Brand not found", 404, "NOT_FOUND");
            }

            var repository = _unitOfWork.GetRepository<Wishlist>();

            // Tìm wishlist hiện có
            var wishlist = await repository.SingleOrDefaultAsync(
                predicate: w => w.StudentId == update.StudentId && w.BrandId == update.BrandId,
                include: source => source
                    .Include(s => s.Student)
                        .ThenInclude(a => a.Account)
                    .Include(b => b.Brand)
                        .ThenInclude(a => a.Account)
            );

            if (wishlist != null)
            {
                // Cập nhật wishlist hiện có
                mapper.Map(update, wishlist);
                repository.UpdateAsync(wishlist);
            }
            else
            {
                // Tạo mới wishlist
                wishlist = new Wishlist
                {
                    Id = Ulid.NewUlid().ToString(),
                    StudentId = update.StudentId,
                    BrandId = update.BrandId,
                    Description = update.Description,
                    State = update.State,
                    Status = true
                };
                await repository.InsertAsync(wishlist);
            }

            await _unitOfWork.CommitAsync();

            // Tải lại wishlist để lấy dữ liệu liên quan
            wishlist = await repository.SingleOrDefaultAsync(
                predicate: w => w.Id == wishlist.Id,
                include: source => source
                    .Include(s => s.Student)
                        .ThenInclude(a => a.Account)
                    .Include(b => b.Brand)
                        .ThenInclude(a => a.Account)
            );

            if (wishlist == null)
            {
                throw new ApiException("Failed to retrieve updated wishlist", 500, "INTERNAL_SERVER_ERROR");
            }

            return mapper.Map<WishlListModel>(wishlist);
        }

        
    }
}
