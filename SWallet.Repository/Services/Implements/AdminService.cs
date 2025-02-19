using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Security;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Enums;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Account;
using SWallet.Repository.Payload.Response.Account;
using SWallet.Repository.Payload.Response.Admin;
using SWallet.Repository.Payload.Response.Area;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Services.Interfaces;
using SWallet.Repository.Utils;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;
using BCryptNet = BCrypt.Net.BCrypt;

namespace SWallet.Repository.Services.Implements
{
    public class AdminService : BaseService<AdminService>, IAdminService
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly Mapper mapper;
        private readonly string ACCOUNT_FOLDER_NAME = "accounts";

        public AdminService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<AdminService> logger, ICloudinaryService cloudinaryService) : base(unitOfWork, logger)
        {
            _cloudinaryService = cloudinaryService;
            var config = new MapperConfiguration(cfg
               =>
            {
                cfg.CreateMap<Account, CreateAdminModel>()
                    .ReverseMap()
                    .ForMember(p => p.Id, opt => opt.MapFrom(src => Ulid.NewUlid()))
                    .ForMember(p => p.Role, opt => opt.MapFrom(src => Role.Admin))
                    .ForMember(p => p.Password, opt => opt.MapFrom(src => BCryptNet.HashPassword(src.Password)))
                    .ForMember(p => p.IsVerify, opt => opt.MapFrom(src => true))
                    .ForMember(p => p.DateCreated, opt => opt.MapFrom(src => DateTime.Now))
                    .ForMember(p => p.DateUpdated, opt => opt.MapFrom(src => DateTime.Now))
                    .ForMember(p => p.DateVerified, opt => opt.MapFrom(src => DateTime.Now))
                    .ForMember(p => p.Status, opt => opt.MapFrom(src => true));
            });
            mapper = new Mapper(config);
        }

        public async Task<AdminResponse> Add(CreateAdminModel admin)
        {
            var existingAccount = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
               predicate: b => b.UserName == admin.UserName);
            if (existingAccount != null)
            {
                throw new ApiException("Username already exists", 400, "BAD_REQUEST");
            }
            var imageUri = string.Empty;
            if (admin.Avatar != null && admin.Avatar.Length > 0)
            {
                var uploadResult = await _cloudinaryService.UploadImageAsync(admin.Avatar);
                imageUri = uploadResult.SecureUrl.AbsoluteUri;
            }
            Account account = mapper.Map<Account>(admin);
            account.Avatar = imageUri;
            await _unitOfWork.GetRepository<Account>().InsertAsync(account);
            await _unitOfWork.CommitAsync();

            

            var newAdmin = new Admin
            {
                Id = Ulid.NewUlid().ToString(),
                AccountId = account.Id,
                FullName = admin.FullName,
                State = admin.State,
                DateCreated = DateTime.Now,
                Status = true
            };

            await _unitOfWork.GetRepository<Admin>().InsertAsync(newAdmin);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;

            if (isSuccess)
            {
                return new AdminResponse
                {
                    Id = newAdmin.Id,
                    AccountId = newAdmin.AccountId,
                    FullName = newAdmin.FullName,
                    State = newAdmin.State,
                    DateCreated = newAdmin.DateCreated,
                    DateUpdated = newAdmin.DateUpdated,
                    UserName = admin.UserName,
                    Phone = admin.Phone,
                    Email = admin.Email,
                    Description = admin.Description,
                    Status = newAdmin.Status,
                    Avatar = imageUri,
                    FileName = !string.IsNullOrEmpty(imageUri)
                        ? imageUri.Split('/')[imageUri.Split('/').Length - 1]
                        : "default_avatar.jpg"

                };
            }
            throw new ApiException("Create Brand Fail", 400, "BAD_REQUEST");
        }


        public async Task<IPaginate<AdminResponse>> GetAll(string? searchName, int page, int size)
        {
            Expression<Func<Admin, bool>> filterQuery;
            if (string.IsNullOrEmpty(searchName))
            {
                filterQuery = p => true;
            }
            else
            {
                filterQuery = p => p.FullName.Contains(searchName);
            }

            var areas = await _unitOfWork.GetRepository<Admin>().GetPagingListAsync(
                selector: x => new AdminResponse
                {
                    Id = x.Id,
                    AccountId = x.AccountId,
                    FullName = x.FullName,
                    State = x.State,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    UserName = x.Account.UserName,
                    Phone = x.Account.Phone,
                    Email = x.Account.Email,
                    Description = x.Account.Description,
                    Status = x.Status,
                    Avatar = x.Account.Avatar,
                    FileName = "abc"

                },
                predicate: filterQuery,
                page: page,
                size: size);
            return areas;
        }

        public async Task<AdminResponse> GetById(string id)
        {
            var area = await _unitOfWork.GetRepository<Admin>().SingleOrDefaultAsync(
                selector: x => new AdminResponse
                {
                    Id = x.Id,
                    AccountId = x.AccountId,
                    FullName = x.FullName,
                    State = x.State,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    UserName = x.Account.UserName,
                    Phone = x.Account.Phone,
                    Email = x.Account.Email,
                    Description = x.Account.Description,
                    Status = x.Status,
                    Avatar = x.Account.Avatar,
                    FileName = "abc"
                },
                predicate: x => x.Id == id);
            return area;
        }

        public async Task<AdminResponse> Update(string id, UpdateAdminModel update)
        {
            var updateAdmin = await _unitOfWork.GetRepository<Admin>().SingleOrDefaultAsync(predicate: x => x.Id == id,
                include: x => x.Include(a => a.Account));
            if (updateAdmin == null)
            {
                throw new ApiException("Admin not found", 404, "NOT_FOUND");
            }
            if (update.Avatar!= null && update.Avatar.Length > 0)
            {

                var f = await _cloudinaryService.UploadImageAsync(update.Avatar);

            }
            updateAdmin.DateUpdated = DateTime.Now;
            updateAdmin.FullName = update.FullName;
            updateAdmin.State = update.State;

             _unitOfWork.GetRepository<Admin>().UpdateAsync(updateAdmin);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;
            if (isSuccess)
            {
                return new AdminResponse
                {
                    Id = updateAdmin.Id,
                    AccountId = updateAdmin.AccountId,
                    FullName = updateAdmin.FullName,
                    State = updateAdmin.State,
                    DateCreated = updateAdmin.DateCreated,
                    DateUpdated = updateAdmin.DateUpdated,
                    UserName = updateAdmin.Account.UserName,
                    Phone = updateAdmin.Account.Phone,
                    Email = updateAdmin.Account.Email,
                    Description = updateAdmin.Account.Description,
                    Status = updateAdmin.Status,
                    Avatar = updateAdmin.Account.Avatar,
                    FileName = "abc"
                };
            }
            throw new ApiException("Update Admin Fail", 400, "BAD_REQUEST");
        }
    }
}