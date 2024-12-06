﻿using Microsoft.AspNetCore.Authorization;
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
            try
            {
                var employee = await _employeeRepository.GetEmployeeDetailsAsync(id);
                if (employee == null) return NotFound("Employee not found");
                return Ok(employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Update Personal Information
        [HttpPut("{id}/update-info")]
        public async Task<IActionResult> UpdatePersonalInfo(int id, [FromBody] EmployeeDTO employeeDto)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // View Pay Stubs
        [HttpGet("{id}/pay-stubs")]
        public async Task<IActionResult> GetPayStubs(int id)
        {
            try
            {
                var payStubs = await _employeeRepository.GetPayStubsAsync(id);
                return Ok(payStubs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Request Leave
        [HttpPost("request-leave")]
        public async Task<IActionResult> RequestLeave([FromBody] LeaveRequestDTO leaveRequestDto)
        {
            try
            {
                var leaveRequest = new LeaveRequest
                {
                    EmployeeID = leaveRequestDto.EmployeeID,
                    StartDate = leaveRequestDto.StartDate,
                    EndDate = leaveRequestDto.EndDate,
                    LeaveType = leaveRequestDto.LeaveType,
                    Status = "Pending",
                    RequestDate = DateTime.UtcNow,
                };

                await _employeeRepository.RequestLeaveAsync(leaveRequest);
                //return CreatedAtAction(nameof(RequestLeave), new { id = leaveRequest.LeaveRequestID }, leaveRequest);
                return Ok("Leave Requested");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Generate Compliance Report
        [HttpPost("generate-compliance-report")]
        public async Task<IActionResult> GenerateComplianceReport([FromBody] ComplianceReportDTO reportDto)
        {
            try
            {
                var report = new ComplianceReport
                {
                    EmployeeID = reportDto.EmployeeID,
                    ComplianceStatus = "Pending",
                    IssuesFound = reportDto.IssuesFound,
                    ResolvedStatus = "Pending",
                    Comments = reportDto.Comments,
                };

                await _employeeRepository.GenerateComplianceReportAsync(report);
                return CreatedAtAction(nameof(GenerateComplianceReport), new { id = report.ReportID }, report);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
