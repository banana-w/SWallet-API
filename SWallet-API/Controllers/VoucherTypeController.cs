using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Repository.Payload.Request.Voucher;
using SWallet.Repository.Payload.Response.Voucher;
using SWallet.Repository.Services.Interfaces;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [Tags("Voucher Type API")]
    [ApiController]
    public class VoucherTypeController : ControllerBase
    {
        private readonly IVoucherTypeService _voucherTypeService;
        public VoucherTypeController(IVoucherTypeService voucherTypeService)
        {
            _voucherTypeService = voucherTypeService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(VoucherTypeResponse), StatusCodes.Status200OK)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddVoucherType([FromForm] VoucherTypeRequest voucherTypeRequest)
        {
            var result = await _voucherTypeService.CreateVoucherType(voucherTypeRequest);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(VoucherTypeResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVoucherTypeById(string id)
        {
            var voucherType = await _voucherTypeService.GetVoucherTypeById(id);
            return Ok(voucherType);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<VoucherTypeResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllVoucherTypes(string search = "", int page = 1, int size = 10)
        {
            var voucherTypes = await _voucherTypeService.GetVoucherTypes(search, page, size);
            return Ok(voucherTypes);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(VoucherTypeResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateVoucherType(string id, [FromForm] VoucherTypeRequest voucherTypeRequest)
        {
            var voucherType = await _voucherTypeService.UpdateVoucherType(id, voucherTypeRequest);
            return Ok(voucherType);
        }


    }
}
