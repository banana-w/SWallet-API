using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Response.Lecturer
{
    public class LecturerResponse
    {
        public string Id { get; set; } = null!;

        public string AccountId { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public List<string> CampusName { get; set; } = null!;

        public decimal Balance { get; set; } = 0;

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public bool? State { get; set; }

        public bool? Status { get; set; }

        //public List<string> CampusIds { get; set; } = new List<string>();
    }
}
