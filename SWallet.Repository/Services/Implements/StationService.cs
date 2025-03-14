using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Station;
using SWallet.Repository.Payload.Response.Station;
using SWallet.Repository.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace SWallet.Repository.Services.Implements
{
    public class StationService : BaseService<StationService>, IStationService
    {
        private readonly Mapper mapper;
        private readonly ICloudinaryService _cloudinaryService;
        public StationService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<StationService> logger, IMapper mapper, ICloudinaryService cloudinaryService) : base(unitOfWork, logger)
        {
            _cloudinaryService = cloudinaryService;
        }
      

        public async Task<StationResponse> CreateStation(CreateStationModel station)
        {
            var imageUri = string.Empty;
            if (station.Image != null && station.Image.Length > 0)
            {
                var uploadResult = await _cloudinaryService.UploadImageAsync(station.Image);
                imageUri = uploadResult.SecureUrl.AbsoluteUri;
            }

            var newStation = new Station
            {
                Id = Ulid.NewUlid().ToString(),
                StationName = station.StationName,
                Address = station.Address,
                Image = imageUri,
                FileName = "abc",
                OpeningHours = station.OpeningHours,
                ClosingHours = station.ClosingHours,
                Phone = station.Phone,
                Email = station.Email,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Description = station.Description,
                State = station.State,
                Status = true
            };
            await _unitOfWork.GetRepository<Station>().InsertAsync(newStation);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccess)
            {
                throw new ApiException("Create Station Fail", 400, "BAD_REQUEST");
            }

            var result = await _unitOfWork.GetRepository<Station>().SingleOrDefaultAsync(
                selector: x => new StationResponse
                {
                    Id = x.Id,
                    StationName = x.StationName,
                    Address = x.Address,
                    Image = x.Image,
                    FileName = x.FileName,
                    OpeningHours = x.OpeningHours,
                    ClosingHours = x.ClosingHours,
                    Phone = x.Phone,
                    Email = x.Email,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    State = x.State.ToString(),
                    Status = x.Status
                },
                predicate: x => x.Id == newStation.Id);

            return result;
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IPaginate<StationResponse>> GetStation(string searchName, int page, int size)
        {
            Expression<Func<Station, bool>> filterQuery;
            if (string.IsNullOrEmpty(searchName))
            {
                filterQuery = p => true;
            }
            else
            {
                filterQuery = p => p.StationName.Contains(searchName);
            }

            var areas = await _unitOfWork.GetRepository<Station>().GetPagingListAsync(
                selector: x => new StationResponse
                {
                    Id = x.Id,
                    StationName = x.StationName,
                    Address = x.Address,
                    Image = x.Image,
                    FileName = x.FileName,
                    OpeningHours = x.OpeningHours,
                    ClosingHours = x.ClosingHours,
                    Phone = x.Phone,
                    Email = x.Email,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    State = x.State.ToString(),
                    Status = x.Status

                },
                predicate: filterQuery,
                page: page,
                size: size);
            return areas;
        }

        public async Task<StationResponse> GetStationById(string id)
        {
            var area = await _unitOfWork.GetRepository<Station>().SingleOrDefaultAsync(
               selector: x => new StationResponse
               {
                   Id = x.Id,
                   StationName = x.StationName,
                   Address = x.Address,
                   Image = x.Image,
                   FileName = x.FileName,
                   OpeningHours = x.OpeningHours,
                   ClosingHours = x.ClosingHours,
                   Phone = x.Phone,
                   Email = x.Email,
                   DateCreated = x.DateCreated,
                   DateUpdated = x.DateUpdated,
                   Description = x.Description,
                   State = x.State.ToString(),
                   Status = x.Status
               },
               predicate: x => x.Id == id);
            return area;
        }

        public async Task<StationResponse> UpdateStation(string id, UpdateStationModel station)
        {
            var updateStation = await _unitOfWork.GetRepository<Station>().SingleOrDefaultAsync(predicate: x => x.Id == id);

            if (updateStation == null)
            {
                throw new ApiException("Station not found", 404, "NOT_FOUND");
            }
            if (station.Image != null && station.Image.Length > 0)
            {

                var f = await _cloudinaryService.UploadImageAsync(station.Image);

            }
            updateStation.StationName = station.StationName;
            updateStation.Address = station.Address;
            updateStation.OpeningHours = station.OpeningHours;
            updateStation.ClosingHours = station.ClosingHours;
            updateStation.Phone = station.Phone;
            updateStation.Email = station.Email;
            updateStation.Description = station.Description;
            updateStation.DateUpdated = DateTime.Now;
            _unitOfWork.GetRepository<Station>().UpdateAsync(updateStation);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;
            if (isSuccess)
            {
                return new StationResponse
                {
                    Id = updateStation.Id,
                    StationName = updateStation.StationName,
                    Address = updateStation.Address,
                    Image = updateStation.Image,
                    FileName = updateStation.FileName,
                    OpeningHours = updateStation.OpeningHours,
                    ClosingHours = updateStation.ClosingHours,
                    Phone = updateStation.Phone,
                    Email = updateStation.Email,
                    DateCreated = updateStation.DateCreated,
                    DateUpdated = updateStation.DateUpdated,
                    Description = updateStation.Description,
                    State = updateStation.State.ToString(),
                    Status = updateStation.Status


                };
            }
            throw new ApiException("Update Station Fail", 400, "BAD_REQUEST");
        }
    }
}
