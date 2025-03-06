using SWallet.Domain.Models;
using SWallet.Repository.Payload.Request.Invitation;
using SWallet.Repository.Payload.Response.Invitation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface IInvitationService
    {
        Task<InvitationResponse> Add(CreateInvitationModel creation);

        Task<bool> ExistInvitation(string invitee);
    }
}
