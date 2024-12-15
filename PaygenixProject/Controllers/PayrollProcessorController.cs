using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewPayGenixAPI.DTO;
using NewPayGenixAPI.Models;
using NewPayGenixAPI.Repositories;
using PaygenixProject.Repositories;

namespace NewPayGenixAPI.Controllers
{
    [ApiController]
    [Route("api/payrollprocessor")]
    //[Authorize(Roles = "PayrollProcessor")] // Only users with the "PayrollProcessor" role can access these endpoints
    public class PayrollProcessorController : ControllerBase
    {
        private readonly IPayrollProcessorRepository _payrollProcessorRepository;
        private readonly EmailService _emailService;

        public PayrollProcessorController(IPayrollProcessorRepository payrollProcessorRepository, EmailService emailService)
        {
            _payrollProcessorRepository = payrollProcessorRepository;
            _emailService = emailService;
        }

        [HttpPost("process/{employeeId}")]
        public async Task<IActionResult> ProcessPayroll(int employeeId, [FromBody] PayrollDTO payrollDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var payroll = await _payrollProcessorRepository.ProcessPayrollAsync(employeeId, payrollDto);
                return CreatedAtAction(nameof(ProcessPayroll), new { id = payroll.PayrollID }, payroll);
            }
            catch (Exception ex)
            {
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
            try
            {
                // Verify the payroll
                var isVerified = await _payrollProcessorRepository.VerifyPayrollAsync(payrollId);
                if (isVerified)
                {
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

                    return Ok("Payroll verified and email sent successfully.");
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
