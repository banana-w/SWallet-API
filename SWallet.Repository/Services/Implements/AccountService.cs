using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SWallet.Domain.Models;
using SWallet.Repository.Enums;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Account;
using SWallet.Repository.Payload.Request.Login;
using SWallet.Repository.Payload.Response.Account;
using SWallet.Repository.Payload.Response.Login;
using SWallet.Repository.Services.Interfaces;
using SWallet.Repository.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCryptNet = BCrypt.Net.BCrypt;

namespace SWallet.Repository.Services.Implements
{
    public class AccountService : BaseService<AccountService>, IAccountService
    {
        private readonly Mapper mapper;
        private readonly IEmailService _emailService;
        private readonly ICloudinaryService _cloudinaryService;

        public AccountService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<AccountService> logger,
            IEmailService emailService, ICloudinaryService cloudinaryService) : base(unitOfWork, logger)
        {
            var config = new MapperConfiguration(cfg
                =>
            {
                cfg.CreateMap<Account, AccountResponse>()
                .ForMember(a => a.RoleId, opt => opt.MapFrom(src => src.Role))
                .ForMember(a => a.RoleName, opt => opt.MapFrom(src => EnumExtensions.GetDisplayNameFromValue<Role>((int)src.Role)))
                .ForMember(a => a.UserId, opt => opt.MapFrom((src, dest) =>
                {
                    if (src.Role != null)
                    {
                        return src.Role switch
                        {
                            (int)Role.Admin => src.Admins?.FirstOrDefault()?.Id,
                            (int)Role.Staff => src.Staffs?.FirstOrDefault()?.Id,
                            (int)Role.Brand => src.Brands?.FirstOrDefault()?.Id,
                            (int)Role.Store => src.Stores?.FirstOrDefault()?.Id,
                            (int)Role.Student => src.Students?.FirstOrDefault()?.Id,
                            _ => null,
                        };
                    }
                    return null;
                }))
                .ForMember(a => a.Name, opt => opt.MapFrom((src, dest) =>
                {
                    if (src.Role != null)
                    {
                        return src.Role switch
                        {
                            (int)Role.Admin => src.Admins?.FirstOrDefault()?.FullName,
                            (int)Role.Staff => src.Staffs?.FirstOrDefault()?.FullName,
                            (int)Role.Brand => src.Brands?.FirstOrDefault()?.BrandName,
                            (int)Role.Store => src.Stores?.FirstOrDefault()?.StoreName,
                            (int)Role.Student => src.Students?.FirstOrDefault()?.FullName,
                            _ => null,
                        };
                    }
                    return null;
                }))
                .ForMember(a => a.State, opt => opt.MapFrom((src, dest) =>
                {
                    if (src.Role != null)
                    {
                        return src.Role switch
                        {
                            (int)Role.Student => src.Students?.FirstOrDefault()?.State,
                            _ => null
                        };
                    }
                    return null;
                }))
                .ReverseMap();


                cfg.CreateMap<Student, CreateStudentAccount>()
           .ReverseMap()
           .ForMember(s => s.Id, opt => opt.MapFrom(src => Ulid.NewUlid()))
           .ForMember(s => s.TotalIncome, opt => opt.MapFrom(src => 0))
           .ForMember(s => s.TotalSpending, opt => opt.MapFrom(src => 0))
           .ForMember(s => s.DateCreated, opt => opt.MapFrom(src => DateTime.Now))
           .ForMember(s => s.DateUpdated, opt => opt.MapFrom(src => DateTime.Now))
           .ForMember(s => s.State, opt => opt.MapFrom(src => StudentState.Pending))
           .ForMember(s => s.Status, opt => opt.MapFrom(src => true));
                cfg.CreateMap<Account, CreateStudentAccount>()
           .ReverseMap()
           .ForMember(t => t.Id, opt => opt.MapFrom(src => Ulid.NewUlid()))
           .ForMember(t => t.Role, opt => opt.MapFrom(src => Role.Student))
           .ForMember(t => t.Password, opt => opt.MapFrom(src => BCryptNet.HashPassword(src.Password))) // HashPassword when create account
           .ForMember(t => t.IsVerify, opt => opt.MapFrom(src => true))
           .ForMember(t => t.DateCreated, opt => opt.MapFrom(src => DateTime.Now))
           .ForMember(t => t.DateUpdated, opt => opt.MapFrom(src => DateTime.Now))
           .ForMember(t => t.Status, opt => opt.MapFrom(src => true));
            });
            mapper ??= new Mapper(config);
            _emailService = emailService;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<AccountResponse> CreateStudentAccount(CreateStudentAccount accountCreation)
        {
            Account account = mapper.Map<Account>(accountCreation);
            //insert account
            await _unitOfWork.GetRepository<Account>().InsertAsync(account);

            Student student = mapper.Map<Student>(accountCreation);

            student.AccountId = account.Id;
            //upload font-back student card images
            if (accountCreation.StudentCardFront != null && accountCreation.StudentCardFront.Length > 0)
            {
                var uploadResult = _cloudinaryService.UploadImageAsync(accountCreation.StudentCardFront);
                student.StudentCardFront = uploadResult.Result.SecureUrl.AbsoluteUri;
                student.FileNameFront = uploadResult.Result.PublicId;
            }
            if (accountCreation.StudentCardBack != null && accountCreation.StudentCardBack.Length > 0)
            {
                var uploadResult = _cloudinaryService.UploadImageAsync(accountCreation.StudentCardBack);
                student.StudentCardBack = uploadResult.Result.SecureUrl.AbsoluteUri;
                student.FileNameBack = uploadResult.Result.PublicId;
            }
            //insert student
            await _unitOfWork.GetRepository<Student>().InsertAsync(student);

            bool issuccessfull = await _unitOfWork.CommitAsync() > 0;
            if (issuccessfull)
            {
                _emailService.SendEmailStudentRegister(account.Email);
                return mapper.Map<AccountResponse>(account);
            }
            else throw new ApiException("Account Creation Fail", 400, "BAD_REQUEST");
        }

        public async Task<AccountResponse> GetAccountById(string id)
        {
            Account account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            return mapper.Map<AccountResponse>(account);
        }


    }
}
