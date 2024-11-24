using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewPayGenixAPI.Repositories;

namespace NewPayGenixAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Manager")] // Only users with the "Manager" role can access these endpoints
    
    public class ManagerController : ControllerBase
    {
        private readonly IManagerRepository _managerRepository;

        public ManagerController(IManagerRepository managerRepository)
        {
            _managerRepository = managerRepository;
        }

        //// Review Team Payrolls
        //[HttpGet("{managerId}/team-payrolls")]
        //public async Task<IActionResult> GetTeamPayrolls(int managerId)
        //{
        //    var payrolls = await _managerRepository.GetTeamPayrollsAsync(managerId);
        //    return Ok(payrolls);
        //}

        //// Approve Leave Requests
        //[HttpGet("{managerId}/leave-requests")]
        //public async Task<IActionResult> GetPendingLeaveRequests(int managerId)
        //{
        //    var leaveRequests = await _managerRepository.GetPendingLeaveRequestsAsync(managerId);
        //    return Ok(leaveRequests);
        //}

        [HttpPut("approve-leave/{leaveRequestId}")]
        public async Task<IActionResult> ApproveLeaveRequest(int leaveRequestId, [FromQuery] bool isApproved)
        {
            await _managerRepository.ApproveLeaveRequestAsync(leaveRequestId, isApproved);
            return Ok(isApproved ? "Leave approved" : "Leave rejected");
        }

    }
}
