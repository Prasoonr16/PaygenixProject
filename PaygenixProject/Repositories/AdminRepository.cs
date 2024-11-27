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
                return await _context.Employees.ToListAsync();
            }
           
            public async Task<Employee> GetEmployeeByIdAsync(int id)
            {
                return await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeID == id);
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

        public async Task<IEnumerable<User>> GetAllUserAsync()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserID == id);
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task AddPayrollAsync(Payroll payroll)
        {
            await _context.Payrolls.AddAsync(payroll);
            await _context.SaveChangesAsync();
        }

        public async Task<Payroll> GetPayrollByEmployeeIdAsync(int id)
        {
            return await _context.Payrolls.FirstOrDefaultAsync(p => p.EmployeeID == id);
        }

        public async Task UpdatePayrollAsync(Payroll payroll)
        {
            _context.Payrolls.Update(payroll);
            _context.SaveChanges();
        }
        public async Task GenerateComplianceReportAsync(ComplianceReport report)
            {
                await _context.ComplianceReports.AddAsync(report);
                await _context.SaveChangesAsync();
            }
        }

    }
