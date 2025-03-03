using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Response.Activity
{
    public class ActivityResponse
    {
        public string Id { get; set; }
        public string StoreId { get; set; }
        public string StudentId { get; set; }
        public string VoucherItemId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public bool State { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
