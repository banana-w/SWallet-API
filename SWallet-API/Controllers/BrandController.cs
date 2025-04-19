using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Account;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Response.Admin;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Services.Implements;
using SWallet.Repository.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SWallet_API.Controllers
{
    [ApiController]
    [Authorize] // Yêu cầu xác thực cho tất cả các endpoint
    [Route("api/[controller]")]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;
        private readonly ILogger<BrandController> _logger; // Inject ILogger

        public BrandController(IBrandService brandService, ILogger<BrandController> logger) // Constructor injection
        {
            _brandService = brandService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<BrandResponse>> CreateBrand(CreateBrandModel creation)
        {
            try
            {
                var brandResponse = await _brandService.CreateBrand(creation);
                return Ok(brandResponse); // Return 201 Created with location header
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating brand"); // Log the error
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating brand"); // Return 500 Internal Server Error
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrandById(string id)
        {
            try
            {
                // Lấy studentId từ JWT token, nếu không có thì để null
                var studentId = User.FindFirst("studentId")?.Value;

                var brandResponse = await _brandService.GetBrandById(id, studentId);
                if (brandResponse == null)
                {
                    return NotFound();
                }
                return Ok(brandResponse);
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, $"Error getting brand by ID: {id}");
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting brand by ID: {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting brand");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IPaginate<BrandResponse>>> GetAllBrands(
            string searchName = "", int page = 1, int size = 10, bool status = true)
        {
            try
            {
                // Lấy studentId từ JWT token
                var studentId = User.FindFirst("studentId")?.Value;

                var brandResponses = await _brandService.GetBrands(searchName, page, size, status, studentId);
                return Ok(brandResponses);
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "Error getting brands");
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting brands");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting brands");
            }
        }

        [HttpPost("existingAccount")] // New route to distinguish it
        public async Task<ActionResult<BrandResponse>> CreateBrandAsync([FromQuery] string accountId, CreateBrandByAccountId creation)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                return BadRequest("Account ID is required"); // Validate accountId
            }

            try
            {
                var brandResponse = await _brandService.CreateBrandAsync(accountId, creation);
                return Ok(brandResponse);
            }
            catch (ApiException ex) // Catch your custom ApiException
            {
                _logger.LogError(ex, $"Error creating brand for account ID: {accountId}");
                return StatusCode(ex.StatusCode, ex.Message); // Return custom error response
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating brand for account ID: {accountId}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating brand");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBrand(string id, UpdateBrandModel update)
        {
            try
            {
                var brandResponse = await _brandService.UpdateBrand(id, update);
                if (brandResponse == null)
                {
                    return NotFound();
                }
                return Ok(brandResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating brand by ID: {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating brand");
            }
        }

        [HttpGet("account/{accountId}")]
        [ProducesResponseType(typeof(BrandResponse), 200)]
        public async Task<IActionResult> GetBrandByAccountId(string accountId)
        {
            var brand = await _brandService.GetBrandbyAccountId(accountId)
                ?? throw new ApiException("Not Found.", StatusCodes.Status400BadRequest, "BRAND_NOTFOUND");
            return Ok(brand);
        }

    }
}