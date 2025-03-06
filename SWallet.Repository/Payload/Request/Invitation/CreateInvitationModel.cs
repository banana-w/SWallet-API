using SWallet.Repository.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Request.Invitation
{
    public class CreateInvitationModel
    {
        [Required(ErrorMessage = "Cần có người mời")]
        public string InviterId { get; set; }

        [Required(ErrorMessage = "Cần có người được mời")]
        public string InviteeId { get; set; }

        public string Description { get; set; }

        public bool? State { get; set; }

    }
}
