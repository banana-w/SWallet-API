using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Response.Invitation
{
    public class InvitationResponse
    {
        public string Id { get; set; }
        public string InviterId { get; set; }
        public string Inviter { get; set; }
        public string InviteeId { get; set; }
        public string Invitee { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Description { get; set; }
        public bool? State { get; set; }
        public bool? Status { get; set; }
    }
}
