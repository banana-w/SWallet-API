using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Response.ActivityTransaction
{
    public class TransactionResponse
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;
        public string TransId { get; set; } = null!;

        public string WalletId { get; set; } = null!;

        public decimal? Amount { get; set; }

        //public decimal? Rate { get; set; }

        public string Description { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }

        public bool? Status { get; set; }
    }
}
