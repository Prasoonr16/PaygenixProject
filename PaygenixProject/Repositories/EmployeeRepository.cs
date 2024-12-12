using Microsoft.EntityFrameworkCore;
using NewPayGenixAPI.Data;
using NewPayGenixAPI.Models;

namespace NewPayGenixAPI.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
            private readonly PaygenixDBContext _context;

            public EmployeeRepository(PaygenixDBContext context)
            {
                _context = context;
            }

            public async Task<Employee> GetEmployeeDetailsAsync(int employeeId)
            {
                return await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeID == employeeId);
            }


        public async Task UpdateEmployeePersonalInfoAsync(Employee employee)
            {
                var existingEmployee = await _context.Employees.FindAsync(employee.EmployeeID);
                if (existingEmployee == null) throw new Exception("Employee not found");

                existingEmployee.FirstName = employee.FirstName;
                existingEmployee.LastName = employee.LastName;
                existingEmployee.Email = employee.Email;
                existingEmployee.PhoneNumber = employee.PhoneNumber;
                existingEmployee.UserID = employee.UserID;

                _context.Employees.Update(existingEmployee);
                await _context.SaveChangesAsync();
            }

            public async Task<IEnumerable<Payroll>> GetPayStubsAsync(int employeeId)
            {
                return await _context.Payrolls.Where(p => p.EmployeeID == employeeId).ToListAsync();
            }

            //public async Task SubmitTimesheetAsync(TimeSheet timesheet)
            //{
            //    await _context.TimeSheets.AddAsync(timesheet);
            //    await _context.SaveChangesAsync();
            //}

            public async Task RequestLeaveAsync(LeaveRequest leaveRequest)
            {
                await _context.LeaveRequests.AddAsync(leaveRequest);
                await _context.SaveChangesAsync();
            }

        public async Task GenerateComplianceReportAsync(ComplianceReport report)
        {
            await _context.ComplianceReports.AddAsync(report);
            await _context.SaveChangesAsync();
        }

        //---------------------------------------------------------------------//
        public async Task<Employee> GetEmployeeDetailsByUserIDAsync(int userId)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.UserID == userId);
        }

        public async Task<IEnumerable<Payroll>> GetPayStubsByUserIDAsync(int userId)
        {
            // Fetch EmployeeID from the Employee table where Employee.UserID matches the given UserID
            var employeeId = await _context.Employees
                .Where(e => e.UserID == userId)
                .Select(e => e.EmployeeID)
                .FirstOrDefaultAsync();

            if (employeeId == 0)
                throw new Exception("Employee not found for the given UserID.");

            // Fetch Payroll records for the fetched EmployeeID
            return await _context.Payrolls
                .Where(p => p.EmployeeID == employeeId)
                .ToListAsync();
        }

    }

}
