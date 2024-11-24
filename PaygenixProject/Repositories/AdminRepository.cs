using Microsoft.EntityFrameworkCore;
using NewPayGenixAPI.Data;
using NewPayGenixAPI.Models;

namespace NewPayGenixAPI.Repositories
{
    public class AdminRepository : IAdminRepository
    {
            private readonly PaygenixDBContext _context;

            public AdminRepository(PaygenixDBContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
            {
                return await _context.Employees.Include(e => e.User).ToListAsync();
            }

            public async Task<Employee> GetEmployeeByIdAsync(int id)
            {
                return await _context.Employees.Include(e => e.User).FirstOrDefaultAsync(e => e.EmployeeID == id);
            }

            public async Task AddEmployeeAsync(Employee employee)
            {
                await _context.Employees.AddAsync(employee);
                await _context.SaveChangesAsync();
            }

            public async Task UpdateEmployeeAsync(Employee employee)
            {
                _context.Employees.Update(employee);
                await _context.SaveChangesAsync();
            }

            public async Task DeleteEmployeeAsync(int id)
            {
                var employee = await _context.Employees.FindAsync(id);
                if (employee == null) throw new Exception("Employee not found");

                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }

            public async Task AssignRoleToUserAsync(int userId, int roleId)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) throw new Exception("User not found");

                user.RoleID = roleId;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }

            public async Task GeneratePayrollAsync(int employeeId)
            {
                var employee = await _context.Employees.FindAsync(employeeId);
                if (employee == null) throw new Exception("Employee not found");

                // Example payroll calculation logic
                var basicSalary = 50000; // Assume static salary
                var hra = basicSalary * 0.2M;
                var deductions = basicSalary * 0.12M;
                var grossPay = basicSalary + hra;
                var netPay = grossPay - deductions;

                var payroll = new Payroll
                {
                    EmployeeID = employeeId,
                    BasicSalary = basicSalary,
                    HRA = hra,
                    Deduction = deductions,
                    GrossPay = grossPay,
                    NetPay = netPay,
                    StartPeriod = DateTime.UtcNow.AddMonths(-1),
                    EndPeriod = DateTime.UtcNow,
                    GeneratedDate = DateTime.UtcNow
                };

                await _context.Payrolls.AddAsync(payroll);
                await _context.SaveChangesAsync();
            }

            public async Task GenerateComplianceReportAsync(ComplianceReport report)
            {
                await _context.ComplianceReports.AddAsync(report);
                await _context.SaveChangesAsync();
            }
        }

    }
