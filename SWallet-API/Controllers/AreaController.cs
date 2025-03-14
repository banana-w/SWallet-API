using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Area;
using SWallet.Repository.Payload.Response.Area;
using SWallet.Repository.Services.Interfaces;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [Tags("🗺️Area API")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        private readonly IAreaService _areaService;
        public AreaController(IAreaService areaService)
        {
            _areaService = areaService;
        }
        [HttpGet("areas")]
        [ProducesResponseType(typeof(AreaResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAreas(string? searchName, int page, int size)
        {
            var result = await _areaService.GetAreas(searchName, page, size);
            if (result == null)
            {
                throw new ApiException("Area not found.", StatusCodes.Status404NotFound, "AREA_NOT_FOUND");
            }
            return Ok(result);
        }
        [HttpGet("areas/{id}")]
        [ProducesResponseType(typeof(AreaResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAreaById(string id)
        {
            var result = await _areaService.GetAreaById(id);
            if (result == null)
            {
                throw new ApiException("Area not found.", StatusCodes.Status404NotFound, "AREA_NOT_FOUND");
            }
            return Ok(result);
        }

        [HttpPost("areas")]
        [ProducesResponseType(typeof(AreaResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateArea([FromForm] AreaRequest area)
        {
            var result = await _areaService.CreateArea(area);
            return Ok(result);
        }

        [HttpPut("areas/{id}")]
        [ProducesResponseType(typeof(AreaResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateArea(string id, [FromForm] AreaRequest area)
        {
            var result = await _areaService.UpdateArea(id, area);
            if (result == null)
            {
                throw new ApiException("Area update failed.", StatusCodes.Status400BadRequest, "AREA_UPDATE_FAILED");
            }
            return Ok(result);
        }


    }
}
