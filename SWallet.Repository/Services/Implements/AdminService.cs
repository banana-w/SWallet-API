using AutoMapper;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Security;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Implement;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.Request.Account;
using SWallet.Repository.Payload.Response.Account;
using SWallet.Repository.Payload.Response.Admin;
using SWallet.Repository.Services.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Account = SWallet.Domain.Models.Account;


namespace SWallet.Repository.Services.Implements
{
    public class AdminService : BaseService<AdminService>, IAdminService
    {
        private readonly ICloudinaryService cloudinaryService;
        private readonly Mapper mapper;
        private readonly string ACCOUNT_FOLDER_NAME = "accounts";

 

        public AdminService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<AdminService> logger, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, httpContextAccessor)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Admin, AdminResponse>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
                    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Account.UserName))
                    .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                    .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Account.Phone))
                    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Account.Email))
                    .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Account.Avatar))
                    .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.Account.FileName))
                    .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => src.DateCreated))
                    .ForMember(dest => dest.DateUpdated, opt => opt.MapFrom(src => src.DateUpdated))
                    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Account.Description))
                    .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                    .ForMember(dest => dest.NumberOfRequests, opt => opt.MapFrom(src => src.Requests.Count))
                    .ForMember(dest => dest.AmountOfRequests, opt => opt.MapFrom(src => src.Requests.Sum(r => r.Amount))); // Assuming Request has an Amount property

              
            });
        }

        public async Task<AdminResponse> Add(CreateAdminModel creation)
        {
            Account account = mapper.Map<Account>(creation);
            await _unitOfWork.GetRepository<Account>().InsertAsync(account);
            Admin admin = mapper.Map<Admin>(creation);
            admin.AccountId = account.Id;
            await _unitOfWork.GetRepository<Admin>().InsertAsync(admin);
            return mapper.Map<AdminResponse>(admin);
        }

        public async Task Delete(string id)
        {
            var adminRepo = _unitOfWork.GetRepository<Admin>();
            var accountRepo = _unitOfWork.GetRepository<Account>();

            var admin = await adminRepo.SingleOrDefaultAsync(
                predicate: a => a.Id == id,
                include: query => query.Include(a => a.Account)
            );

            if (admin == null)
            {
                throw new InvalidParameterException("Không tìm thấy quản trị viên");
            }
            if (admin.Requests?.Count > 0)
            {
                throw new InvalidParameterException("Không thể xóa quản trị viên do tồn tại yêu cầu nạp bởi tài khoản này");
            }

            if (!string.IsNullOrEmpty(admin.Account?.Avatar))
            {
                var isImageDeleted = await cloudinaryService.RemoveImageAsync(admin.Account.Avatar);

                if (!isImageDeleted)
                {
                    throw new Exception($"Không thể xóa ảnh đại diện: {admin.Account.Avatar}");
                }
            }

             adminRepo.DeleteAsync(admin);

            if (admin.Account != null)
            {
                 accountRepo.DeleteAsync(admin.Account);
            }
        }



        public async Task<IPaginate<AdminResponse>> GetAll(int page, int size)
        {
            var adminRepo = _unitOfWork.GetRepository<Admin>();

            // Lấy tất cả các quản trị viên từ repository và chuyển chúng thành IQueryable
            var adminsQuery = (await adminRepo.GetListAsync()).AsQueryable();

            // Phân trang kết quả
            var paginatedAdmins = await adminsQuery
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            // Ánh xạ các Admin sang AdminResponse
            var adminResponses = paginatedAdmins.Select(admin => mapper.Map<AdminResponse>(admin)).ToList();

            // Lấy tổng số lượng admin để trả về thông tin phân trang
            var totalCount = adminsQuery.Count();

            return new Paginate<AdminResponse>
            {
                
                Items = adminResponses
            };
        }


        public async Task<AdminResponse> GetById(string id)
        {
            var adminRepo = _unitOfWork.GetRepository<Admin>();

            // Lấy thông tin quản trị viên cùng với tài khoản
            var admin = await adminRepo.SingleOrDefaultAsync(
                predicate: a => a.Id == id,
                include: query => query.Include(a => a.Account)
            );

            if (admin == null)
            {
                throw new InvalidParameterException("Không tìm thấy quản trị viên");
            }

            // Ánh xạ Admin sang AdminResponse
            return mapper.Map<AdminResponse>(admin);
        }

        public async Task<AdminResponse> Update(string id, Admin update)
        {
            var adminRepo = _unitOfWork.GetRepository<Admin>();
            var accountRepo = _unitOfWork.GetRepository<Account>();

            // Lấy thông tin quản trị viên và tài khoản liên quan
            var admin = await adminRepo.SingleOrDefaultAsync(
                predicate: a => a.Id == id,
                include: query => query.Include(a => a.Account)
            );

            if (admin == null)
            {
                throw new InvalidParameterException("Không tìm thấy quản trị viên");
            }

            // Cập nhật thông tin tài khoản
            if (update.Account != null)
            {
                var account = admin.Account;
                account.UserName = update.Account.UserName;
                account.Phone = update.Account.Phone;
                account.Email = update.Account.Email;
                account.Description = update.Account.Description;

                accountRepo.UpdateAsync(account);
            }

            // Cập nhật thông tin quản trị viên
            admin.FullName = update.FullName;
            admin.State = update.State;
            admin.DateUpdated = DateTime.UtcNow;
            admin.Status = update.Status;
            adminRepo.UpdateAsync(admin);

            // Ánh xạ Admin sang AdminResponse
            return mapper.Map<AdminResponse>(admin);
        }

        
    }
}
