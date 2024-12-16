using NewPayGenixAPI.DTO;
using NewPayGenixAPI.Models;

namespace NewPayGenixAPI.Repositories
{
    public interface IPayrollProcessorRepository
    {

        Task<IEnumerable<Payroll>> GetPayrollByEmployeeIdAsync(int employeeId);
        Task<List<PayrollDTO>> FetchPayrollsByPeriodAsync(DateTime startPeriod, DateTime endPeriod);
        //Task VerifyPayrollAsync(int employeeId);
        //Task<Payroll> ProcessPayrollAsync(int employeeId, PayrollDTO payrollDto);
        Task<bool> VerifyPayrollAsync(int payrollId);
        Task<PayrollDTO> ProcessPayrollByIdAsync(int payrollId);
        Task<(EmployeeDTO, decimal)> GetEmployeeByPayrollIdAsync(int payrollId);
        //void ProcessPayrollAsync(int employeeId, PayrollDTO payrollDto);
        //Task<List<Payroll>> ProcessPayrollsAsync(DateTime startPeriod, DateTime endPeriod);
    }
}
