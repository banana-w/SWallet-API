using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Authentication;
using SWallet.Repository.Payload.Response.Authentication;
using SWallet.Repository.Services.Interfaces;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [Tags("🔐Authentication API")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authService;
        public AuthController(IAuthenticationService authService)
        {
            _authService = authService;
        }
        [HttpPost("login")]
        [ProducesResponseType( typeof(LoginResponse),StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var result = await _authService.Login(loginRequest);
            if ( result == null)
            {
                throw new ApiException("Invalid username or password", Unauthorized().StatusCode,"INVALID_AUTH");
            }
            return Ok(result);
        }

        [HttpPost("verify-code")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeRequest verifyCodeRequest)
        {
            var result = await _authService.VerifyEmail(verifyCodeRequest.Email, verifyCodeRequest.Code);
            if (!result)
            {
                throw new ApiException("Invalid code", StatusCodes.Status400BadRequest, "INVALID_CODE");
            }
            return Ok(result);
        }

    }
}
