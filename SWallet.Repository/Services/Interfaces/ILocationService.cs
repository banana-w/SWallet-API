using SWallet.Domain.Models;
using SWallet.Repository.Payload.Request.Account;
using SWallet.Repository.Payload.Response.Lecturer;
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
        Task<Location> CreateLocation(Location location);
    }
}
