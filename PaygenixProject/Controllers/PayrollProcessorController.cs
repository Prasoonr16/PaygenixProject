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
        private readonly IEmailService _emailService;

        public PayrollProcessorController(IPayrollProcessorRepository payrollProcessorRepository, IAdminRepository adminRepository, IEmailService emailService)
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
        
        [HttpGet("payrolls-by-period")]
        public async Task<IActionResult> FetchPayrollsByPeriod([FromQuery] DateTime startPeriod, [FromQuery] DateTime endPeriod)
        {
            try
            {
                // Validate the date range
                if (startPeriod >= endPeriod)
                    return BadRequest("Invalid date range. StartPeriod must be earlier than EndPeriod.");

                // Fetch payrolls
                var payrolls = await _payrollProcessorRepository.FetchPayrollsByPeriodAsync(startPeriod, endPeriod);

                if (!payrolls.Any())
                    return NotFound($"No payrolls found for the period {startPeriod:yyyy-MM-dd} to {endPeriod:yyyy-MM-dd}.");

                return Ok(payrolls);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching payrolls: {ex.Message}");
            }
        }
        [HttpPost("process/{payrollId}")]
        public async Task<IActionResult> ProcessPayroll(int payrollId)
        {
            try
            {
                // Process the payroll
                var processedPayroll = await _payrollProcessorRepository.ProcessPayrollByIdAsync(payrollId);

                if (processedPayroll == null)
                    return NotFound($"Payroll with ID {payrollId} not found.");

                return Ok(new
                {
                    Message = "Payroll processed successfully.",
                    Payroll = processedPayroll
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error processing payroll: {ex.Message}");
            }
        }


        [HttpPost("verify/{payrollId}")]
        public async Task<IActionResult> VerifyPayroll(int payrollId)
        {
            // Log the attempt to verify payroll
            await _adminRepository.LogAuditTrailAsync(new AuditTrail
            {
                Action = "Verify Payroll Attempt",
                PerformedBy = "PayrollProcessor", 
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
                        PerformedBy = "PayrollProcessor", 
                        Timestamp = DateTime.Now,
                        Details = $"Payroll with PayrollID {payrollId} was verified successfully."
                    });

                    // Fetch employee details associated with this payroll
                    var (employee, netPay) = await _payrollProcessorRepository.GetEmployeeByPayrollIdAsync(payrollId);
                    if (employee == null)
                    {
                        return BadRequest("Employee associated with payroll not found.");
                    }

                    // Prepare the email content
                    var emailBody = $@"
                Dear {employee.FirstName} {employee.LastName},

                Your payroll has been successfully processed.

                Payroll ID: {payrollId}
                Net Pay: {netPay:C}

                Thank you,
                Payroll Team
            ";

                    // Send the email
                    await _emailService.SendEmailAsync(employee.Email, "Payroll Processed", emailBody);

                    return Ok("Payroll verified and email sent successfully.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
            // Log failed payroll verification attempt
            await _adminRepository.LogAuditTrailAsync(new AuditTrail
            {
                Action = "Verify Payroll Failed",
                PerformedBy = "PayrollProcessor", 
                Timestamp = DateTime.Now,
                Details = $"Failed to verify payroll for PayrollID {payrollId}. Verification failed."
            });

            return BadRequest("Payroll verification failed");
        }
    }
}
           