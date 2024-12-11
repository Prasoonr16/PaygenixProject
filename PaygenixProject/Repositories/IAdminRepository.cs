using NewPayGenixAPI.DTO;
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
        Task<IEnumerable<User>> GetAllUserAsync();
        Task<User> GetUserByIdAsync(int id);
        Task UpdateUserAsync(User user);
        Task AddUserAsync(User user);

        Task<IEnumerable<Payroll>> GetAllPayrollsAsync();
        Task AddPayrollAsync(Payroll payroll);
        Task<Payroll> GetPayrollByEmployeeIdAsync(int id);
        Task UpdatePayrollAsync(Payroll payroll);

        Task<IEnumerable<ComplianceReport>> GetAllComplianceReportAsync();

        Task<bool> UpdateComplianceReportAsync(int employeeId, ComplianceReportDTO updateDTO);


    }
}
