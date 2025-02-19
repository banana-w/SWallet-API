using Microsoft.AspNetCore.Mvc;
using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.Account; // Import your request models
using SWallet.Repository.Payload.Response.Admin; // Import your response models
using SWallet.Repository.Services.Interfaces; // Import your service interface
using System.Threading.Tasks;

namespace SWallet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Or a more specific route
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger; // Inject ILogger

        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<AdminResponse>> CreateAdmin(CreateAdminModel creation)
        {
            try
            {
                var adminResponse = await _adminService.Add(creation);
                return Ok(adminResponse); // Return 201 Created with location header
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating admin"); // Log the error
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating admin"); // Return 500 Internal Server Error
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AdminResponse>> GetAdminById(string id)
        {
            try
            {
                var adminResponse = await _adminService.GetById(id);
                if (adminResponse == null)
                {
                    return NotFound(); // Return 404 Not Found
                }
                return Ok(adminResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting admin by ID: {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting admin");
            }
        }


        [HttpGet]
        public async Task<ActionResult<IPaginate<AdminResponse>>> GetAllAdmins(string? searchName = "", int page = 1, int size = 10) // Pagination parameters
        {
            try
            {
                var adminResponses = await _adminService.GetAll(searchName, page, size);
                return Ok(adminResponses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all admins");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting admins");
            }
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<AdminResponse>> UpdateAdmin(string id, UpdateAdminModel update)
        {
            try
            {
                var adminResponse = await _adminService.Update(id, update);
                return Ok(adminResponse); // Return 200 OK with the updated admin
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating admin: {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating admin");
            }
        }

    }
}