using AutoMapper;
using CloudinaryDotNet.Core;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Campaign;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Payload.Response.Campaign;
using SWallet.Repository.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace SWallet.Repository.Services.Implements
{
    public class CampaignService : BaseService<CampaignService>, ICampaignService
    {
        private readonly Mapper mapper;
        private readonly ICloudinaryService _cloudinaryService;

        public CampaignService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<CampaignService> logger, ICloudinaryService cloudinaryService) : base(unitOfWork, logger)
        {
            _cloudinaryService = cloudinaryService;

            var config = new MapperConfiguration(cfg
                =>
            {
                cfg.CreateMap<Campaign, CreateCampaignModel>()
                .ReverseMap()
                .ForMember(c => c.Id, opt => opt.MapFrom(src => Ulid.NewUlid()))
                .ForMember(c => c.Duration, opt => opt.MapFrom(src
                    => ((DateOnly)src.EndOn).DayNumber - ((DateOnly)src.StartOn).DayNumber + 1))
                .ForMember(c => c.TotalSpending, opt => opt.MapFrom(src => 0))
                .ForMember(c => c.DateCreated, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(c => c.DateUpdated, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(c => c.Status, opt => opt.MapFrom(src => true));
            });

        }

        public async Task<CampaignResponse> CreateCampaign(CreateCampaignModel campaign)
        {
            var imageUri = string.Empty;
            if (campaign.Image != null && campaign.Image.Length > 0)
            {
                var uploadResult = await _cloudinaryService.UploadImageAsync(campaign.Image);
                imageUri = uploadResult.SecureUrl.AbsoluteUri;
            }

            Campaign newCampaign = new Campaign
            {
                Id = Ulid.NewUlid().ToString(),
                BrandId = campaign.BrandId,
                TypeId = campaign.TypeId,
                CampaignName = campaign.CampaignName,
                Image = imageUri,
                ImageName = !string.IsNullOrEmpty(imageUri)
                          ? imageUri.Split('/')[imageUri.Split('/').Length - 1]
                          : "default_cover.jpg",
                Condition = campaign.Condition,
                Link = campaign.Link,
                StartOn = campaign.StartOn,
                EndOn = campaign.EndOn,
                File = "abc",
                FileName = "abc",
                Duration = ((DateOnly)campaign.EndOn).DayNumber - ((DateOnly)campaign.StartOn).DayNumber + 1,
                TotalIncome = campaign.TotalIncome,
                TotalSpending = 0,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Description = campaign.Description,
                Status = true
            };

            newCampaign.CampaignDetails = newCampaign.CampaignDetails.Select(c => new CampaignDetail
            {

                Id = Ulid.NewUlid().ToString(),
                VoucherId = c.VoucherId,
                CampaignId = newCampaign.Id,
                Description = c.Description,
                Quantity = c.Quantity,
                Price = c.Price,
                Rate = c.Rate,
                FromIndex = c.FromIndex,
                ToIndex = c.ToIndex,
                State = c.State,
                Status = c.Status,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now
            }).ToList();

            newCampaign.CampaignStores = newCampaign.CampaignStores.Select(c => new CampaignStore
            {
                Id = Ulid.NewUlid().ToString(),
                CampaignId = newCampaign.Id,
                StoreId = c.StoreId,
            }).ToList();

            newCampaign.CampaignCampuses = newCampaign.CampaignCampuses.Select(c => new CampaignCampus
            {
                Id = Ulid.NewUlid().ToString(),
                CampaignId = newCampaign.Id,
                CampusId = c.CampusId
            }).ToList();

            await _unitOfWork.GetRepository<Campaign>().InsertAsync(newCampaign);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;

            if (isSuccess)
            {
                return new CampaignResponse
                {
                    Id = newCampaign.Id,
                    BrandId = newCampaign.BrandId,
                    TypeId = newCampaign.TypeId,
                    CampaignName = newCampaign.CampaignName,
                    Image = newCampaign.Image,
                    ImageName = newCampaign.ImageName,
                    Condition = newCampaign.Condition,
                    Link = newCampaign.Link,
                    StartOn = newCampaign.StartOn,
                    EndOn = newCampaign.EndOn,
                    Duration = newCampaign.Duration,
                    TotalIncome = newCampaign.TotalIncome,
                    TotalSpending = newCampaign.TotalSpending,
                    DateCreated = newCampaign.DateCreated,
                    DateUpdated = newCampaign.DateUpdated,
                    Description = newCampaign.Description,
                    Status = newCampaign.Status
                };
            }
            throw new ApiException("Create Campaign Fail", 400, "BAD_REQUEST");

        }


        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<CampaignResponse> GetCampaignById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginate<CampaignResponse>> GetCampaigns(string searchName, int page, int size)
        {
            throw new NotImplementedException();
        }

        public Task<CampaignResponse> UpdateCampaign(string id, UpdateCampaignModel campaign)
        {
            throw new NotImplementedException();
        }
    }
}
