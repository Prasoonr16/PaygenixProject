﻿using Microsoft.AspNetCore.Authorization;
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
    }
}
