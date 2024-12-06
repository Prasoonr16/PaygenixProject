﻿using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        //private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        // Manage Employee Information
        [HttpGet("employees")]
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                var employees = await _adminRepository.GetAllEmployeesAsync();
                return Ok(employees);
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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
            try
            {
                var employee = await _adminRepository.GetEmployeeByIdAsync(id);
                if (employee == null) return NotFound("Employee not found");

                employee.FirstName = employeeDto.FirstName;
                employee.LastName = employeeDto.LastName;
                employee.Email = employeeDto.Email;
                employee.PhoneNumber = employeeDto.PhoneNumber;
                employee.Position = employeeDto.Position;
                employee.Department = employeeDto.Department;
                employee.HireDate = employeeDto.HireDate;
                employee.ActiveStatus= employeeDto.ActiveStatus;

                await _adminRepository.UpdateEmployeeAsync(employee);
                return Ok("Employee updated successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("employee/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                await _adminRepository.DeleteEmployeeAsync(id);
                return Ok("Employee deleted successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetAllUser()
        {
            try
            {
                var user = await _adminRepository.GetAllUserAsync();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _adminRepository.GetUserByIdAsync(id);
                if (user == null) return NotFound("User not found");
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("user/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDTO userDTO)
        {
            try
            {
                var user = await _adminRepository.GetUserByIdAsync(id);
                if (user == null) return NotFound("Employee not found");

                user.UserID = userDTO.UserID;
                user.Username = userDTO.Username;
                user.RoleID = userDTO.RoleID;

                await _adminRepository.UpdateUserAsync(user);
                return Ok("User updated successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Add Payroll
        [HttpPost("add-payroll")]
        public async Task<IActionResult> AddPayroll([FromBody] PayrollDTO payrollDto)
        {
            try {
                var payroll = new Payroll
                {
                    EmployeeID = payrollDto.EmployeeID,
                    BasicSalary = payrollDto.BasicSalary,
                    HRA = (20/100)*payrollDto.BasicSalary,
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPut("payroll/{id}")]
        public async Task<IActionResult> UpdatePayrollByEmployeeId(int id, [FromBody] PayrollDTO payrollDto)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/admin/compliance-reports
        [HttpGet("compliance-reports")]
        public async Task<IActionResult> GetAllComplianceReports()
        {
            try
            {
                var reports = await _adminRepository.GetAllComplianceReportAsync();
                return Ok(reports);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("compliance-reports/{employeeId}")]
        public async Task<IActionResult> UpdateComplianceReport(int employeeId, [FromBody] ComplianceReportDTO updateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return 400 if input is invalid
            }

            try
            {
                var isUpdated = await _adminRepository.UpdateComplianceReportAsync(employeeId, updateDTO);
                if (!isUpdated)
                {
                    return NotFound($"Compliance report for EmployeeID {employeeId} not found.");
                }

                return Ok($"Compliance report for EmployeeID {employeeId} updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




        //[HttpGet("logs")]
        //public IActionResult GetLogs([FromQuery] string? date)
        //{
        //    //Determine log file path
        //    var logDirectory = Path.Combine(_webHostEnvironment.ContentRootPath, "Logs");
        //    string logFileName = string.IsNullOrWhiteSpace(date)
        //        ? "api-log.txt" //Default to the current log file
        //        : $"api-log-{date}"; //Log file for a specific date

        //    var logFilePath = Path.Combine(logDirectory, logFileName);

        //    //Check if the log file exists
        //    if(!System.IO.File.Exists(logFilePath))
        //    {
        //        return NotFound($"Log file not found for the specified date: {date}");
        //    }

        //    var logContent = System.IO.File.ReadAllText(logFilePath);

        //    return Ok(new
        //    {
        //        FileName = logFileName,
        //        Content = logContent
        //    });
        //}
    }
}

