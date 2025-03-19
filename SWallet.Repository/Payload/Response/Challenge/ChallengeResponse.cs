using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Response.Challenge
{
    public class ChallengeResponse
    {
        public string Id { get; set; } = null!;

        public int? Type { get; set; }

        public string ChallengeName { get; set; } = null!;

        public decimal? Amount { get; set; }

        public decimal? Condition { get; set; }

        public string Image { get; set; } = null!;

        public string FileName { get; set; } = null!;

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public string Description { get; set; } = null!;

        public bool? State { get; set; }

        public bool? Status { get; set; }
    }
}
