using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Request.LuckyPrize
{
    public class LuckyPrizeRequest
    {
        public string? PrizeName { get; set; }

        public decimal? Probability { get; set; }

        public int? Quantity { get; set; }

        public bool? Status { get; set; }
    }
}

