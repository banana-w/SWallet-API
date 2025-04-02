using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Voucher;
using SWallet.Repository.Payload.Response.Voucher;
using SWallet.Repository.Services.Interfaces;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [Tags("Voucher API")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;
        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }
        [HttpPost]
        [ProducesDefaultResponseType(typeof(bool))]
        public async Task<IActionResult> CreateVoucher([FromForm] VoucherRequest request)
        {
            var result = await _voucherService.CreateVoucher(request);
            if (result)
            {
                return CreatedAtAction(nameof(CreateVoucher), result);
            }
            throw new ApiException("Create voucher fail", 400, "VOUCHER_FAIL");
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(VoucherResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVoucherById(string id)
        {
            var result = await _voucherService.GetVoucherById(id);
            if (result != null)
            {
                return Ok(result);
            }
            throw new ApiException("Voucher not found.", StatusCodes.Status404NotFound, "VOUCHER_NOT_FOUND");
        }

        [HttpGet("withCDid")]
        [ProducesResponseType(typeof(VoucherResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVoucherByCDId(string id, string campaignDetailId)
        {
            var result = await _voucherService.GetVoucherWithCampaignDetailId(id, campaignDetailId);
            if (result != null)
            {
                return Ok(result);
            }
            throw new ApiException("Voucher not found.", StatusCodes.Status404NotFound, "VOUCHER_NOT_FOUND");
        }

        [HttpGet("withCId")]
        [ProducesResponseType(typeof(VoucherResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVoucherByCId(string id, string campaignId)
        {
            var result = await _voucherService.GetVoucherWithCampaignId(id, campaignId);
            if (result != null)
            {
                return Ok(result);
            }
            throw new ApiException("Voucher not found.", StatusCodes.Status404NotFound, "VOUCHER_NOT_FOUND");
        }

        [HttpGet]
        [ProducesResponseType(typeof(IPaginate<VoucherResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVouchers(string brandId, string? search, bool? state, bool? isAsc, int page, int size)
        {
            var result = await _voucherService.GetVouchers(brandId, search, state, isAsc, page, size);
            if (result != null)
            {
                return Ok(result);
            }
            throw new ApiException("Voucher not found.", StatusCodes.Status404NotFound, "VOUCHER_NOT_FOUND");
        }

        [HttpGet("get-all-vouchers")]
        [ProducesResponseType(typeof(IPaginate<VoucherResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllVouchers(string? search, int page = 1, int size = 10)
        {
            var result = await _voucherService.GetAllVouchers(search, page, size);
            if (result != null)
            {
                return Ok(result);
            }
            throw new ApiException("Voucher not found.", StatusCodes.Status404NotFound, "VOUCHER_NOT_FOUND");
        }

        [HttpGet("campaign-detail/{campaignId}")]
        [ProducesResponseType(typeof(IEnumerable<VoucherResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVoucherByCampaignId(string campaignId)
        {
            var result = await _voucherService.GetVouchersByCampaignId(campaignId);
            if (result != null)
            {
                return Ok(result);
            }
            throw new ApiException("Voucher not found.", StatusCodes.Status404NotFound, "VOUCHER_NOT_FOUND");
        }

        [HttpPut("{id}")]
        [ProducesDefaultResponseType(typeof(bool))]
        public async Task<IActionResult> UpdateVoucher(string id, [FromForm] VoucherRequest request)
        {
            var result = await _voucherService.UpdateVoucher(id, request);
            return Ok(result);
        }

    

    }
}
