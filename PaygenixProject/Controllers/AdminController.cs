using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPayGenixAPI.DTO;
using NewPayGenixAPI.Models;
using NewPayGenixAPI.Repositories;
using PaygenixProject.DTO;
using PaygenixProject.Models;
using PaygenixProject.Repositories;

namespace NewPayGenixAPI.Controllers
{
    [ApiController]
    [Route("api/admin")]
    //[Authorize(Roles = "Admin")] // Only users with the "Admin" role can access these endpoints
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _adminRepository;
        private readonly EmailService _emailService;

        //private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(IAdminRepository adminRepository, EmailService emailService)
        {
            _adminRepository = adminRepository;
            _emailService = emailService;
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("employee/{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeDTO employeeDto)
        {
            try
            {
                var employee = await _adminRepository.GetEmployeeByIdAsync(id);
                if (employee == null)
                {
                    // Log employee not found
                    await _adminRepository.LogAuditTrailAsync(new AuditTrail
                    {
                        Action = "Employee Update Failed",
                        PerformedBy = "Admin", // Use the current user's name or ID
                        Timestamp = DateTime.Now,
                        Details = $"Employee with ID {id} not found.",
                    });
                    return NotFound("Employee not found");
                }

                // Log before updating employee details
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Employee Update Attempt",
                    PerformedBy = "Admin",
                    Timestamp = DateTime.Now,
                    Details = $"Employee ID {id} update Attempt"
                });

                employee.Position = employeeDto.Position;
                employee.Department = employeeDto.Department;
                employee.HireDate = employeeDto.HireDate.Date;
                employee.ActiveStatus = employeeDto.ActiveStatus;

                await _adminRepository.UpdateEmployeeAsync(employee);

                // Log successful update
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Employee Updated Successfully",
                    PerformedBy = "Admin",
                    Timestamp = DateTime.Now,
                    Details = $"Employee {id} updated successfully. Position: {employeeDto.Position}, Department: {employeeDto.Department}"
                });

