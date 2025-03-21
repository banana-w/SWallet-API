using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Response.Voucher
{
    public class VoucherItemResponse
    {
        public string Id { get; set; } = null!;

        public string VoucherId { get; set; } = null!;

        public string CampaignDetailId { get; set; } = null!;

        public string VoucherCode { get; set; } = null!;

        public int? Index { get; set; }

        public bool? IsLocked { get; set; }

        public bool? IsBought { get; set; }

        public bool? IsUsed { get; set; }

        public DateOnly? ValidOn { get; set; }

        public DateOnly? ExpireOn { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateIssued { get; set; }

        public bool? Status { get; set; }
    }
}
