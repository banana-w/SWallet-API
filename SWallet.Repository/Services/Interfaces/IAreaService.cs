using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.Area;
using SWallet.Repository.Payload.Response.Area;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface IAreaService
    {
        Task<IPaginate<AreaResponse>> GetAreas(string? searchName, int page, int size);
        Task<AreaResponse> GetAreaById(string id);
        Task<AreaResponse> CreateArea(AreaRequest area);
        Task<AreaResponse> UpdateArea(string id, AreaRequest area);
    }
}
