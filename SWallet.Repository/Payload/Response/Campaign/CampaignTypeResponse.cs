using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Response.Campaign
{
    public class CampaignTypeResponse
    {
        public string Id { get; set; }
        public string TypeName { get; set; }
        public string Image { get; set; }
        public string FileName { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string Description { get; set; }
        public bool? State { get; set; }
        public bool? Status { get; set; }
        public int? NumberOfCampaign { get; set; }
    }
}
