using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Response.Admin
{
    public class AdminResponse
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public string FileName { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string Description { get; set; }
        public bool? State { get; set; }
        public bool? Status { get; set; }
        public int? NumberOfRequests { get; set; }
        public decimal? AmountOfRequests { get; set; }
    }
}
