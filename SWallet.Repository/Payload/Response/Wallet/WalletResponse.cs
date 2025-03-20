using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Response.Wallet
{
    public class WalletResponse
    {
        public string Id { get; set; } = null!;

        public string CampaignId { get; set; } = null!;

        public string StudentId { get; set; } = null!;

        public string BrandId { get; set; } = null!;

        public string CampusId { get; set; } = null!;

        public int? Type { get; set; }

        public decimal? Balance { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public string Description { get; set; } = null!;

        public bool? State { get; set; }

        public bool? Status { get; set; }
    }
}
