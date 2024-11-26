using NewPayGenixAPI.Models;

namespace NewPayGenixAPI.Repositories
{
    public interface IPayrollProcessorRepository
    {
       
        Task<Payroll> GetPayrollByEmployeeIdAsync(int employeeId);
        Task VerifyPayrollAsync(int employeeId);
    }
}
