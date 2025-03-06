using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Request.Account
{
    public class UpdateLecturerModel
    {
        public string FullName { get; set; } = null!;

        public bool? State { get; set; }

        public bool? Status { get; set; }
    }
}
