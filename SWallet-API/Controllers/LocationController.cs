using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Domain.Models;
using SWallet.Repository.Payload.Request.Account;
using SWallet.Repository.Payload.Response.Lecturer;
using SWallet.Repository.Services.Implements;
using SWallet.Repository.Services.Interfaces;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly SwalletDbContext _context;

        private readonly ILocationService _locationService;

        private readonly ILogger<LocationController> _logger;

        public LocationController(SwalletDbContext context, ILocationService locationService, ILogger<LocationController> logger)
        {
            _context = context;
            _locationService = locationService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetLocations()
        {
            var locations = _context.Locations
                .Select(l => new { l.Id, l.Name, l.Latitue, l.Longtitude })
                .ToList();
            return Ok(locations);
        }

        [HttpPost("create-location")]
        public async Task<ActionResult<Location>> CreateLocation(Location location)
        {
            try
            {
                var locationResponse = await _locationService.CreateLocation(location);
                return Ok(locationResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating location");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating location");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocation(string id, Location update)
        {
            try
            {
                var locationResponse = await _locationService.UpdateLocation(id, update);
                if (locationResponse == null)
                {
                    return NotFound();
                }
                return Ok(locationResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating location ");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating location");
            }
        }

    }
}
