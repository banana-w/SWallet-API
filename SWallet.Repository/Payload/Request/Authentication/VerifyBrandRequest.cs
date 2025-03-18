using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Request.Authentication
{
    public class VerifyBrandRequest
    {
        public string BrandId { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
    }
}
