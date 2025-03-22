using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Request.DistributionPoint
{
    public class PointDistributionRequest
    {
        public string CampusId { get; set; }
        public List<string> lecturerIds { get; set; }
        public int Points { get; set; }
    }
}
