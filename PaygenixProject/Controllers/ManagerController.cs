using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewPayGenixAPI.Repositories;

namespace NewPayGenixAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Manager")] // Only users with the "Manager" role can access these endpoints
    
    public class ManagerController : ControllerBase
    {
        private readonly IManagerRepository _managerRepository;

        public ManagerController(IManagerRepository managerRepository)
        {
            _managerRepository = managerRepository;
        }


        [HttpPut("leave-request/{leaveRequestId}/update-status")]
        public async Task<IActionResult> UpdateLeaveRequestStatus(int leaveRequestId, [FromQuery] string status)
        {
            try
            {
                // Validate the status (e.g., Approved, Rejected)
                var validStatuses = new[] { "Approved", "Rejected" };
                if (!validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase))
                {
                    return BadRequest("Invalid status. Valid values are: Approved, Rejected.");
                }

                await _managerRepository.UpdateLeaveRequestStatusAsync(leaveRequestId, status);
                return Ok($"Leave request {leaveRequestId} status updated to {status}.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        //API for getting the payroll and leave request
        [HttpGet("payrolls/{managerUserId}")]
        public async Task<IActionResult> GetPayrollsByManager(int managerUserId)
        {
            try
            {
                var payrolls = await _managerRepository.GetPayrollsByManagerAsync(managerUserId);
                return Ok(payrolls);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("leave-requests/{managerUserId}")]
        public async Task<IActionResult> GetLeaveRequestsByManager( int managerUserId)
        {
            try
            {
                var leaveRequests = await _managerRepository.GetLeaveRequestsByManagerAsync(managerUserId);
                return Ok(leaveRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
