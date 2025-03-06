using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Request.Invitation;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Payload.Response.Invitation;
using SWallet.Repository.Services.Implements;
using SWallet.Repository.Services.Interfaces;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvitationController : ControllerBase
    {
        private readonly IInvitationService _invitationService;

        public InvitationController(IInvitationService invitationService)
        {
            _invitationService = invitationService;
        }

        [HttpPost]
        public async Task<ActionResult<InvitationResponse>> CreateInvitation(CreateInvitationModel creation)
        {
            try
            {
                var invitationResponse = await _invitationService.Add(creation);
                return Ok(invitationResponse); // Return 201 Created with location header
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating invitation"); // Return 500 Internal Server Error
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExistInvitation(string invitee)
        {
            var response = await _invitationService.ExistInvitation(invitee);
            return Ok(response);
        }
    }
}
