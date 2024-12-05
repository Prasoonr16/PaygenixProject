using NewPayGenixAPI.Models;

namespace NewPayGenixAPI.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Employee> GetEmployeeDetailsAsync(int employeeId);
        Task UpdateEmployeePersonalInfoAsync(Employee employee);
        Task<IEnumerable<Payroll>> GetPayStubsAsync(int employeeId);
        //Task SubmitTimesheetAsync(TimeSheet timesheet);
        Task RequestLeaveAsync(LeaveRequest leaveRequest);

        Task GenerateComplianceReportAsync(ComplianceReport report);
    }
}
