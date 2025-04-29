using SWallet.Domain.Models;
using SWallet.Repository.Implement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Interfaces.CampaignRepository
{
    public class CampaignRepository : GenericRepository<Campaign>, ICampaignRepository
    {
        public CampaignRepository(SwalletDbContext context) : base(context)
        {
        }

        public async Task UpdateStatusAsync(string id, bool status)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                entity.Status = status ? 1 : 3;
                _dbSet.Update(entity);
            }
        }
    }
}
