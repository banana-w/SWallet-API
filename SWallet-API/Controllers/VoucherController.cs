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
                return CreatedAtAction(nameof(CreateVoucher),result);
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
        [HttpGet]
        [ProducesResponseType(typeof(IPaginate<VoucherResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVouchers(string? search, bool? state, bool? isAsc, int page, int size)
        {
            var result = await _voucherService.GetVouchers(search, state, isAsc, page, size);
            if (result != null)
            {
                return Ok(result);
            }
            throw new ApiException("Voucher not found.", StatusCodes.Status404NotFound, "VOUCHER_NOT_FOUND");
        }
    }
}
