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

        //public async Task<IEnumerable<Payroll>> GetTeamPayrollsAsync(int managerId)
        //{
        //    return await _context.Payrolls
        //        .Include(p => p.Employee)
        //        .Where(p => p.Employee.ManagerID == managerId)
        //        .ToListAsync();
        //}

        //public async Task<IEnumerable<LeaveRequest>> GetPendingLeaveRequestsAsync(int managerId)
        //{
        //    return await _context.LeaveRequests
        //        .Include(lr => lr.Employee)
        //        .Where(lr => lr.Employee.ManagerID == managerId && lr.Status == "Pending")
        //        .ToListAsync();
        //}


        public async Task ApproveLeaveRequestAsync(int leaveRequestId, bool isApproved)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(leaveRequestId);
            if (leaveRequest == null) throw new Exception("Leave Request not found");

            leaveRequest.Status = isApproved ? "Approved" : "Rejected";
            leaveRequest.ApprovalDate = DateTime.UtcNow;

            _context.LeaveRequests.Update(leaveRequest);
            await _context.SaveChangesAsync();
        }
    }

}