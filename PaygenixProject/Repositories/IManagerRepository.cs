using NewPayGenixAPI.DTO;
using NewPayGenixAPI.Models;

namespace NewPayGenixAPI.Repositories
{
    public interface IManagerRepository
    {
        Task<IEnumerable<Payroll>> GetTeamPayrollsAsync();
        Task<IEnumerable<LeaveRequest>> GetLeaveRequestsByEmployeeIdAsync(int employeeId);
        Task UpdateLeaveRequestStatusAsync(int leaveRequestId, string status);

        Task<List<LeaveRequest>> GetAllLeaveRequestsAsync();

        
        Task<List<Employee>> GetEmployeesByManagerAsync(int managerUserId);
        Task<List<PayrollDTO>> GetPayrollsByManagerAsync(int managerUserId);
        Task<List<LeaveRequestDTO>> GetLeaveRequestsByManagerAsync(int managerUserId);
    }
    
}
