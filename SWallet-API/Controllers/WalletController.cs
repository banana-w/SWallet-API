using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Repository.Payload.Request.Wallet;
using SWallet.Repository.Payload.Response.Wallet;
using SWallet.Repository.Services.Interfaces;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(WalletResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddWallet([FromBody] WalletRequest walletRequest)
        {
            var result = await _walletService.AddWallet(walletRequest);
            return Ok(result);
        }

        [HttpGet("{studentId}/{type}")]
        [ProducesResponseType(typeof(WalletResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWalletByStudentId(string studentId, int type)
        {
            var wallet = await _walletService.GetWalletByStudentId(studentId, type);
            return Ok(wallet);
        }

        [HttpPut("{id}/{balance}")]
        [ProducesResponseType(typeof(WalletResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateWallet(string id, decimal balance)
        {
            var wallet = await _walletService.UpdateWallet(id, balance);
            return Ok(wallet);
        }
    }
}
