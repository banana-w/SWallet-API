using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Response.CampTransaction
{
    public class CampTransactionResponse
    {
        public string CampaignId { get; set; } = null!;

        public string WalletId { get; set; } = null!;

        public decimal? Amount { get; set; }

        public decimal? Rate { get; set; }

        public DateTime? DateCreated { get; set; }

        public string Description { get; set; } = null!;
    }
}
