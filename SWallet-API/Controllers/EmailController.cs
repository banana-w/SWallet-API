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
        private readonly IRedisService _redisService;

        public EmailController(IEmailService emailService, IRedisService redisService)
        {
            _emailService = emailService;
            _redisService = redisService;
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SendVerificationEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required");
            }

            var code = await _emailService.SendVerificationEmail(email);
            await _redisService.SaveVerificationCodeAsync(email, code);

            return Ok(code);
        }

    }
}
