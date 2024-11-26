using Microsoft.EntityFrameworkCore;
using NewPayGenixAPI.Data;
using NewPayGenixAPI.Models;

namespace NewPayGenixAPI.Repositories
{
    public class PayrollProcessorRepository : IPayrollProcessorRepository
    {
            private readonly PaygenixDBContext _context;

        public PayrollProcessorRepository(PaygenixDBContext context)
        {
            _context = context;
        }

       


        public async Task<Payroll> GetPayrollByEmployeeIdAsync(int employeeId)
            {
                return await _context.Payrolls.FirstOrDefaultAsync(p => p.EmployeeID == employeeId);
            }

        public async Task VerifyPayrollAsync(int employeeId)
        {
            var payroll = await _context.Payrolls.FirstOrDefaultAsync(p => p.EmployeeID == employeeId);
            if (payroll == null) throw new Exception("Payroll not found");

            if (payroll.NetPay != payroll.GrossPay - payroll.Deduction)
            {
                throw new Exception("Payroll verification failed. NetPay calculation mismatch.");
            }
        }
    }
}

