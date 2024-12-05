using Microsoft.EntityFrameworkCore;
using NewPayGenixAPI.Data;
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

        //public async Task<IEnumerable<LeaveRequest>> GetPendingLeaveRequestsAsync(int managerId)
        //{
        //    return await _context.LeaveRequests
        //        .Include(lr => lr.Employee)
        //        .Where(lr => lr.Employee.ManagerID == managerId && lr.Status == "Pending")
        //        .ToListAsync();
        //}

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
            leaveRequest.ApprovalDate = DateTime.UtcNow; // Set approval date when status is updated
            _context.LeaveRequests.Update(leaveRequest);
            await _context.SaveChangesAsync();
        }
        //public async Task ApproveLeaveRequestAsync(int leaveRequestId, bool isApproved)
        //{
        //    var emp = await _context.LeaveRequests.FindAsync();
        //    if (leaveRequest == null) throw new Exception("Leave Request not found");

        //    leaveRequest.Status = isApproved ? "Approved" : "Rejected";
        //    leaveRequest.ApprovalDate = DateTime.UtcNow;

        //    _context.LeaveRequests.Update(leaveRequest);
        //    await _context.SaveChangesAsync();
        //}
    }

}