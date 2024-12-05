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
    //[Authorize(Roles = "Admin")] // Only users with the "Admin" role can access these endpoints
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

        [HttpGet("user")]
        public async Task<IActionResult> GetAllUser()
        {
            var user = await _adminRepository.GetAllUserAsync();
            return Ok(user);
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _adminRepository.GetUserByIdAsync(id);
            if (user == null) return NotFound("User not found");
            return Ok(user);
        }

        [HttpPut("user/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDTO userDTO)
        {
            var user = await _adminRepository.GetUserByIdAsync(id);
            if (user == null) return NotFound("Employee not found");

            user.UserID = userDTO.UserID;
            user.Username = userDTO.Username;
            user.RoleID = userDTO.RoleID;

            await _adminRepository.UpdateUserAsync(user);
            return Ok("User updated successfully!");
        }

        // Add Payroll
        [HttpPost("add-payroll")]
        public async Task<IActionResult> AddPayroll([FromBody] PayrollDTO payrollDto)
        {
            var payroll = new Payroll
            {
                EmployeeID = payrollDto.EmployeeID,
                BasicSalary = payrollDto.BasicSalary,
                HRA = payrollDto.HRA,
                LTA = payrollDto.LTA,
                TravellingAllowance = payrollDto.TravellingAllowance,
                DA = payrollDto.DA,
                GrossPay = payrollDto.GrossPay,
                PF = payrollDto.PF,
                TDS = payrollDto.TDS,
                ESI = payrollDto.ESI,
                Deduction = payrollDto.Deduction,
                TaxAmount = payrollDto.TaxAmount,
                NetPay = payrollDto.NetPay,
                StartPeriod = payrollDto.StartPeriod,
                EndPeriod = payrollDto.EndPeriod,
                GeneratedDate = payrollDto.GeneratedDate
            };

            await _adminRepository.AddPayrollAsync(payroll);
            return CreatedAtAction(nameof(AddPayroll), new { id = payroll.PayrollID }, payroll);
        }
        [HttpPut("payroll/{id}")]
        public async Task<IActionResult> UpdatePayrollByEmployeeId(int id, [FromBody] PayrollDTO payrollDto)
        {
            var payroll = await _adminRepository.GetPayrollByEmployeeIdAsync(id);
            if (payroll == null) return NotFound("Payroll not found");

            payroll.BasicSalary = payrollDto.BasicSalary;
            payroll.HRA = payrollDto.HRA;
            payroll.LTA = payrollDto.LTA;
            payroll.TravellingAllowance = payrollDto.TravellingAllowance;
            payroll.DA = payrollDto.DA;
            payroll.GrossPay = payrollDto.GrossPay;
            payroll.PF = payrollDto.PF;
            payroll.TDS = payrollDto.TDS;
            payroll.ESI = payrollDto.ESI;
            payroll.Deduction = payrollDto.Deduction;
            payroll.TaxAmount = payrollDto.TaxAmount;
            payroll.NetPay = payrollDto.NetPay;
            payroll.StartPeriod = payrollDto.StartPeriod;
            payroll.EndPeriod = payrollDto.EndPeriod;

            await _adminRepository.UpdatePayrollAsync(payroll);
            return Ok("Payroll updated successfully!");
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
            };

            await _adminRepository.GenerateComplianceReportAsync(report);
            return CreatedAtAction(nameof(GenerateComplianceReport), new { id = report.ReportID }, report);
        }

    }
}
