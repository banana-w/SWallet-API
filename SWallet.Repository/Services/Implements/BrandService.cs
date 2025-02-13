using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Repository.Enums;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCryptNet = BCrypt.Net.BCrypt;

namespace SWallet.Repository.Services.Implements
{
    public class BrandService : BaseService<BrandService>, IBrandService
    {
        private readonly Mapper mapper;

        public BrandService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<BrandService> logger, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, httpContextAccessor)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Brand, BrandResponse>()
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
    .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
    .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.BrandName))
    .ForMember(dest => dest.Acronym, opt => opt.MapFrom(src => src.Acronym))
    .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
    .ForMember(dest => dest.CoverPhoto, opt => opt.MapFrom(src => src.CoverPhoto))
    .ForMember(dest => dest.CoverFileName, opt => opt.MapFrom(src => src.CoverFileName))
    .ForMember(dest => dest.Link, opt => opt.MapFrom(src => src.Link))
    .ForMember(dest => dest.OpeningHours, opt => opt.MapFrom(src => src.OpeningHours))
    .ForMember(dest => dest.ClosingHours, opt => opt.MapFrom(src => src.ClosingHours))
    .ForMember(dest => dest.TotalIncome, opt => opt.MapFrom(src => src.TotalIncome))
    .ForMember(dest => dest.TotalSpending, opt => opt.MapFrom(src => src.TotalSpending))
    .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => src.DateCreated))
    .ForMember(dest => dest.DateUpdated, opt => opt.MapFrom(src => src.DateUpdated))
    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
    .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));


                cfg.CreateMap<Brand, CreateBrandModel>()
.ReverseMap()
.ForMember(p => p.Id, opt => opt.MapFrom(src => Ulid.NewUlid()))
.ForMember(p => p.TotalIncome, opt => opt.MapFrom(src => 0))
.ForMember(p => p.TotalSpending, opt => opt.MapFrom(src => 0))
.ForMember(p => p.DateCreated, opt => opt.MapFrom(src => DateTime.Now))
.ForMember(p => p.DateUpdated, opt => opt.MapFrom(src => DateTime.Now))
.ForMember(p => p.Status, opt => opt.MapFrom(src => true));
                cfg.CreateMap<Account, CreateBrandModel>()
                .ReverseMap()
                .ForMember(p => p.Id, opt => opt.MapFrom(src => Ulid.NewUlid()))
                .ForMember(p => p.Role, opt => opt.MapFrom(src => Role.Brand))
                .ForMember(p => p.Password, opt => opt.MapFrom(src => BCryptNet.HashPassword(src.Password)))
                .ForMember(p => p.IsVerify, opt => opt.MapFrom(src => true))
                .ForMember(p => p.DateCreated, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(p => p.DateUpdated, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(p => p.DateVerified, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(p => p.Status, opt => opt.MapFrom(src => true));
            });
            mapper = new Mapper(config);
            //this.brandRepository = brandRepository;
            //this.fireBaseService = fireBaseService;
            //this.accountRepository = accountRepository;
            //this.campaignService = campaignService;
            //this.storeService = storeService;
            //this.voucherService = voucherService;
            //this.emailService = emailService;
            //this.transactionService = transactionService;
        }

        public Task<BrandResponse> Add(CreateBrandModel creation)
        {
            Account account = mapper.Map<Account>(creation);

            //Upload logo
            if (creation.Logo != null && creation.Logo.Length > 0)
            {
                FireBaseFile f = await fireBaseService.UploadFileAsync(creation.Logo, ACCOUNT_FOLDER_NAME);
                account.Avatar = f.URL;
                account.FileName = f.FileName;
            }

            account = accountRepository.Add(account);
            emailService.SendEmailBrandRegister(account.Email);
            Brand brand = mapper.Map<Brand>(creation);
            brand.AccountId = account.Id;

            //Upload cover photo
            if (creation.CoverPhoto != null && creation.CoverPhoto.Length > 0)
            {
                FireBaseFile f = await fireBaseService.UploadFileAsync(creation.CoverPhoto, FOLDER_NAME);
                brand.CoverPhoto = f.URL;
                brand.CoverFileName = f.FileName;
            }

            return mapper.Map<BrandExtraModel>(brandRepository.Add(brand));
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}
