using Microsoft.EntityFrameworkCore;
using NewPayGenixAPI.Models;

namespace NewPayGenixAPI.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Employee> GetEmployeeDetailsAsync(int employeeId);
        Task UpdateEmployeePersonalInfoAsync(Employee employee);
        Task<IEnumerable<Payroll>> GetPayStubsAsync(int employeeId);
        Task RequestLeaveAsync(LeaveRequest leaveRequest);
        Task AddEmployeeAsync(Employee employee);
        Task GenerateComplianceReportAsync(ComplianceReport report);

        Task<Employee> GetEmployeeDetailsByUserIDAsync(int userId);
        Task<IEnumerable<Payroll>> GetPayStubsByUserIDAsync(int userId);

        Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeIdAsync(int employeeId);

        Task<List<LeaveRequest>> GetLeaveRequestsByUserIdAsync(int userId);
    }
}
