using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPayGenixAPI.DTO;
using NewPayGenixAPI.Models;
using NewPayGenixAPI.Repositories;
using PaygenixProject.Models;

namespace NewPayGenixAPI.Controllers

{
    [ApiController]
    [Route("api/employee")]
    //[Authorize(Roles = "Employee")] // Only users with the "Employee" role can access these endpoints
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAdminRepository _adminRepository;

        public EmployeeController(IEmployeeRepository employeeRepository, IAdminRepository adminRepository)
        {
            _employeeRepository = employeeRepository;
            _adminRepository = adminRepository;
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
            // Log the start of the update process
            await _adminRepository.LogAuditTrailAsync(new AuditTrail
            {
                Action = "Update Personal Information Attempt",
                PerformedBy = "Employee",
                Timestamp = DateTime.Now,
                Details = $"Attempting to update personal information for EmployeeID {id}."
            });
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

                // Log success
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Update Personal Information Success",
                    PerformedBy = "Employee",
                    Timestamp = DateTime.Now,
                    Details = $"Personal information updated successfully for EmployeeID {id}. Updated Email: {employeeDto.Email}, Phone: {employeeDto.PhoneNumber}"
                });
               
                return Ok("Employee details updated successfully");
                var existingEmployee = await _employeeRepository.GetEmployeeDetailsAsync(id);

                if (existingEmployee == null)
                {
                    var employee = new Employee
                    {
                        EmployeeID = employeeDto.EmployeeID,
                        FirstName = employeeDto.FirstName,
                        LastName = employeeDto.LastName,
                        Email = employeeDto.Email,
                        PhoneNumber = employeeDto.PhoneNumber,
                        UserID = employeeDto.UserID
                        
                    };
                    await _employeeRepository.AddEmployeeAsync(employee);
                    return Ok("Employee added successfully.");
                }
                else
                {
                    existingEmployee.FirstName = employeeDto.FirstName;
                    existingEmployee.LastName = employeeDto.LastName;
                    existingEmployee.Email = employeeDto.Email;
                    existingEmployee.PhoneNumber = employeeDto.PhoneNumber;
                    existingEmployee.UserID = employeeDto.UserID;

                    await _employeeRepository.UpdateEmployeePersonalInfoAsync(existingEmployee);
                    return Ok("Employee details updated successfully");
                }
            }
            catch (Exception ex)
            {
                // Log any exception that occurs
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Update Personal Information Error",
                    PerformedBy = "Employee",
                    Timestamp = DateTime.Now,
                    Details = $"Error updating personal information for EmployeeID {id}: {ex.Message}"
                });

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
            // Log the start of the leave request creation
            await _adminRepository.LogAuditTrailAsync(new AuditTrail
            {
                Action = "Request Leave Attempt",
                PerformedBy = "Employee",
                Timestamp = DateTime.Now,
                Details = $"Attempting to request leave for EmployeeID {leaveRequestDto.EmployeeID}."
            });
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
                //return CreatedAtAction(nameof(RequestLeave), new{ id = leaveRequest.LeaveRequestID }, leaveRequest);
                // // Log success
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Request Leave Success",
                    PerformedBy = "Employee",
                    Timestamp = DateTime.Now,
                    Details = $"Leave requested successfully for EmployeeID {leaveRequestDto.EmployeeID}. Leave Type: {leaveRequestDto.LeaveType}, Start Date: {leaveRequestDto.StartDate}, End Date: {leaveRequestDto.EndDate}"
                });
                    

                return Ok("Leave Requested");
            }
            catch (Exception ex)
            {
                // Log error if any exception occurs
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Request Leave Error",
                    PerformedBy = "Employee",
                    Timestamp = DateTime.Now,
                    Details = $"Error requesting leave for EmployeeID {leaveRequestDto.EmployeeID}. Error: {ex.Message}"
                });

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Generate Compliance Report
        [HttpPost("generate-compliance-report")]
        public async Task<IActionResult> GenerateComplianceReport([FromBody] ComplianceReportDTO reportDto)
        {
            // Log the attempt to generate a compliance report
            await _adminRepository.LogAuditTrailAsync(new AuditTrail
            {
                Action = "Generate Compliance Report Attempt",
                PerformedBy = "Employee",
                Timestamp = DateTime.Now,
                Details = $"Attempting to generate compliance report for EmployeeID {reportDto.EmployeeID}."
            });

            try
            {
                //reportDto.ComplianceStatus = "Pending";
                //reportDto.ResolvedStatus = "Pending";
                var report = new ComplianceReport
                {
                    ReportDate = DateTime.UtcNow,
                    EmployeeID = reportDto.EmployeeID,
                    StartPeriod = reportDto.StartPeriod,
                    EndPeriod = reportDto.EndPeriod,
                    ComplianceStatus = "Pending",
                    IssuesFound = reportDto.IssuesFound,
                    ResolvedStatus = "Pending",
                    Comments = reportDto.Comments
                };

                await _employeeRepository.GenerateComplianceReportAsync(report);

                // Log success
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Generate Compliance Report Success",
                    PerformedBy = "Employee",
                    Timestamp = DateTime.Now,
                    Details = $"Compliance report successfully generated for EmployeeID {reportDto.EmployeeID}. Start Period: {reportDto.StartPeriod}, End Period: {reportDto.EndPeriod}. Issues Found: {reportDto.IssuesFound}"
                });

               
                return CreatedAtAction(nameof(GenerateComplianceReport), new { id = report.ReportID }, report);

            }
            catch (Exception ex)
            {
                // Log error if any exception occurs
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Generate Compliance Report Error",
                    PerformedBy = "Employee",
                    Timestamp = DateTime.Now,
                    Details = $"Error generating compliance report for EmployeeID {reportDto.EmployeeID}. Error: {ex.Message}"

                });

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //------------------------------------------------------------//
        // View Personal Details based on User ID
        [HttpGet("userid/{id}")]
        public async Task<IActionResult> GetEmployeeDetailsByUserID(int id)
        {
            try
            {
                var employee = await _employeeRepository.GetEmployeeDetailsByUserIDAsync(id);
                if (employee == null) return NotFound("Employee not found");
                return Ok(employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // View Pay Stubs By UserID
        [HttpGet("{userid}/pay-stubs-by-userid")]
        public async Task<IActionResult> GetPayStubsByUserID(int userid)
        {
            try
            {
                var payStubs = await _employeeRepository.GetPayStubsByUserIDAsync(userid);
                return Ok(payStubs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{employeeId}/leave-requests")]
        public async Task<IActionResult> GetLeaveRequestsByEmployeeId(int employeeId)
        {
            try
            {
                // Fetch leave requests via repository
                var leaveRequests = await _employeeRepository.GetLeaveRequestsByEmployeeIdAsync(employeeId);

                if (leaveRequests == null || leaveRequests.Count == 0)
                {
                    return NotFound($"No leave requests found for EmployeeID {employeeId}.");
                }

                return Ok(leaveRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

            //View leave Request by userID

            [HttpGet("leave-requests/{userId}")]
            public async Task<IActionResult> GetLeaveRequestsByUserId(int userId)
            {
                try
                {
                    // Fetch leave requests via repository
                    var leaveRequests = await _employeeRepository.GetLeaveRequestsByUserIdAsync(userId);

                    if (leaveRequests == null || leaveRequests.Count == 0)
                    {
                        return NotFound($"No leave requests found.");
                    }

                    return Ok(leaveRequests);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
        }
    }