                return Ok("Employee updated successfully!");
            }
            catch (Exception ex)
            {
                // Log exception
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Employee Update Error",
                    PerformedBy = "Admin",
                    Timestamp = DateTime.Now,
                    Details = $"Error updating employee {id}: {ex.Message}"            
                });
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("employee/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                // Log the deletion attempt
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Employee Deletion Attempt",
                    PerformedBy = "Admin",  // You can use the currently logged-in user
                    Timestamp = DateTime.Now,
                    Details = $"Attempting to delete employee with ID {id}"
                });
                await _adminRepository.DeleteEmployeeAsync(id);

                // Log the successful deletion
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Employee Deleted Successfully",
                    PerformedBy = "Admin",
                    Timestamp = DateTime.Now,
                    Details = $"Employee with ID {id} deleted successfully.",
      
                });

                return Ok("Employee deleted successfully!");
            }
            catch (Exception ex)
            {
                // Log employee not found error
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Employee Deletion Error",
                    PerformedBy = "Admin",
                    Timestamp = DateTime.Now,
                    Details = $"Error deleting employee with ID {id}: {ex.Message}"
                });
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

        [HttpPost("user/add")]
        public async Task<IActionResult> AddUser([FromBody] UserDTO userDto)
        {
            try
            {
                // Log the user creation attempt
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Add User Attempt",
                    PerformedBy = "Admin",  // Get the currently logged-in user
                    Timestamp = DateTime.Now,
                    Details = $"Attempting to create user with username {userDto.Username}"
                });

                // Create a new User entity from the UserDTO
                var user = new User
                {
                    Username = userDto.Username,
                    PasswordHash = userDto.PasswordHash, // Assuming password is already hashed
                    RoleID = userDto.RoleID,
                    CreatedDate = userDto.CreatedDate.Date, // Automatically set the created date
                    Email = userDto.Email,
                };

                // Add the user via the repository
                await _adminRepository.AddUserAsync(user);

                // Log successful user creation
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "User Created Successfully",
                    PerformedBy = "Admin",
                    Timestamp = DateTime.Now,
                    Details = $"User with username {userDto.Username} created successfully with UserID {user.UserID}"
                });
                var emailBody = $@"
                Hello {userDto.Username},
                
                Your account has been created successfully! Below are your login details:
                - User ID: {user.UserID}
                - Username: {userDto.Username}
                - Password: {userDto.PasswordHash}
                - Role ID: {userDto.RoleID}
                
                Please use these details to log in.

                Thank you,
                Admin Team
            ";

                // Step 3: Send email
                await _emailService.SendEmailAsync(userDto.Email, "Your Login Details", emailBody);

                return CreatedAtAction(nameof(AddUser), new { id = user.UserID }, user);
            }
            catch (Exception ex)
            {
                // Log error
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Add User Error",
                    PerformedBy = "Admin",
                    Timestamp = DateTime.Now,
                    Details = $"Error adding user with username {userDto.Username}: {ex.Message}"
                });
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPut("user/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDTO userDTO)
        {
            try
            {
                // Log the attempt to update the user
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Update User Attempt",
                    PerformedBy = "Admin",  // Get the currently logged-in user
                    Timestamp = DateTime.Now,
                    Details = $"Attempting to update user with UserID {id} and username {userDTO.Username}"
                 
                });

                var user = await _adminRepository.GetUserByIdAsync(id);
                if (user == null)
                {
                    // Log failed attempt due to user not found
                    await _adminRepository.LogAuditTrailAsync(new AuditTrail
                    {
                        Action = "Update User Failed",
                        PerformedBy = "Admin",
                        Timestamp = DateTime.Now,
                        Details = $"User with UserID {id} not found"
                    });
                    return NotFound("Employee not found");
                }

                user.UserID = userDTO.UserID;
                user.Username = userDTO.Username;
                user.Email = userDTO.Email;
                user.RoleID = userDTO.RoleID;


                await _adminRepository.UpdateUserAsync(user);

                // Log successful update
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "User Updated Successfully",
                    PerformedBy = "Admin",
                    Timestamp = DateTime.Now,
                    Details = $"User with UserID {user.UserID} and username {user.Username} updated successfully"                  
                });


                return Ok("User updated successfully!");
            }
            catch (Exception ex)
            {
                // Log any exception
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Update User Error",
                    PerformedBy = "Admin",
                    Timestamp = DateTime.Now,
                    Details = $"Error updating user with UserID {id}: {ex.Message}"
                });
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("payroll")]
        public async Task<IActionResult> GetAllPayrollsAsync()
        {
            try
            {
                var payrolls = await _adminRepository.GetAllPayrollsAsync();
                return Ok(payrolls);
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
            try
            {
                // Log the attempt to add payroll
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Add Payroll Attempt",
                    PerformedBy = "Admin", // Get the currently logged-in user
                    Timestamp = DateTime.Now,
                    Details = $"Attempting to add payroll for EmployeeID {payrollDto.EmployeeID}"
                    
                });
                // Validate the employee
                var employee = await _adminRepository.GetEmployeeByIdAsync(payrollDto.EmployeeID);
                if (employee == null)
                {
                    // Log failed attempt due to employee not found
                    await _adminRepository.LogAuditTrailAsync(new AuditTrail
                    {
                        Action = "Add Payroll Failed",
                        PerformedBy = "Admin",
                        Timestamp = DateTime.Now,
                        Details = $"Employee with EmployeeID {payrollDto.EmployeeID} not found."
                    });
                    return NotFound("Employee not found.");
                }

                // Automated calculations
                var basicSalary = payrollDto.BasicSalary;
                var hra = 0.20m * basicSalary; // 20% of Basic Salary
                var travellingAllowance = 0.10m * basicSalary; // 10% of Basic Salary
                var da = 0.15m * basicSalary; // 15% of Basic Salary
                var lta = 0.15m * basicSalary;
                var tds = 0.10m * basicSalary;

                // Gross Pay
                var grossPay = 0;

                // Deductions
                var pf = 0.12m * basicSalary; // 12% of Basic Salary
                var esi = 0.075m * grossPay; // ESI only for gross pay <= 21,000

                //var esi = grossPay <= 21000 ? 0.075m * grossPay : 0; // ESI only for gross pay <= 21,000
                var totalDeductions = pf + tds + esi;

                // Net Pay
                var netPay = 0;

                var payroll = new Payroll
                {
                    EmployeeID = payrollDto.EmployeeID,
                    //BasicSalary = payrollDto.BasicSalary,
                    //HRA = payrollDto.BasicSalary,
                    //BasicSalary = payrollDto.BasicSalary,
                    BasicSalary = basicSalary,
                    HRA = hra,
                    LTA = lta,
                    //TravellingAllowance = payrollDto.TravellingAllowance,
                    TravellingAllowance = travellingAllowance,
                    //DA = payrollDto.DA,
                    DA = da,
                    //GrossPay = payrollDto.GrossPay,
                    GrossPay = grossPay,
                    //PF = payrollDto.PF,
                    PF = pf,
                    //TDS = payrollDto.TDS,
                    TDS = tds,
                    //ESI = payrollDto.ESI,
                    ESI = esi,
                    //Deduction = payrollDto.Deduction,
                    Deduction = totalDeductions,
                    //TaxAmount = payrollDto.TaxAmount,

                    //NetPay = payrollDto.NetPay,
                    NetPay = netPay,
                    StartPeriod = payrollDto.StartPeriod,
                    EndPeriod = payrollDto.EndPeriod,
                    GeneratedDate = DateTime.UtcNow
                };

                await _adminRepository.AddPayrollAsync(payroll);

                // Log successful payroll addition
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Add Payroll Success",
                    PerformedBy = "Admin",
                    Timestamp = DateTime.Now,
                    Details = $"Payroll for EmployeeID {payrollDto.EmployeeID} added successfully"
                });

                return CreatedAtAction(nameof(AddPayroll), new { id = payroll.PayrollID }, payroll);
            }
            catch (Exception ex)
            {
                // Log any error that occurs
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Add Payroll Error",
                    PerformedBy = "Admin",
                    Timestamp = DateTime.Now,
                    Details = $"Error adding payroll for EmployeeID {payrollDto.EmployeeID}: {ex.Message}"
                });
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //[HttpPut("payroll/{id}")]
        //public async Task<IActionResult> UpdatePayrollByEmployeeId(int id, [FromBody] PayrollDTO payrollDto)
        //{
        //    try
        //    {
        //        var payroll = await _adminRepository.GetPayrollByEmployeeIdAsync(id);
        //        if (payroll == null) return NotFound("Payroll not found");

        //        payroll.BasicSalary = payrollDto.BasicSalary;
        //        payroll.HRA = payrollDto.HRA;
        //        payroll.LTA = payrollDto.LTA;
        //        payroll.TravellingAllowance = payrollDto.TravellingAllowance;
        //        payroll.DA = payrollDto.DA;
        //        payroll.GrossPay = payrollDto.GrossPay;
        //        payroll.PF = payrollDto.PF;
        //        payroll.TDS = payrollDto.TDS;
        //        payroll.ESI = payrollDto.ESI;
        //        payroll.Deduction = payrollDto.Deduction;
        //        payroll.TaxAmount = payrollDto.TaxAmount;
        //        payroll.NetPay = payrollDto.NetPay;
        //        payroll.StartPeriod = payrollDto.StartPeriod;
        //        payroll.EndPeriod = payrollDto.EndPeriod;

        //        await _adminRepository.UpdatePayrollAsync(payroll);
        //        return Ok("Payroll updated successfully!");

        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}

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
            // Log the start of the update process
            await _adminRepository.LogAuditTrailAsync(new AuditTrail
            {
                Action = "Update Compliance Report Attempt",
                PerformedBy = "Admin", // Get the currently logged-in user
                Timestamp = DateTime.Now,
                Details = $"Attempting to update compliance report for EmployeeID {employeeId}. New ComplianceStatus: {updateDTO.ComplianceStatus}"
            });

            if (!ModelState.IsValid)
            {
                // Log validation failure
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Update Compliance Report Validation Failed",
                    PerformedBy = "Admin",
                    Timestamp = DateTime.Now,
                    Details = $"Validation failed for EmployeeID {employeeId}. Invalid data provided."
                });
                return BadRequest(ModelState); // Return 400 if input is invalid
            }

            try
            {
                var isUpdated = await _adminRepository.UpdateComplianceReportAsync(employeeId, updateDTO);
                if (!isUpdated)
                {
                    // Log failure to find the report
                    await _adminRepository.LogAuditTrailAsync(new AuditTrail
                    {
                        Action = "Update Compliance Report Failed",
                        PerformedBy = "Admin",
                        Timestamp = DateTime.Now,
                        Details = $"Compliance report for EmployeeID {employeeId} not found."
                    });
                    return NotFound($"Compliance report for EmployeeID {employeeId} not found.");
                }

                // Log successful update
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Update Compliance Report Success",
                    PerformedBy = "Admin",
                    Timestamp = DateTime.Now,
                    Details = $"Compliance report for EmployeeID {employeeId} updated successfully. New ComplianceStatus: {updateDTO.ComplianceStatus}."
                });
                return Ok($"Compliance report for EmployeeID {employeeId} updated successfully.");
            }
            catch (Exception ex)
            {
                // Log any exception that occurs
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Update Compliance Report Error",
                    PerformedBy = "Admin",
                    Timestamp = DateTime.Now,
                    Details = $"Error updating compliance report for EmployeeID {employeeId}: {ex.Message}"
                });
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }







        //---------------------------------------------------------------//

        // View all audit logs
        [HttpGet("get-audit-trails")]
        public async Task<IActionResult> GetAllAuditTrails()
        {
            try
            {
                var auditTrails = await _adminRepository.GetAllAuditTrailsAsync();
                return Ok(auditTrails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Search audit logs
        [HttpGet("search-audit-trails")]
        public async Task<IActionResult> SearchAuditTrails([FromQuery] string searchTerm, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var auditTrails = await _adminRepository.SearchAuditTrailsAsync(searchTerm, startDate, endDate);

                if (auditTrails == null)
                {
                    return NotFound("Logs not found.");
                }
                return Ok(auditTrails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Log a new audit trail
        [HttpPost("log-audit-trails")]
        public async Task<IActionResult> LogAuditTrail([FromBody] AuditTrail auditTrail)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid data.");
                }

                await _adminRepository.LogAuditTrailAsync(auditTrail);
                return Ok("Audit trail logged successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

