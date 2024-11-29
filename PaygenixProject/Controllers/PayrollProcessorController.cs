using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewPayGenixAPI.DTO;
using NewPayGenixAPI.Models;
using NewPayGenixAPI.Repositories;

namespace NewPayGenixAPI.Controllers
{
    [ApiController]
    [Route("api/payrollprocessor")]
    [Authorize(Roles = "PayrollProcessor")] // Only users with the "PayrollProcessor" role can access these endpoints
    public class PayrollProcessorController : ControllerBase
    {
        private readonly IPayrollProcessorRepository _payrollProcessorRepository;

        public PayrollProcessorController(IPayrollProcessorRepository payrollProcessorRepository)
        {
            _payrollProcessorRepository = payrollProcessorRepository;
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
        [HttpPost("verify/{payrollId}")]
        public async Task<IActionResult> VerifyPayroll(int payrollId)
        {
            try
            {
                var isVerified = await _payrollProcessorRepository.VerifyPayrollAsync(payrollId);
                if (isVerified)
                {
                    return Ok("Payroll verified successfully");
                }
                return BadRequest("Payroll verification failed");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
