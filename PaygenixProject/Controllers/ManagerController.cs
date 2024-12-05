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

        // Review Team Payrolls
        [HttpGet("team-payrolls")]
        public async Task<IActionResult> GetTeamPayrolls()
        {
            var payrolls = await _managerRepository.GetTeamPayrollsAsync();
            return Ok(payrolls);
        }

        //// Approve Leave Requests
        //[HttpGet("{managerId}/leave-requests")]
        //public async Task<IActionResult> GetPendingLeaveRequests(int managerId)
        //{
        //    var leaveRequests = await _managerRepository.GetPendingLeaveRequestsAsync(managerId);
        //    return Ok(leaveRequests);
        //}

        [HttpGet("employee/{employeeId}/leave-requests")]
        public async Task<IActionResult> GetLeaveRequestsByEmployeeId(int employeeId)
        {
            var leaveRequests = await _managerRepository.GetLeaveRequestsByEmployeeIdAsync(employeeId);
            if (!leaveRequests.Any()) return NotFound("No leave requests found for this employee.");
            return Ok(leaveRequests);
        }

        [HttpPut("leave-request/{leaveRequestId}/update-status")]
        public async Task<IActionResult> UpdateLeaveRequestStatus(int leaveRequestId, [FromQuery] string status)
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
        //[HttpPut("approve-leave/{employeeid}")]
        //public async Task<IActionResult> ApproveLeaveRequest(int employeeid, [FromQuery] bool isApproved)
        //{
        //    await _managerRepository.ApproveLeaveRequestAsync(employeeid, isApproved);
        //    return Ok(isApproved ? "Leave approved" : "Leave rejected");
        //}

    }
}
