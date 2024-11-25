using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewPayGenixAPI.DTO;
using NewPayGenixAPI.Models;
using NewPayGenixAPI.Repositories;

namespace NewPayGenixAPI.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")] // Only users with the "Admin" role can access these endpoints
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _adminRepository;

        public AdminController(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        // Manage Employee Information
        [HttpGet("employees")]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _adminRepository.GetAllEmployeesAsync();
            return Ok(employees);
        }

        [HttpGet("employee/{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _adminRepository.GetEmployeeByIdAsync(id);
            if (employee == null) return NotFound("Employee not found");
            return Ok(employee);
        }

        [HttpPost("employee")]
        public async Task<IActionResult> AddEmployee([FromBody] EmployeeDTO employeeDto)
        {
            var employee = new Employee
            {
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                Email = employeeDto.Email,
                PhoneNumber = employeeDto.PhoneNumber,
                Position = employeeDto.Position,
                Department = employeeDto.Department,
                HireDate = employeeDto.HireDate
            };

            await _adminRepository.AddEmployeeAsync(employee);
            return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.EmployeeID }, employee);
        }

        [HttpPut("employee/{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeDTO employeeDto)
        {
            var employee = await _adminRepository.GetEmployeeByIdAsync(id);
            if (employee == null) return NotFound("Employee not found");

            employee.FirstName = employeeDto.FirstName;
            employee.LastName = employeeDto.LastName;
            employee.Email = employeeDto.Email;
            employee.PhoneNumber = employeeDto.PhoneNumber;
            employee.Position = employeeDto.Position;
            employee.Department = employeeDto.Department;

            await _adminRepository.UpdateEmployeeAsync(employee);
            return Ok("Employee updated successfully!");
        }

        [HttpDelete("employee/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            await _adminRepository.DeleteEmployeeAsync(id);
            return Ok("Employee deleted successfully!");
        }

        //// Manage User Roles
        //[HttpPost("assign-role")]
        //public async Task<IActionResult> AssignRoleToUser([FromBody] RoleAssignmentDTO roleAssignmentDto)
        //{
        //    await _adminRepository.AssignRoleToUserAsync(roleAssignmentDto.UserId, roleAssignmentDto.RoleId);
        //    return Ok("Role assigned successfully");
        //}

        // Generate Payroll
        [HttpPost("generate-payroll/{employeeId}")]
        public async Task<IActionResult> GeneratePayroll(int employeeId)
        {
            await _adminRepository.GeneratePayrollAsync(employeeId);
            return Ok($"Payroll for Employee ID {employeeId} generated successfully.");
        }

        // Generate Compliance Report
        [HttpPost("generate-compliance-report")]
        public async Task<IActionResult> GenerateComplianceReport([FromBody] ComplianceReportDTO reportDto)
        {
            var report = new ComplianceReport
            {
                ReportDate = DateTime.UtcNow,
                EmployeeID = reportDto.EmployeeID,
                ComplianceStatus = reportDto.ComplianceStatus,
                IssuesFound = reportDto.IssuesFound,
                ResolvedStatus = reportDto.ResolvedStatus,
                Comments = reportDto.Comments,
                GeneratedBy = reportDto.GeneratedBy
            };

            await _adminRepository.GenerateComplianceReportAsync(report);
            return CreatedAtAction(nameof(GenerateComplianceReport), new { id = report.ReportID }, report);
        }

    }
}
