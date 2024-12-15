using NewPayGenixAPI.DTO;
using NewPayGenixAPI.Models;

namespace NewPayGenixAPI.Repositories
{
    public interface IPayrollProcessorRepository
    {

        Task<IEnumerable<Payroll>> GetPayrollByEmployeeIdAsync(int employeeId);
        //Task VerifyPayrollAsync(int employeeId);
        Task<Payroll> ProcessPayrollAsync(int employeeId, PayrollDTO payrollDto);
        Task<bool> VerifyPayrollAsync(int payrollId);
        Task<EmployeeDTO> GetEmployeeByPayrollIdAsync(int payrollId);
    }
}
