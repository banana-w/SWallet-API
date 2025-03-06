using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Repository.Services.Interfaces;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SendVerificationEmail(string email)
        {
           return Ok(await _emailService.SendVerificationEmail(email));
        }

    }
}
