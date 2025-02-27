using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Account;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Response.Account;
using SWallet.Repository.Services.Interfaces;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpPost("/register")]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] CreateStudentAccount registerRequest)
        {
            var result = await _accountService.CreateStudentAccount(registerRequest);
            if (result == null)
            {
                throw new ApiException("Account creation failed.", StatusCodes.Status400BadRequest, "ACCOUNT_CREATION_FAILED");
            }
            return Ok(result);
        }

        [HttpPost("/brandRegister")]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> BrandRegister([FromForm] AccountRequest accountRequest, [FromForm] CreateBrandByAccountId brandRequest)
        {
            var result = await _accountService.CreateBrandAccount(accountRequest, brandRequest);
            if (result == null)
            {
                throw new ApiException("Account creation failed.", StatusCodes.Status400BadRequest, "ACCOUNT_CREATION_FAILED");
            }
            return Ok(result);
        }
    }
}
