using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewPayGenixAPI.DTO;
using NewPayGenixAPI.Models;
using NewPayGenixAPI.Repositories;
using PaygenixProject.Models;
using PaygenixProject.Repositories;

namespace NewPayGenixAPI.Controllers
{
    [ApiController]
    [Route("api/payrollprocessor")]
    //[Authorize(Roles = "PayrollProcessor")] // Only users with the "PayrollProcessor" role can access these endpoints
    public class PayrollProcessorController : ControllerBase
    {
        private readonly IPayrollProcessorRepository _payrollProcessorRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly EmailService _emailService;

        public PayrollProcessorController(IPayrollProcessorRepository payrollProcessorRepository, IAdminRepository adminRepository,EmailService emailService)
        {
            _payrollProcessorRepository = payrollProcessorRepository;
            _adminRepository = adminRepository;
            _emailService = emailService;
        }

        [HttpGet("pay-roll/{employeeID}")]

        public async Task<IActionResult> GetPayrollByEmployeeID(int employeeID)
        {
            try
            {
                var payrolls = await _payrollProcessorRepository.GetPayrollByEmployeeIdAsync(employeeID);
                if (payrolls == null) return NotFound("Payroll not found");
                return Ok(payrolls);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("process/{employeeId}")]
        public async Task<IActionResult> ProcessPayroll(int employeeId, [FromBody] PayrollDTO payrollDto)
        {
            // Log the attempt to process payroll
            await _adminRepository.LogAuditTrailAsync(new AuditTrail
            {
                Action = "Process Payroll Attempt",
                PerformedBy = "PayrollProcessor", // Get the currently logged-in user
                Timestamp = DateTime.Now,
                Details = $"Attempting to process payroll for EmployeeID {employeeId}."
            });

            if (!ModelState.IsValid)
            {
                // Log validation failure
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Process Payroll Validation Failed",
                    PerformedBy = "PayrollProcessor",
                    Timestamp = DateTime.Now,
                    Details = $"Payroll processing for EmployeeID {employeeId} failed validation. Invalid data."
                });

                return BadRequest(ModelState);
            }

            try
            {
                var payroll = await _payrollProcessorRepository.ProcessPayrollAsync(employeeId, payrollDto);

                // Log successful payroll processing
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Process Payroll Success",
                    PerformedBy = "PayrollProcessor", // Get the currently logged-in user
                    Timestamp = DateTime.Now,
                    Details = $"Payroll processed successfully for EmployeeID {employeeId}. PayrollID: {payroll.PayrollID}. Gross Pay: {payroll.GrossPay}, Net Pay: {payroll.NetPay}."
                });

                //return CreatedAtAction(nameof(ProcessPayroll), new { id = payroll.PayrollID }, payroll);

                return Ok("Payroll processed successfully.");

            }
            catch (Exception ex)
            {
                // Log the exception/error if the processing fails
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Process Payroll Error",
                    PerformedBy = "PayrollProcessor", // Get the currently logged-in user
                    Timestamp = DateTime.Now,
                    Details = $"Error processing payroll for EmployeeID {employeeId}. Error: {ex.Message}"
                });

                return BadRequest(ex.Message);
            }
        }
        //[HttpPost("verify/{payrollId}")]
        //public async Task<IActionResult> VerifyPayroll(int payrollId)
        //{
        //    try
        //    {
        //        var isVerified = await _payrollProcessorRepository.VerifyPayrollAsync(payrollId);
        //        if (isVerified)
        //        {
        //            return Ok("Payroll verified successfully");
        //        }
        //        EmployeeDTO.Email
        //        return BadRequest("Payroll verification failed");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
        [HttpPost("verify/{payrollId}")]
        public async Task<IActionResult> VerifyPayroll(int payrollId)
        {
            // Log the attempt to verify payroll
            await _adminRepository.LogAuditTrailAsync(new AuditTrail
            {
                Action = "Verify Payroll Attempt",
                PerformedBy = "PayrollProcessor", // Get the currently logged-in user
                Timestamp = DateTime.Now,
                Details = $"Attempting to verify payroll for PayrollID {payrollId}."
            });

            try
            {
                // Verify the payroll
                var isVerified = await _payrollProcessorRepository.VerifyPayrollAsync(payrollId);
                if (isVerified)
                {
                    // Log successful payroll verification
                    await _adminRepository.LogAuditTrailAsync(new AuditTrail
                    {
                        Action = "Verify Payroll Success",
                        PerformedBy = "PayrollProcessor", // Get the currently logged-in user
                        Timestamp = DateTime.Now,
                        Details = $"Payroll with PayrollID {payrollId} was verified successfully."
                    });

                    // Fetch employee details associated with this payroll
                    var employee = await _payrollProcessorRepository.GetEmployeeByPayrollIdAsync(payrollId);
                    if (employee == null)
                    {
                        return BadRequest("Employee associated with payroll not found.");
                    }

                    // Prepare the email content
                    var emailBody = $@"
                Dear {employee.FirstName} {employee.LastName},

                Your payroll has been successfully processed.

                Payroll ID: {payrollId}
                Net Pay: {employee.NetPay:C}

                Thank you,
                Payroll Team
            ";

                    // Send the email
                    await _emailService.SendEmailAsync(employee.Email, "Payroll Processed", emailBody);

                    return Ok("Payroll verified and email sent successfully.")
                }

                // Log failed payroll verification attempt
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Verify Payroll Failed",
                    PerformedBy = "PayrollProcessor", // Get the currently logged-in user
                    Timestamp = DateTime.Now,
                    Details = $"Failed to verify payroll for PayrollID {payrollId}. Verification failed."
                });

                return BadRequest("Payroll verification failed");
            }
            catch (Exception ex)
            {
                // Log error if verification fails due to an exception
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Verify Payroll Error",
                    PerformedBy = "PayrollProcessor", // Get the currently logged-in user
                    Timestamp = DateTime.Now,
                    Details = $"Error while verifying payroll for PayrollID {payrollId}. Error: {ex.Message}"
                });

                return BadRequest(ex.Message);
                    ;
                }

                return BadRequest("Payroll verification failed.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
}
}
