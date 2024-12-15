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

        public async Task<Payroll> ProcessPayrollAsync(int employeeId, PayrollDTO payrollDto)
        {
            // Find the employee
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null) throw new Exception("Employee not found");

            // Calculate payroll details
            var grossPay = payrollDto.BasicSalary + payrollDto.HRA + payrollDto.LTA + payrollDto.TravellingAllowance;
            var esi = 0.075m * grossPay;
            var totalDeductions = payrollDto.PF + payrollDto.TDS + esi;
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
                ESI = esi,
                Deduction = totalDeductions,
                NetPay = netPay,
                StartPeriod = payrollDto.StartPeriod,
                EndPeriod = payrollDto.EndPeriod,
                GeneratedDate = DateTime.Now
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

