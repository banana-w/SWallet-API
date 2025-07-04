﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Account;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Request.Store;
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

        [HttpPost("storeRegister")]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> StoresRegister([FromForm] AccountRequest accountRequest, CreateStoreModel storeRequest)
        {
            var result = await _accountService.CreateStoreAccount(accountRequest, storeRequest)
                ?? throw new ApiException("Account creation failed.", StatusCodes.Status400BadRequest, "ACCOUNT_CREATION_FAILED");
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

        [HttpPost("campusRegister")]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CampusRegister([FromForm] AccountRequest accountRequest, string campusId)
        {
            var result = await _accountService.CreateCampusAccount(accountRequest, campusId)
                ?? throw new ApiException("Account creation failed.", StatusCodes.Status400BadRequest, "ACCOUNT_CREATION_FAILED");
            return Ok(result);
        }

        [HttpPost("lecturerRegister")]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LecturerRegister([FromForm] AccountRequest accountRequest, [FromForm] CreateLecturerModel lecturerReq, [FromForm] string campusId)
        {
            var result = await _accountService.CreateLecturerAccount(accountRequest, lecturerReq, campusId)
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

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAccount(string id, string phone, string email, string oldPassword, string newPassword)
        {
            var result = await _accountService.UpdateAccount(id, phone, email, oldPassword, newPassword)
                ?? throw new ApiException("Account update failed.", StatusCodes.Status400BadRequest, "ACCOUNT_UPDATE_FAILED");
            return Ok(result);
        }

        [HttpPut("{id}/avatar")]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateAccountAvatar(string id, IFormFile avatar)
        {
            var result = await _accountService.UpdateAccountAvatar(id, avatar);
            return Ok(result);
        }

        [HttpPost("validUsername/{username}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ValidUsername(string username)
        {
            var result = await _accountService.ValidUsername(username);
            return Ok(result);
        }

        [HttpPost("validEmail/{email}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ValidEmail(string email)
        {
            var result = await _accountService.ValidEmail(email);
            return Ok(result);
        }

        [HttpPost("validInviteCode")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ValidInviteCode(string inviteCode)
        {
            var result = await _accountService.ValidInviteCode(inviteCode);
            return Ok(result);
        }
    }
}
