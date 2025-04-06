using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Request.Voucher
{
    public class UseVoucherRequest
    {
        public string StudentId { get; set; }
        public string VoucherId { get; set; }
        public string StoreId { get; set; }
    }
}
