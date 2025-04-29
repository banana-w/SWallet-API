using SWallet.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Repository
{
    public interface ICampaignRepository
    {
        long CountCampaign();
    }

    public class CampaignRepository : ICampaignRepository
    {
        private readonly SwalletDbContext swalletDB;

        public CampaignRepository(SwalletDbContext swalletDB)
        {
            this.swalletDB = swalletDB;
        }
        public long CountCampaign()
        {
            long count = 0;
            try
            {
                var db = swalletDB;
                count = db.Campaigns.Where(c => c.Status == 1).Count();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return count;
        }

    }
}
