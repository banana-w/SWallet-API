using AutoMapper;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Campaign;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Payload.Response.Campaign;
using SWallet.Repository.Services.Interfaces;
using System.Linq.Expressions;


namespace SWallet.Repository.Services.Implements
{
    public class CampaignTypeService : BaseService<CampaignTypeService>, ICampaignTypeService
    {

        private readonly Mapper mapper;
        private readonly ICloudinaryService _cloudinaryService;
        public CampaignTypeService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<CampaignTypeService> logger, ICloudinaryService cloudinaryService) : base(unitOfWork, logger)
        {
            _cloudinaryService = cloudinaryService;
            mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CampaignType, CampaignTypeResponse>();
                cfg.CreateMap<CampaignTypeResponse, CampaignType>();
            }));
        }

        public async Task<CampaignTypeResponse> CreateCampaignType(CreateCampaignTypeModel type)
        {
            var imageUri = string.Empty;
            if (type.Image != null && type.Image.Length > 0)
            {
                var uploadResult = await _cloudinaryService.UploadImageAsync(type.Image);
                imageUri = uploadResult.SecureUrl.AbsoluteUri;
            }

            var campaignType = new CampaignType
            {
                Id = Ulid.NewUlid().ToString(),
                TypeName = type.TypeName,
                Description = type.Description,
                Image = imageUri,
                FileName = !string.IsNullOrEmpty(imageUri)
                          ? imageUri.Split('/')[imageUri.Split('/').Length - 1]
                          : "default_cover.jpg",
                Duration = type.Duration,
                Coin = type.Coin,
                DateCreated = DateTime.Now,   
                DateUpdated = DateTime.Now,
                State = type.State,
                Status = true
            };
            await _unitOfWork.GetRepository<CampaignType>().InsertAsync(campaignType);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;

            if (isSuccess)
            {
                return new CampaignTypeResponse
                {
                    Id = campaignType.Id,
                    TypeName = campaignType.TypeName,
                    Description = campaignType.Description,
                    Image = campaignType.Image,
                    FileName = campaignType.FileName,
                    Duration = campaignType.Duration,
                    Coin = campaignType.Coin,
                    DateCreated = campaignType.DateCreated,
                    DateUpdated = campaignType.DateUpdated,
                    State = campaignType.State,
                    Status = campaignType.Status,
                    NumberOfCampaign = campaignType.Campaigns.Count
                };
            }
            throw new ApiException("Create CampaignType Fail", 400, "BAD_REQUEST");
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IPaginate<CampaignTypeResponse>> GetCampaignType(string searchName, int page, int size)
        {
            Expression<Func<CampaignType, bool>> filterQuery;
            if (string.IsNullOrEmpty(searchName))
            {
                filterQuery = p => true;
            }
            else
            {
                filterQuery = p => p.TypeName.Contains(searchName);
            }

            var areas = await _unitOfWork.GetRepository<CampaignType>().GetPagingListAsync(
                selector: x => new CampaignTypeResponse
                {
                    Id = x.Id,
                    TypeName = x.TypeName,
                    Description = x.Description,
                    Image = x.Image,
                    FileName = x.FileName,
                    Duration = x.Duration,
                    Coin = x.Coin,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    State = x.State,
                    Status = x.Status,
                    NumberOfCampaign = x.Campaigns.Count

                },
                predicate: filterQuery,
                page: page,
                size: size);
            return areas;
        }

        public async Task<CampaignTypeResponse> GetCampaignTypeById(string id)
        {
            var area = await _unitOfWork.GetRepository<CampaignType>().SingleOrDefaultAsync(
                selector: x => new CampaignTypeResponse
                {
                    Id = x.Id,
                    TypeName = x.TypeName,
                    Description = x.Description,
                    Image = x.Image,
                    Duration = x.Duration,
                    Coin = x.Coin,
                    FileName = x.FileName,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    State = x.State,
                    Status = x.Status
                },
                predicate: x => x.Id == id);
            return area;
        }

        public async Task<CampaignTypeResponse> UpdateCampaignType(string id, UpdateCampaignTypeModel type)
        {
            var updateCampaignType = await _unitOfWork.GetRepository<CampaignType>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (updateCampaignType == null)
            {
                throw new ApiException("CampaignType not found", 404, "NOT_FOUND");
            }
            if (type.Image!= null && type.Image.Length > 0)
            {

                var f = await _cloudinaryService.UploadImageAsync(type.Image);

            }

            updateCampaignType.TypeName = type.TypeName;
            updateCampaignType.Duration = type.Duration;
            updateCampaignType.Coin = type.Coin;
            updateCampaignType.Description = type.Description;
            updateCampaignType.State = type.State;
            updateCampaignType.DateUpdated = DateTime.Now;
            _unitOfWork.GetRepository<CampaignType>().UpdateAsync(updateCampaignType);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;
            if (isSuccess)
            {
                return new CampaignTypeResponse
                {
                    Id = updateCampaignType.Id,
                    TypeName = updateCampaignType.TypeName,
                    Description = updateCampaignType.Description,
                    Image = updateCampaignType.Image,
                    Duration = updateCampaignType.Duration,
                    Coin = updateCampaignType.Coin,
                    FileName = updateCampaignType.FileName,
                    DateCreated = updateCampaignType.DateCreated,
                    DateUpdated = updateCampaignType.DateUpdated,
                    State = updateCampaignType.State,
                    Status = updateCampaignType.Status
                };
            }
            throw new ApiException("Update CampaignType Fail", 400, "BAD_REQUEST");
        }
    }
}
