using NewPayGenixAPI.Models;

namespace NewPayGenixAPI.Repositories
{
    public interface IManagerRepository
    {
        Task<IEnumerable<Payroll>> GetTeamPayrollsAsync();
        //Task<IEnumerable<LeaveRequest>> GetPendingLeaveRequestsAsync(int managerId);
        //Task ApproveLeaveRequestAsync(int leaveRequestId, bool isApproved);
        Task<IEnumerable<LeaveRequest>> GetLeaveRequestsByEmployeeIdAsync(int employeeId);
        Task UpdateLeaveRequestStatusAsync(int leaveRequestId, string status);
        }
    }
