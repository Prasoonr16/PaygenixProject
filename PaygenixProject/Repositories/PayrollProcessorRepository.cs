using Microsoft.EntityFrameworkCore;
using NewPayGenixAPI.Data;
using NewPayGenixAPI.DTO;
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

        public async Task<Payroll> ProcessPayrollAsync(int employeeId, PayrollDTO payrollDto)
        {
            // Find the employee
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null) throw new Exception("Employee not found");

            // Calculate payroll details
            var grossPay = payrollDto.BasicSalary + payrollDto.HRA + payrollDto.LTA + payrollDto.TravellingAllowance;
            var totalDeductions = payrollDto.PF + payrollDto.TDS + payrollDto.ESI;
            var netPay = grossPay - totalDeductions;

            // Create a new payroll entry
            var payroll = new Payroll
            {
                EmployeeID = employeeId,
                BasicSalary = payrollDto.BasicSalary,
                HRA = payrollDto.HRA,
                LTA = payrollDto.LTA,
                TravellingAllowance = payrollDto.TravellingAllowance,
                DA = payrollDto.DA,
                GrossPay = grossPay,
                PF = payrollDto.PF,
                TDS = payrollDto.TDS,
                ESI = payrollDto.ESI,
                Deduction = totalDeductions,
                NetPay = netPay,
                StartPeriod = payrollDto.StartPeriod,
                EndPeriod = payrollDto.EndPeriod,
                GeneratedDate = DateTime.UtcNow
            };

            // Save to the database
            _context.Payrolls.Update(payroll);
            await _context.SaveChangesAsync();

            return payroll;
        }
        public async Task<bool> VerifyPayrollAsync(int payrollId)
        {
            // Find the payroll entry
            var payroll = await _context.Payrolls.FindAsync(payrollId);
            if (payroll == null) throw new Exception("Payroll not found");

            // Business logic to verify payroll
            if (payroll.NetPay == (payroll.GrossPay - payroll.Deduction))
            {
                payroll.GeneratedDate = DateTime.UtcNow.Date;
                _context.Payrolls.Update(payroll);
                await _context.SaveChangesAsync();
                return true;
            }

            return false; // Verification failed
        }
        //public async Task VerifyPayrollAsync(int employeeId)
        //{
        //    var payroll = await _context.Payrolls.FirstOrDefaultAsync(p => p.EmployeeID == employeeId);
        //    if (payroll == null) throw new Exception("Payroll not found");

        //    if (payroll.NetPay != payroll.GrossPay - payroll.Deduction)
        //    {
        //        throw new Exception("Payroll verification failed. NetPay calculation mismatch.");
        //    }
        //}
    }
}

