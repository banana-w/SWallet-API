using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Repository.Payload.Request.Voucher;
using SWallet.Repository.Payload.Response.Voucher;
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

        [HttpPost]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> GenerateVoucherItems([FromBody] VoucherItemRequest voucherItemRequest)
        {
            var result = await _voucherItemService.GenerateVoucherItemsAsync(voucherItemRequest);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<VoucherItemResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVoucherItems([FromQuery] IEnumerable<string> campaignDetailId)
        {
            var result = await _voucherItemService.GetVoucherItemsByCampaignDetailIdAsync(campaignDetailId);
            return Ok(result);
        }
        [HttpGet("viId")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVoucherItemIdAvailable(string voucherId, string studentId, string campaignId)
        {
            var result = await _voucherItemService.GetVoucherItemIdAvailable(voucherId, studentId, campaignId);
            return Ok(result);
        }

    }
}
