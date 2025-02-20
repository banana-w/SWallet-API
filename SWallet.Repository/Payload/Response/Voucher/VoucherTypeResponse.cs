using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Response.Voucher
{
    public class VoucherTypeResponse
    {
        public string Id { get; set; } = null!;

        public string TypeName { get; set; } = null!;

        public string Image { get; set; } = null!;

        public string FileName { get; set; } = null!;

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public string Description { get; set; } = null!;

        public bool? State { get; set; }

        public bool? Status { get; set; }
    }
}
