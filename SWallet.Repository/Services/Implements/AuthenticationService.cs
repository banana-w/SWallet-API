using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Repository.Enums;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.Request.Authentication;
using SWallet.Repository.Payload.Response.Account;
using SWallet.Repository.Payload.Response.Authentication;
using SWallet.Repository.Services.Interfaces;
using SWallet.Repository.Utils;
using System.Diagnostics;
using BCryptNet = BCrypt.Net.BCrypt;


namespace SWallet.Repository.Services.Implements
{
    public class AuthenticationService : BaseService<AuthenticationService>, IAuthenticationService
    {
        private readonly IJwtService _jwtService;
        private readonly IRedisService redisService;
        private readonly IChallengeService _challengeService;
        private readonly Mapper mapper;

        public AuthenticationService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<AuthenticationService> logger, IJwtService jwtService, 
            IRedisService redisService, IChallengeService challengeService) : base(unitOfWork, logger)
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
                            (int)Role.Lecturer => src.Lecturers?.FirstOrDefault()?.Id,
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
                            (int)Role.Lecturer => src.Lecturers?.FirstOrDefault()?.FullName,
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
            });
            mapper ??= new Mapper(config);
            _jwtService = jwtService;
            this.redisService = redisService;
            _challengeService = challengeService;
        }

        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            Account account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.UserName == loginRequest.UserName,
                include: x => x.Include(x => x.Brands)
                                .Include(x => x.Students));

            if (account == null || !BCryptNet.Verify(loginRequest.Password, account.Password))
            {
                return null;
            }
            var acc = mapper.Map<AccountResponse>(account);
            Tuple<string, string> guidClaim = null;
            switch (account.Role)
            {              
                case (int)Role.Brand:
                    guidClaim = new Tuple<string, string>("brandId", account.Brands?.FirstOrDefault()?.Id);
                    break;
                case (int)Role.Student:
                    guidClaim = new Tuple<string, string>("studentId", account.Students?.FirstOrDefault()?.Id);
                    break;
            }
            return new LoginResponse
            {
                Token = _jwtService.GenerateJwtToken(acc, guidClaim),
                Role = acc.RoleName,
                AccountId = acc.Id,
                IsVerify = acc.IsVerify,
                Email = acc.Email,
            };
        }

        public async Task<bool> VerifyEmail(string email, string userInput)
        {
            return await redisService.VerifyCodeAsync(email, userInput);
        }
        public async Task<bool> VerifyStudent(string email, string userInput, string studentId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {

                var student = await _unitOfWork.GetRepository<Student>().SingleOrDefaultAsync(predicate: x => x.Id == studentId);

                if (student != null)
                {
                    var result = await redisService.VerifyCodeAsync(email, userInput);
                    if (result)
                    {
                        student.State = (int)StudentState.Active;
                        student.StudentEmail = email;
                        _unitOfWork.GetRepository<Student>().UpdateAsync(student);

                        await _challengeService.AssignAllChallengesToStudent(studentId);

                        var reuslt = await _unitOfWork.CommitAsync() > 0;

                        await _unitOfWork.CommitTransactionAsync();
                        if (result)
                            return true;
                    }
                }
                return false;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
        
        public async Task<bool> VerifyBrand(string email, string userInput, string brandId)
        {
            var brand = await _unitOfWork.GetRepository<Brand>().SingleOrDefaultAsync(predicate: x => x.Id == brandId);

            if (brand != null)
            {
                var result = await redisService.VerifyCodeAsync(email, userInput);
                if (result)
                {
                    brand.State = true;
                    _unitOfWork.GetRepository<Brand>().UpdateAsync(brand);
                    return await _unitOfWork.CommitAsync() > 0;
                }
            }
            return false;
        }

        public async Task<bool> VerifyAccount(string email, string userInput, string accountId)
        {
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.Id == accountId);
            if (account != null)
            {
                var result = await redisService.VerifyCodeAsync(email, userInput);
                if (result)
                {
                    account.IsVerify = true;
                    account.DateVerified = DateTime.UtcNow;
                    _unitOfWork.GetRepository<Account>().UpdateAsync(account);
                    return await _unitOfWork.CommitAsync() > 0;
                }
            }
            return false;
        }
    }
}
