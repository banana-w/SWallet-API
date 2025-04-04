using SWallet.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Interfaces.CampaignRepository
{
    public interface ICampaignRepository : IGenericRepository<Campaign>
    {
        Task UpdateStatusAsync(string id, bool status);
    }
}
