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

        public string Category { get; set; } = null!;

        public string Image { get; set; } = null!;

        public string FileName { get; set; } = null!;

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public string Description { get; set; } = null!;

        public bool? State { get; set; }

        public bool? Status { get; set; }
    }

    public class ChallengeResponseExtra
    {
        public string id { get; set; }
        public string challengeId { get; set; }
        public string challengeTypeId { get; set; }
        public string challengeType { get; set; }
        public string challengeTypeName { get; set; }
        public string challengeName { get; set; }
        public string category { get; set; }
        public string challengeImage { get; set; }
        public string studentId { get; set; }
        public string studentName { get; set; }
        public decimal amount { get; set; }
        public decimal current { get; set; }
        public decimal condition { get; set; }
        public bool isCompleted { get; set; }
        public bool isClaimed { get; set; }
        public DateTime dateCreated { get; set; }
        public DateTime? dateUpdated { get; set; }
        public string description { get; set; }
        public bool status { get; set; }
    }
}
