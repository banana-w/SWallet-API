using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.Store;
using SWallet.Repository.Payload.Response.Store;
using SWallet.Repository.Services.Interfaces;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _storeService;
        private readonly ILogger<StoreController> _logger;

        public StoreController(IStoreService storeService, ILogger<StoreController> logger)
        {
            _storeService = storeService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<StoreResponse>> CreateStore(string accountId, CreateStoreModel creation)
        {
            try
            {
                var storeResponse = await _storeService.CreateStore(accountId, creation);
                return Ok(storeResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating store");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating store");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStoreById(string id)
        {
            var storeResponse = await _storeService.GetStoreById(id);
            if (storeResponse == null)
            {
                return NotFound();
            }
            return Ok(storeResponse);
        }

        [HttpGet]
        public async Task<ActionResult<IPaginate<StoreResponse>>> GetAllStores(string searchName = "", int page = 1, int size = 10)
        {
            try
            {
                var result = await _storeService.GetStores(searchName, page, size);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stores");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting stores");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStore(string id,UpdateStoreModel store)
        {
            var result = await _storeService.UpdateStore(id, store);
            return Ok(result);
        }

       
    }
}
