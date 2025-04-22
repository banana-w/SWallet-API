using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Security;
using SWallet.Domain.Models;
using SWallet.Repository.Payload.Charts;
using SWallet.Repository.Services.Implements;
using SWallet.Repository.Services.Interfaces;
using System.Net;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RankingController : Controller
    {
        private readonly ILogger<RankingController> _logger;
        private readonly IChartService _chartService;

        public RankingController(IChartService chartService, ILogger<RankingController> logger)
        {
            _chartService = chartService;
            _logger = logger;
        }

        [HttpGet("{id}admin/student-ranking")]
        [ProducesResponseType(typeof(List<RankingModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetStudentRankingByAdminId(string id)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid parameters");

            try
            {
                var result = await _chartService.GetRankingChart(id, typeof(Student), "Quản trị viên");
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


        [HttpGet("{id}admin/brand-ranking")]
        [ProducesResponseType(typeof(List<RankingModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetBrandRankingByAdminId(string id)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid parameters");

            try
            {
                var result = await _chartService.GetRankingChart(id, typeof(Brand), "Quản trị viên");
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


        [HttpGet("{id}brand/student-ranking")]
        [ProducesResponseType(typeof(List<RankingModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        /// <summary>
        /// Retrieves the top students who spent the most points to redeem vouchers for a specific brand.
        public async Task<IActionResult> GetStudentRankingByBrandId(string id)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid parameters");

            try
            {
                var result = await _chartService.GetRankingChart(id, typeof(Student), "Thương hiệu");
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

        [HttpGet("{id}brand/campaign-ranking")]
        [ProducesResponseType(typeof(List<RankingModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCampaignRankingByBrandId(string id)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid parameters");

            try
            {
                var result = await _chartService.GetRankingChart(id, typeof(Campaign), "Thương hiệu");
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
