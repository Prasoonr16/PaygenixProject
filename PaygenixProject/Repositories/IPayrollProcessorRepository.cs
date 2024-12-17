using NewPayGenixAPI.DTO;
using NewPayGenixAPI.Models;

namespace NewPayGenixAPI.Repositories
{
    public interface IPayrollProcessorRepository
    {

        Task<IEnumerable<Payroll>> GetPayrollByEmployeeIdAsync(int employeeId);
        Task<List<PayrollDTO>> FetchPayrollsByPeriodAsync(DateTime startPeriod, DateTime endPeriod);
        
        Task<bool> VerifyPayrollAsync(int payrollId);
        Task<PayrollDTO> ProcessPayrollByIdAsync(int payrollId);
        Task<(EmployeeDTO, decimal)> GetEmployeeByPayrollIdAsync(int payrollId);
       
    }
}
