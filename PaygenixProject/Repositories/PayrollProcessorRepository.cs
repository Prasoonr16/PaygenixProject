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

       


        public async Task<IEnumerable<Payroll>> GetPayrollByEmployeeIdAsync(int employeeId)
            {
                return await _context.Payrolls.Where(p => p.EmployeeID == employeeId).ToListAsync();
            }

        
        public async Task<List<PayrollDTO>> FetchPayrollsByPeriodAsync(DateTime startPeriod, DateTime endPeriod)
        {
            // Fetch payrolls within the specified date range
            var payrolls = await _context.Payrolls
                .Where(p => p.StartPeriod >= startPeriod && p.EndPeriod <= endPeriod)
                .Include(p => p.Employee) // Include Employee navigation property
                .ToListAsync();

            // Map payrolls to PayrollDTO
            return payrolls.Select(p => new PayrollDTO
            {
                PayrollID = p.PayrollID,
                EmployeeID = p.EmployeeID,
                BasicSalary = p.BasicSalary,
                HRA = p.HRA,
                LTA = p.LTA,
                TravellingAllowance = p.TravellingAllowance,
                DA = p.DA,
                GrossPay = p.GrossPay,
                PF = p.PF,
                TDS = p.TDS,
                ESI = p.ESI,
                Deduction = p.Deduction,
                NetPay = p.NetPay,
                StartPeriod = p.StartPeriod,
                EndPeriod = p.EndPeriod,
                GeneratedDate = p.GeneratedDate
            }).ToList();
        }

        public async Task<PayrollDTO> ProcessPayrollByIdAsync(int payrollId)
        {
            // Fetch the payroll entry
            var payroll = await _context.Payrolls
                .Include(p => p.Employee) // Include Employee details
                .FirstOrDefaultAsync(p => p.PayrollID == payrollId);

            if (payroll == null)
                throw new Exception($"Payroll with ID {payrollId} not found.");

            // Business logic to process the payroll
            var grossPay = payroll.BasicSalary + payroll.HRA + payroll.LTA + payroll.TravellingAllowance;
            var esi = 0.075m * grossPay;
            var totalDeductions = payroll.PF + payroll.TDS + esi;
            var netPay = grossPay - totalDeductions;

            payroll.GrossPay = grossPay;
            payroll.ESI = esi;
            payroll.Deduction = totalDeductions;
            payroll.NetPay = netPay;
            payroll.GeneratedDate = DateTime.Now;

            // Save the updated payroll
            _context.Payrolls.Update(payroll);
            await _context.SaveChangesAsync();
            return new PayrollDTO
            {
                PayrollID = payroll.PayrollID,
                EmployeeID = payroll.EmployeeID,
                BasicSalary = payroll.BasicSalary,
                HRA = payroll.HRA,
                LTA = payroll.LTA,
                TravellingAllowance = payroll.TravellingAllowance,
                DA = payroll.DA,
                GrossPay = payroll.GrossPay,
                PF = payroll.PF,
                TDS = payroll.TDS,
                ESI = payroll.ESI,
                Deduction = payroll.Deduction,
                NetPay = payroll.NetPay,
                StartPeriod = payroll.StartPeriod,
                EndPeriod = payroll.EndPeriod,
                GeneratedDate = payroll.GeneratedDate
            };
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
        public async Task<(EmployeeDTO, decimal)> GetEmployeeByPayrollIdAsync(int payrollId)
        {
            // Fetch the payroll with associated employee details
            var payroll = await _context.Payrolls
                .Include(p => p.Employee) // Include the Employee navigation property
                .FirstOrDefaultAsync(p => p.PayrollID == payrollId);

            if (payroll == null || payroll.Employee == null)
                throw new Exception("Payroll or associated employee not found.");

            // Map Employee entity to EmployeeDTO and return NetPay
            var employeeDto = new EmployeeDTO
            {
                EmployeeID = payroll.Employee.EmployeeID,
                FirstName = payroll.Employee.FirstName,
                LastName = payroll.Employee.LastName,
                Email = payroll.Employee.Email,
                PhoneNumber = payroll.Employee.PhoneNumber,
                Position = payroll.Employee.Position,
                Department = payroll.Employee.Department,
                HireDate = payroll.Employee.HireDate,
                ActiveStatus = payroll.Employee.ActiveStatus,
                UserID = payroll.Employee.UserID
            };

            return (employeeDto, payroll.NetPay);
        }
    }
}

