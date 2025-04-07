using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Security;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Charts;
using SWallet.Repository.Payload.Request.Store;
using SWallet.Repository.Payload.Response.Store;
using SWallet.Repository.Services.Implements;
using SWallet.Repository.Services.Interfaces;
using System.Net;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IChartService _chartService;
        private readonly IStoreService _storeService;
        private readonly ILogger<StoreController> _logger;

        public StoreController(IStoreService storeService, IChartService chartService, ILogger<StoreController> logger)
        {
            _storeService = storeService;
            _chartService = chartService;
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

        [HttpGet("account/{id}")]
        public async Task<IActionResult> GetStoreByAccountId(string id)
        {
            var storeResponse = await _storeService.GetStoreByAccountId(id);
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

        [HttpGet("brand")]
        public async Task<ActionResult<IPaginate<StoreResponse>>> GetAllStoresInBrand(string searchName = "", int page = 1, int size = 10)
        {
            try
            {
                var result = await _storeService.GetStoreInBrand(searchName, page, size);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stores");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting stores");
            }
        }

        [HttpGet("campaign/{campaignId}")]
        public async Task<ActionResult<IPaginate<StoreResponse>>> GetStoresInCampaign(string campaignId, string searchName = "", int page = 1, int size = 10)
        {
                var result = await _storeService.GetStoresInCampaign(campaignId, searchName, page, size);
                return Ok(result);
        }


        [HttpGet("brand/{brandId}")]
        public async Task<ActionResult<IPaginate<StoreResponse>>> GetStoresByBrandId(string brandId, string searchName = "", int page = 1, int size = 10)
        {
            try
            {
                var result = await _storeService.GetStoreByBrandId(brandId, searchName, page, size);
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

        [HttpGet("{id}/column-chart")]
        [ProducesResponseType(typeof(List<ColumnChartModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetColumnChartByStoreId(
    string id,
    [FromQuery] DateOnly fromDate,
    [FromQuery] DateOnly toDate,
    [FromQuery] bool? isAsc)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid parameters");

            try
            {
                var result = await _chartService.GetColumnChart(id, fromDate, toDate, isAsc, "Cửa hàng");
                return Ok(result);
            }
            catch (InvalidParameterException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting column chart");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("{id}/line-chart")]
        [ProducesResponseType(typeof(List<LineChartModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetLineChartByStoreId(string id)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid parameters");

            try
            {
                var result = await _chartService.GetLineChart(id, "Cửa hàng");
                return Ok(result);
            }
            catch (InvalidParameterException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting line chart");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("{id}/student-ranking")]
        [ProducesResponseType(typeof(List<RankingModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetStudentRankingByStoreId(string id)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid parameters");

            try
            {
                var result = await _chartService.GetRankingChart(id, typeof(Student), "Cửa hàng");
                return Ok(result);
            }
            catch (InvalidParameterException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting student rankings");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("{id}/title")]
        [ProducesResponseType(typeof(TitleStoreModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetTitleByStoreId(string id)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid parameters");

            try
            {
                var result = await _chartService.GetTitleStore(id);
                return Ok(result);
            }
            catch (InvalidParameterException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting store title");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("{id}/campaign-ranking")]
        [ProducesResponseType(typeof(List<RankingModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCampaignRankingByStoreId(string id)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid parameters");

            try
            {
                var result = await _chartService.GetRankingChart(id, typeof(Campaign), "Cửa hàng");
                return Ok(result);
            }
            catch (InvalidParameterException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting campaign rankings");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
    }
}
