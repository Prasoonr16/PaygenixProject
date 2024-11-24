using NewPayGenixAPI.Models;

namespace NewPayGenixAPI.Repositories
{
    public interface IAdminRepository
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<Employee> GetEmployeeByIdAsync(int id);
        Task AddEmployeeAsync(Employee employee);
        Task UpdateEmployeeAsync(Employee employee);
        Task DeleteEmployeeAsync(int id);
        Task AssignRoleToUserAsync(int userId, int roleId);

        Task GeneratePayrollAsync(int employeeId);
        Task GenerateComplianceReportAsync(ComplianceReport report);
    }
}
