using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Response.Campus;
using SWallet.Repository.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Implements
{
    public class LocationService : BaseService<LocationService>, ILocationService
    {
        public LocationService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<LocationService> logger) : base(unitOfWork, logger)
        {
        }

        public async Task<Location> CreateLocation(Location location)
        {
            // Chuẩn hóa Latitude và Longitude
            string NormalizeCoordinate(string coordinate)
            {
                if (string.IsNullOrWhiteSpace(coordinate))
                    throw new ApiException("Coordinate cannot be empty", 400, "BAD_REQUEST");

                // Thay dấu phẩy bằng dấu chấm
                coordinate = coordinate.Replace(',', '.');

                // Kiểm tra xem giá trị có thể chuyển thành double hay không
                if (!double.TryParse(coordinate, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                    throw new ApiException("Invalid coordinate format", 400, "BAD_REQUEST");

                return coordinate;
            }

            var newLocation = new Location
            {
                Id = Ulid.NewUlid().ToString(),
                Name = location.Name,
                Latitue = (decimal?)double.Parse(NormalizeCoordinate(location.Latitue.ToString()), CultureInfo.InvariantCulture),
                Longtitude = (decimal?)double.Parse(NormalizeCoordinate(location.Longtitude.ToString()), CultureInfo.InvariantCulture),
                Qrcode = location.Qrcode
            };

            await _unitOfWork.GetRepository<Location>().InsertAsync(newLocation);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;

            if (isSuccess)
            {
                return new Location
                {
                    Id = newLocation.Id,
                    Name = newLocation.Name,
                    Latitue = newLocation.Latitue,
                    Longtitude = newLocation.Longtitude,
                    Qrcode = newLocation.Qrcode
                };
            }

            throw new ApiException("Create Location Fail", 400, "BAD_REQUEST");
        }

        public async Task<Location> UpdateLocation(string id, Location location)
        {
            var updateLocation = await _unitOfWork.GetRepository<Location>()
       .SingleOrDefaultAsync(
           predicate: x => x.Id == id
       );
            if (updateLocation == null)
            {
                throw new ApiException("Location not found", 404, "NOT_FOUND");
            }

            updateLocation.Id = id;
            updateLocation.Name = location.Name;
            updateLocation.Latitue = location.Latitue;
            updateLocation.Longtitude = location.Longtitude;
            updateLocation.Qrcode = location.Qrcode;
            _unitOfWork.GetRepository<Location>().UpdateAsync(updateLocation);
           
            var isSuccess = await _unitOfWork.CommitAsync() > 0;
            if (isSuccess)
            {
                return new Location
                {
                    Id = id,
                    Name = updateLocation.Name,
                    Latitue = updateLocation.Latitue,
                    Longtitude = updateLocation.Longtitude,
                    Qrcode = updateLocation.Qrcode

                };
            }
            throw new ApiException("Update Location Fail", 400, "BAD_REQUEST");

        }
    }
    
}
