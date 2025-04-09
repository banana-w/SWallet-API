using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Domain.Models;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly SwalletDbContext _context;

        public LocationController(SwalletDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetLocations()
        {
            var locations = _context.Locations
                .Select(l => new { l.Id, l.Name, l.Latitue, l.Longtitude })
                .ToList();
            return Ok(locations);
        }
    }
}
