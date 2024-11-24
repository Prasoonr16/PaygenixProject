using NewPayGenixAPI.Models;

namespace NewPayGenixAPI.Repositories
{
    public interface IManagerRepository
    {
        //Task<IEnumerable<Payroll>> GetTeamPayrollsAsync(int managerId);
        //Task<IEnumerable<LeaveRequest>> GetPendingLeaveRequestsAsync(int managerId);
        Task ApproveLeaveRequestAsync(int leaveRequestId, bool isApproved);
        }
    }
