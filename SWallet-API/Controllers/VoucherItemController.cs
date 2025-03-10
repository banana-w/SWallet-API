using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Repository.Payload.Request.Voucher;
using SWallet.Repository.Services.Interfaces;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [Tags("Voucher Item API")]
    [ApiController]
    public class VoucherItemController : ControllerBase
    {
        private readonly IVoucherItemService _voucherItemService;

        public VoucherItemController(IVoucherItemService voucherItemService)
        {
            _voucherItemService = voucherItemService;
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> RedeemVoucher(string id)
        {
            var result = await _voucherItemService.RedeemVoucherAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> GenerateVoucherItems([FromBody] VoucherItemRequest voucherItemRequest)
        {
            var result = await _voucherItemService.GenerateVoucherItemsAsync(voucherItemRequest);
            return Ok(result);
        }

    }
}
