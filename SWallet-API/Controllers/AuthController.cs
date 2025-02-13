using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Login;
using SWallet.Repository.Payload.Response.Login;
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
        [HttpPost("/login")]
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

    }
}
