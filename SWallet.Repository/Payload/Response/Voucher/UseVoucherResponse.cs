using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Response.Voucher
{
    public class UseVoucherResponse
    {
        public string VoucherItemId { get; set; }
        public string VoucherCode { get; set; }
        public DateTime DateUsed { get; set; }
    }
}
