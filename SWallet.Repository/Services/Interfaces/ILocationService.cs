using SWallet.Domain.Models;
using SWallet.Repository.Payload.Request.Account;
using SWallet.Repository.Payload.Response.Lecturer;
using SWallet.Repository.Payload.Response.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface ILocationService
    {
        Task<Location> UpdateLocation(string id, Location location);
        Task<IEnumerable<LocationResponse>> GetLocations();
        Task<Location> CreateLocation(Location location);
    }
}
