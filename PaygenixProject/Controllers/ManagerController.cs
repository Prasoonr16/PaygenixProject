using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewPayGenixAPI.Repositories;
using PaygenixProject.Models;

namespace NewPayGenixAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Manager")] // Only users with the "Manager" role can access these endpoints
    
    public class ManagerController : ControllerBase
    {
        private readonly IManagerRepository _managerRepository;
        private readonly IAdminRepository _adminRepository;

        public ManagerController(IManagerRepository managerRepository, IAdminRepository adminRepository)
        {
            _managerRepository = managerRepository;
            _adminRepository = adminRepository;
        }

        // Review Team Payrolls
        [HttpGet("team-payrolls")]
        public async Task<IActionResult> GetTeamPayrolls()
        {
            var payrolls = await _managerRepository.GetTeamPayrollsAsync();
            return Ok(payrolls);
        }

       [HttpGet("employee/{employeeId}/leave-requests")]
        public async Task<IActionResult> GetLeaveRequestsByEmployeeId(int employeeId)
        {
            try
            {
                var leaveRequests = await _managerRepository.GetLeaveRequestsByEmployeeIdAsync(employeeId);
                if (!leaveRequests.Any()) return NotFound("No leave requests found for this employee.");
                return Ok(leaveRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("leave-request/{leaveRequestId}/update-status")]
        public async Task<IActionResult> UpdateLeaveRequestStatus(int leaveRequestId, [FromQuery] string status)
        {
            // Log the attempt to update the leave request status
            await _adminRepository.LogAuditTrailAsync(new AuditTrail
            {
                Action = "Update Leave Request Status Attempt",
                PerformedBy = "Manager", // Get the currently logged-in user
                Timestamp = DateTime.UtcNow,
                Details = $"Attempting to update leave request status for LeaveRequestID {leaveRequestId}."
            });

            try
            {
                // Validate the status (e.g., Approved, Rejected)
                var validStatuses = new[] { "Approved", "Rejected" };
                if (!validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase))
                {
                    return BadRequest("Invalid status. Valid values are: Approved, Rejected.");
                }

                await _managerRepository.UpdateLeaveRequestStatusAsync(leaveRequestId, status);

                // Log the success of the leave request status update
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Update Leave Request Status Success",
                    PerformedBy = "Manager", // Get the currently logged-in user
                    Timestamp = DateTime.UtcNow,
                    Details = $"Leave request status updated successfully for LeaveRequestID {leaveRequestId}. New Status: {status}"
                });

                return Ok($"Leave request {leaveRequestId} status updated to {status}.");
            }
            catch (Exception ex)
            {

                // Log the error if any exception occurs
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Update Leave Request Status Error",
                    PerformedBy = "Manager", // Get the currently logged-in user
                    Timestamp = DateTime.UtcNow,
                    Details = $"Error updating leave request status for LeaveRequestID {leaveRequestId}. Error: {ex.Message}"
                });
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("leave-requests")]
        public async Task<IActionResult> GetAllLeaveRequests()
        {
            try
            {
                // Fetch all leave requests from the repository
                var leaveRequests = await _managerRepository.GetAllLeaveRequestsAsync();

                if (!leaveRequests.Any())
                    return NotFound("No leave requests found.");

                return Ok(leaveRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
