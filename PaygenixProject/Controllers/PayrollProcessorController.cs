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

        

        // Verify Payroll
        [HttpGet("verify/{employeeId}")]
        public async Task<IActionResult> VerifyPayroll(int employeeId)
        {
            await _payrollProcessorRepository.VerifyPayrollAsync(employeeId);
            return Ok($"Payroll for Employee ID {employeeId} has been verified.");
        }

    }
}
