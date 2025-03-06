using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Request.Voucher
{
    public class VoucherItemRequest
    {
        public string VoucherId { get; set; }
        public string CampaignDetailId { get; set; }
        public int Quantity { get; set; }
        public DateOnly? ValidOn { get; set; }
        public DateOnly? ExpireOn { get; set; }
    }
}
