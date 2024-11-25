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

        // Add Payroll
        [HttpPost("add")]
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

            await _payrollProcessorRepository.AddPayrollAsync(payroll);
            return CreatedAtAction(nameof(AddPayroll), new { id = payroll.PayrollID }, payroll);
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
