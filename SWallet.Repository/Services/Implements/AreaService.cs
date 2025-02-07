using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Area;
using SWallet.Repository.Payload.Response.Area;
using SWallet.Repository.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SWallet.Repository.Services.Implements
{
    public class AreaService : BaseService<AreaService>, IAreaService
    {
        private readonly ICloudinaryService _cloudinaryService;
        public AreaService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<AreaService> logger, ICloudinaryService cloudinaryService) : base(unitOfWork, logger)
        {
            _cloudinaryService = cloudinaryService;
        }

        public async Task<IPaginate<AreaResponse>> GetAreas(string searchName, int page, int size)
        {
            var areas = await _unitOfWork.GetRepository<Area>().GetPagingListAsync(
                selector: x => new AreaResponse
                {
                    Id = x.Id,
                    AreaName = x.AreaName,
                    Image = x.Image,
                    FileName = x.FileName,
                    Address = x.Address,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    State = x.State,
                    Status = x.Status
                },
                predicate: x => x.AreaName == searchName, 
                page: page, 
                size: size);
            return areas;
        }

        public async Task<AreaResponse> GetAreaById(string id)
        {
            var area = await _unitOfWork.GetRepository<Area>().SingleOrDefaultAsync(
                selector: x => new AreaResponse
                {
                    Id = x.Id,
                    AreaName = x.AreaName,
                    Image = x.Image,
                    FileName = x.FileName,
                    Address = x.Address,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    State = x.State,
                    Status = x.Status
                },
                predicate: x => x.Id == id);
            return area;
        }

        public async Task<AreaResponse> CreateArea(AreaRequest areaRequest)
        {
            var imageUri = string.Empty;
            if (areaRequest.Image != null && areaRequest.Image.Length > 0)
            {
                var uploadResult = await _cloudinaryService.UploadImageAsync(areaRequest.Image);
                imageUri = uploadResult.SecureUrl.AbsoluteUri;
            }

                var newArea = new Area
            {
                AreaName = areaRequest.AreaName,
                Image = imageUri,
                FileName = areaRequest.Image.FileName,
                Address = areaRequest.Address,
                Description = areaRequest.Description,
                State = areaRequest.State,
                Status = true
            };
            await _unitOfWork.GetRepository<Area>().InsertAsync(newArea);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;
            
            if (isSuccess)
            {
                return new AreaResponse
                {
                    Id = newArea.Id,
                    AreaName = newArea.AreaName,
                    Image = newArea.Image,
                    FileName = newArea.FileName,
                    Address = newArea.Address,
                    DateCreated = newArea.DateCreated,
                    DateUpdated = newArea.DateUpdated,
                    Description = newArea.Description,
                    State = newArea.State,
                    Status = newArea.Status
                };
            }
            throw new ApiException("Create Area Fail", 400, "BAD_REQUEST");
        }

        public async Task<AreaResponse> UpdateArea(string id, AreaRequest areaRequest)
        {
            var area = await _unitOfWork.GetRepository<Area>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (area == null)
            {
                throw new ApiException("Area not found", 404, "NOT_FOUND");
            }
            if (areaRequest.Image != null && areaRequest.Image.Length > 0)
            {
                // Remove image
                //await _cloudinaryService

                //Upload new image update
                var f = await _cloudinaryService.UploadImageAsync(areaRequest.Image);
                area.Image = f.SecureUrl.AbsoluteUri;
                area.FileName = areaRequest.Image.FileName;
            }
            area.AreaName = areaRequest.AreaName;
            area.Address = areaRequest.Address;
            area.Description = areaRequest.Description;
            area.State = areaRequest.State;
            area.DateUpdated = DateTime.Now;
             _unitOfWork.GetRepository<Area>().UpdateAsync(area);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;
            if (isSuccess)
            {
                return new AreaResponse
                {
                    Id = area.Id,
                    AreaName = area.AreaName,
                    Image = area.Image,
                    FileName = area.FileName,
                    Address = area.Address,
                    DateCreated = area.DateCreated,
                    DateUpdated = area.DateUpdated,
                    Description = area.Description,
                    State = area.State,
                    Status = area.Status
                };
            }
            throw new ApiException("Update Area Fail", 400, "BAD_REQUEST");
        }
    }
}
