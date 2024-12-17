using Microsoft.EntityFrameworkCore;
using NewPayGenixAPI.Data;
using NewPayGenixAPI.DTO;
using NewPayGenixAPI.Models;

namespace NewPayGenixAPI.Repositories
{
    public class ManagerRepository : IManagerRepository
    {
        private readonly PaygenixDBContext _context;

        public ManagerRepository(PaygenixDBContext context)


        {
            _context = context;
        }

        public async Task<IEnumerable<Payroll>> GetTeamPayrollsAsync()
        {
            return await _context.Payrolls.ToListAsync();
        }

       

        // Get all leave requests for a specific employee
        public async Task<IEnumerable<LeaveRequest>> GetLeaveRequestsByEmployeeIdAsync(int employeeId)
        {
            return await _context.LeaveRequests
                .Where(lr => lr.EmployeeID == employeeId)
                .ToListAsync();
        }
        // Update leave request status
        public async Task UpdateLeaveRequestStatusAsync(int leaveRequestId, string status)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(leaveRequestId);
            if (leaveRequest == null) throw new Exception("Leave request not found");

            leaveRequest.Status = status;
            leaveRequest.ApprovalDate = DateTime.Now; 
            _context.LeaveRequests.Update(leaveRequest);
            await _context.SaveChangesAsync();
        }
       
        public async Task<List<LeaveRequest>> GetAllLeaveRequestsAsync()
        {
            try
            {
                return await _context.LeaveRequests.ToListAsync(); // Fetch all leave requests
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching leave requests: {ex.Message}");
            }
        }

        
        public async Task<List<Employee>> GetEmployeesByManagerAsync(int managerUserId)
        {
            // Verify if the user is a manager
            var isManager = await _context.Users
                .AnyAsync(u => u.UserID == managerUserId && u.RoleID == 3); // RoleID 2 = Manager

            if (!isManager)
                throw new Exception("The specified user is not a manager.");

            // Fetch employees managed by this manager
            return await _context.Employees
                .Where(e => e.ManagerUserID == managerUserId)
                .ToListAsync();
        }

        public async Task<List<PayrollDTO>> GetPayrollsByManagerAsync(int managerUserId)
        {
            // Get Employee IDs managed by this manager
            var employeeIds = await _context.Employees
                .Where(e => e.ManagerUserID == managerUserId)
                .Select(e => e.EmployeeID)
                .ToListAsync();

            if (!employeeIds.Any())
                throw new Exception("No employees found under this manager.");

            // Fetch payrolls for these employees
            var payrolls = await _context.Payrolls
                .Where(p => employeeIds.Contains(p.EmployeeID))
                .ToListAsync();

            // Map to DTO
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
        public async Task<List<LeaveRequestDTO>> GetLeaveRequestsByManagerAsync(int managerUserId)
        {
            // Get Employee IDs managed by this manager
            var employeeIds = await _context.Employees
                .Where(e => e.ManagerUserID == managerUserId)
                .Select(e => e.EmployeeID)
                .ToListAsync();

            if (!employeeIds.Any())
                throw new Exception("No employees found under this manager.");

            // Fetch leave requests for these employees
            var leaveRequests = await _context.LeaveRequests
                .Where(lr => employeeIds.Contains(lr.EmployeeID))
                .ToListAsync();

            // Map to DTO
            return leaveRequests.Select(lr => new LeaveRequestDTO
            {
                LeaveRequestID = lr.LeaveRequestID,
                EmployeeID = lr.EmployeeID,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                LeaveType = lr.LeaveType,
                Status = lr.Status,
                RequestDate = lr.RequestDate,
                ApprovalDate = lr.ApprovalDate
            }).ToList();
        }

    }


}