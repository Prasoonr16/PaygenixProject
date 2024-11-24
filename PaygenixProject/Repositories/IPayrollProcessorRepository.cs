using NewPayGenixAPI.Models;

namespace NewPayGenixAPI.Repositories
{
    public interface IPayrollProcessorRepository
    {
        Task AddPayrollAsync(Payroll payroll);
        Task<Payroll> GetPayrollByEmployeeIdAsync(int employeeId);
        Task VerifyPayrollAsync(int employeeId);
    }
}
