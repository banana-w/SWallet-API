using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Activity;
using SWallet.Repository.Payload.Response.Activity;
using SWallet.Repository.Services.Interfaces;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService _activityService;
        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }
        [HttpGet]
        [ProducesResponseType(typeof(IPaginate<ActivityResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IPaginate<ActivityResponse>>> GetAllActivities(string? searchName = "", bool? isAsc = true, int page = 1, int size = 10)
        {
            try
            {
                var activities = await _activityService.GetActivityAsync(searchName, isAsc, page, size);
                return Ok(activities);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting activities: {ex.Message}");
            }
        }

        //[HttpGet("{id}")]
        //[ProducesResponseType(typeof(ActivityResponse), StatusCodes.Status200OK)]
        //public async Task<ActionResult<ActivityResponse>> GetActivityById(string id)
        //{
        //    try
        //    {
        //        var activity = await _activityService.GetActivityAsync(id);
        //        if (activity == null)
        //        {
        //            return NotFound();
        //        }
        //        return Ok(activity);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting activity by ID: {ex.Message}");
        //    }
        //}

        [HttpPost]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<ActivityResponse>> CreateActivity([FromBody] ActivityRequest activity)
        {
            try
            {
                var newActivity = await _activityService.RedeemVoucherActivityAsync(activity);
                if (newActivity)
                return Ok(newActivity);
                throw new ApiException("Create activity fail", StatusCodes.Status400BadRequest, "ACTIVITY_FAIL");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error creating activity: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ActivityResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<ActivityResponse>> UpdateActivity(string id, [FromBody] ActivityRequest activity)
        {
            try
            {
                var updatedActivity = await _activityService.UpdateActivityAsync(id, activity);
                if (!updatedActivity)
                {
                    return NotFound();
                }
                return Ok(updatedActivity);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating activity: {ex.Message}");
            }
        }

        [HttpPost("RedeemVoucher")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> RedeemVoucher([FromBody] ActivityRequest activity)
        {
            try
            {
                var newActivity = await _activityService.RedeemVoucherActivityAsync(activity);
                if (newActivity)
                    return Ok(newActivity);
                throw new ApiException("Redeem voucher fail", StatusCodes.Status400BadRequest, "REDEEM_VOUCHER_FAIL");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error redeeming voucher: {ex.Message}");
            }
        }

    }
}
