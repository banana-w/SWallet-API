using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Repository.Enums;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.Request.Login;
using SWallet.Repository.Payload.Response.Account;
using SWallet.Repository.Payload.Response.Login;
using SWallet.Repository.Services.Interfaces;
using SWallet.Repository.Utils;
using BCryptNet = BCrypt.Net.BCrypt;


namespace SWallet.Repository.Services.Implements
{
    public class AuthenticationService : BaseService<AuthenticationService>, IAuthenticationService
    {
        private readonly IJwtService _jwtService;
        private readonly Mapper mapper;

        public AuthenticationService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<AuthenticationService> logger, IJwtService jwtService) : base(unitOfWork, logger)
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

        }

        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            Account account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.UserName == loginRequest.UserName);
            if (account == null || !BCryptNet.Verify(loginRequest.Password, account.Password))
            {
                return null;
            }
            var acc = mapper.Map<AccountResponse>(account);
            return new LoginResponse
            {
                Token = _jwtService.GenerateJwtToken(acc),
                Role = acc.RoleName,
                AccountId = acc.Id
            };
        }
    }
}
