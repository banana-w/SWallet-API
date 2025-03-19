using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using SWallet.Domain.Models;
using SWallet.Repository.Implement;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Services.Implements;
using SWallet.Repository.Services.Interfaces;
using System.Text;
using VNPAY.NET;

namespace SWallet_API.Extentions
{
    public static class DependencyServices
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<SwalletDbContext>(options =>
                             options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
            services.AddScoped<IUnitOfWork<SwalletDbContext>, UnitOfWork<SwalletDbContext>>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<IVoucherTypeService, VoucherTypeService>();
            services.AddScoped<IVoucherService, VoucherService>();
            services.AddScoped<IAreaService, AreaService>();
            services.AddScoped<ICampusService, CampusService>();
            services.AddScoped<ILecturerService, LecturerService>();
            services.AddScoped<IInvitationService, InvitationService>();
            services.AddScoped<ICampaignTypeService, CampaignTypeService>();
            services.AddScoped<ICampaignDetailService, CampaignDetailService>();
            services.AddScoped<ICampaignService, CampaignService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IActivityService, ActivityService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IVoucherItemService, VoucherItemService>();
            services.AddScoped<IVnpay, Vnpay>();
            services.AddScoped<IPointPackageService, PointPackageService>();

            return services;
        }
        public static void AddRedisServices(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConfig = configuration.GetValue<string>("Redis:ConnectionString");
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfig));
            services.AddScoped<IRedisService, RedisService>();
        }

        public static IServiceCollection AddConfigSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(option =>
            {
                option.DescribeAllParametersInCamelCase();
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type=ReferenceType.SecurityScheme,
                                    Id="Bearer"
                                }
                            },
                            new string[]{}
                        }
                });
            });
            return services;
        }

        public static IServiceCollection AddJwtValidation(this IServiceCollection services, IConfiguration config)
        {
            // JWT authentication service
            _ = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(s: config["Jwt:Key"]))
                };
            });
            return services;
        }
    }
}
