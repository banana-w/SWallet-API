using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Request.PointPackage
{
    public class PurchasePointRequest
    {
        public string PointPackageId { get; set; }
        public string CampusId { get; set; }
        public string ReturnUrl { get; set; }
    }
}

