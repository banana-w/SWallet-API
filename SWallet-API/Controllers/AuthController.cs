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

        [HttpPost("verify-student")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> VerifyStudent([FromBody] VerifyStudentRequest verifyStudentRequest)
        {
            var result = await _authService.VerifyStudent(verifyStudentRequest.Email, verifyStudentRequest.Code, verifyStudentRequest.StudentId);
            if (!result)
            {
                throw new ApiException("Invalid student", StatusCodes.Status400BadRequest, "INVALID_STUDENT");
            }
            return Ok(result);
        }

        [HttpPost("verify-brand")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> VerifyBrand([FromBody] VerifyBrandRequest verifyBrandRequest)
        {
            var result = await _authService.VerifyBrand(verifyBrandRequest.Email, verifyBrandRequest.Code, verifyBrandRequest.BrandId);
            if (!result)
            {
                throw new ApiException("Invalid brand", StatusCodes.Status400BadRequest, "INVALID_BRAND");
            }
            return Ok(result);
        }

        [HttpPost("verify-account")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> VerifyAccount([FromBody] VerifyAccountRequest verifyAccountRequest)
        {
            var result = await _authService.VerifyAccount(verifyAccountRequest.Email, verifyAccountRequest.Code, verifyAccountRequest.Id);
            if (!result)
            {
                throw new ApiException("Invalid account", StatusCodes.Status400BadRequest, "INVALID_ACCOUNT");
            }
            return Ok(result);
        }

    }
}
