using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.Station;
using SWallet.Repository.Payload.Response.Station;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface IStationService
    {
        Task<IPaginate<StationResponse>> GetStation(string searchName, int page, int size);
        Task<StationResponse> GetStationById(string id);
        Task<StationResponse> CreateStation(CreateStationModel station);
        Task<StationResponse> UpdateStation(string id, UpdateStationModel station);
        void Delete(string id);
    }
}
