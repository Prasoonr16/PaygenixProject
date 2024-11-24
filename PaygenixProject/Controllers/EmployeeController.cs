using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewPayGenixAPI.DTO;
using NewPayGenixAPI.Models;
using NewPayGenixAPI.Repositories;

namespace NewPayGenixAPI.Controllers
{
    [ApiController]
    [Route("api/employee")]
    //[Authorize(Roles = "Employee")] // Only users with the "Employee" role can access these endpoints
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        // View Personal Details
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeDetails(int id)
        {
            var employee = await _employeeRepository.GetEmployeeDetailsAsync(id);
            if (employee == null) return NotFound("Employee not found");
            return Ok(employee);
        }

        // Update Personal Information
        [HttpPut("{id}/update-info")]
        public async Task<IActionResult> UpdatePersonalInfo(int id, [FromBody] EmployeeDTO employeeDto)
        {
            var employee = new Employee
            {
                EmployeeID = id,
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                Email = employeeDto.Email,
                PhoneNumber = employeeDto.PhoneNumber,
                UserID = employeeDto.UserID
            };

            await _employeeRepository.UpdateEmployeePersonalInfoAsync(employee);
            return Ok("Employee details updated successfully");
        }

        // View Pay Stubs
        [HttpGet("{id}/pay-stubs")]
        public async Task<IActionResult> GetPayStubs(int id)
        {
            var payStubs = await _employeeRepository.GetPayStubsAsync(id);
            return Ok(payStubs);
        }

        // Submit Timesheet
        //[HttpPost("submit-timesheet")]
        //public async Task<IActionResult> SubmitTimesheet([FromBody] TimeSheetDTO timesheetDto)
        //{
        //    var timesheet = new TimeSheet
        //    {
        //        EmployeeID = timesheetDto.EmployeeID,
        //        Date = timesheetDto.Date,
        //        HoursWorked = timesheetDto.HoursWorked
        //    };

        //    await _employeeRepository.SubmitTimesheetAsync(timesheet);
        //    return CreatedAtAction(nameof(SubmitTimesheet), new { id = timesheet.EmployeeID }, timesheet);
        //}

        // Request Leave
        [HttpPost("request-leave")]
        public async Task<IActionResult> RequestLeave([FromBody] LeaveRequestDTO leaveRequestDto)
        {
            var leaveRequest = new LeaveRequest
            {
                EmployeeID = leaveRequestDto.EmployeeID,
                StartDate = leaveRequestDto.StartDate,
                EndDate = leaveRequestDto.EndDate,
                LeaveType = leaveRequestDto.LeaveType,
                Status = "Pending",
                RequestDate = DateTime.UtcNow
            };

            await _employeeRepository.RequestLeaveAsync(leaveRequest);
            return CreatedAtAction(nameof(RequestLeave), new { id = leaveRequest.LeaveRequestID }, leaveRequest);

        }
    }
}
