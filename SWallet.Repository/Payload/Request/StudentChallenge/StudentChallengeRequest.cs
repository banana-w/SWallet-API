using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Request.StudentChallenge
{
    public class StudentChallengeRequest
    {
        public string ChallengeId { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Current { get; set; }
        public decimal? Condition { get; set; }
        public string Description { get; set; }
        public bool? Status { get; set; }
    }
}
