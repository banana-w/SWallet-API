using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Account;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Request.Student;
using SWallet.Repository.Payload.Response.Account;
using SWallet.Repository.Services.Interfaces;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [Tags("🧑🏻‍💼Account API")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpPost("studentRegister")]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> StudentRegister([FromForm] AccountRequest accountRequest ,[FromForm] StudentRequest studentRequest)
        {
            var result = await _accountService.CreateStudentAccount(accountRequest, studentRequest);
            if (result == null)
            {
                throw new ApiException("Account creation failed.", StatusCodes.Status400BadRequest, "ACCOUNT_CREATION_FAILED");
            }
            return Ok(result);
        }

        [HttpPost("brandRegister")]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> BrandRegister([FromForm] AccountRequest accountRequest, [FromForm] CreateBrandByAccountId brandRequest)
        {
            var result = await _accountService.CreateBrandAccount(accountRequest, brandRequest)
                ?? throw new ApiException("Account creation failed.", StatusCodes.Status400BadRequest, "ACCOUNT_CREATION_FAILED");
            return Ok(result);
        }



        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAccountById(string id)
        {
            var result = await _accountService.GetAccountById(id);
            if (result == null)
            {
                throw new ApiException("Account not found.", StatusCodes.Status400BadRequest, "ACCOUNT_NOT_FOUND");
            }
            return Ok(result);
        }
    }
}
