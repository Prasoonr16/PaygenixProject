using Microsoft.AspNetCore.Mvc;
using Moq;
using NewPayGenixAPI.Controllers;
using NewPayGenixAPI.DTO;
using NewPayGenixAPI.Models;
using NewPayGenixAPI.Repositories;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace PaygenixProject.Tests
{
    [TestFixture]
    public class PayrollProcessorControllerTests
    {
        private Mock<IPayrollProcessorRepository> _payrollProcessorRepositoryMock;
        private PayrollProcessorController _payrollProcessorController;

        [SetUp]
        public void SetUp()
        {
            _payrollProcessorRepositoryMock = new Mock<IPayrollProcessorRepository>();
            _payrollProcessorController = new PayrollProcessorController(_payrollProcessorRepositoryMock.Object);
        }

        [Test]
        public async Task ProcessPayroll_ValidData_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var employeeId = 1;
            var payrollDto = new PayrollDTO
            {
                BasicSalary = 50000,
                HRA = 10000,
                LTA = 5000,
                TravellingAllowance = 2000,
                DA = 3000,
                GrossPay = 70000,
                PF = 5000,
                TDS = 2000,
                ESI = 1000,
                Deduction = 8000,
                TaxAmount = 2000,
                NetPay = 62000,
                StartPeriod = DateTime.UtcNow.AddMonths(-1),
                EndPeriod = DateTime.UtcNow
            };

            var payroll = new Payroll
            {
                PayrollID = 101,
                EmployeeID = employeeId,
                BasicSalary = payrollDto.BasicSalary,
                NetPay = payrollDto.NetPay,
                StartPeriod = payrollDto.StartPeriod,
                EndPeriod = payrollDto.EndPeriod
            };

            _payrollProcessorRepositoryMock
                .Setup(repo => repo.ProcessPayrollAsync(employeeId, payrollDto))
                .ReturnsAsync(payroll);

            // Act
            var result = await _payrollProcessorController.ProcessPayroll(employeeId, payrollDto);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
            var createdResult = result as CreatedAtActionResult;
            Assert.That(createdResult?.Value, Is.EqualTo(payroll));
        }

        [Test]
        public async Task ProcessPayroll_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            _payrollProcessorController.ModelState.AddModelError("BasicSalary", "Required");

            // Act
            var result = await _payrollProcessorController.ProcessPayroll(1, new PayrollDTO());

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task VerifyPayroll_ValidPayrollId_ReturnsOkResult()
        {
            // Arrange
            var payrollId = 101;
            _payrollProcessorRepositoryMock.Setup(repo => repo.VerifyPayrollAsync(payrollId)).ReturnsAsync(true);

            // Act
            var result = await _payrollProcessorController.VerifyPayroll(payrollId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo("Payroll verified successfully"));
        }

        [Test]
        public async Task VerifyPayroll_InvalidPayrollId_ReturnsBadRequest()
        {
            // Arrange
            var payrollId = 102;
            _payrollProcessorRepositoryMock.Setup(repo => repo.VerifyPayrollAsync(payrollId)).ReturnsAsync(false);

            // Act
            var result = await _payrollProcessorController.VerifyPayroll(payrollId);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("Payroll verification failed"));
        }

        [Test]
        public async Task ProcessPayroll_ExceptionThrown_ReturnsBadRequest()
        {
            // Arrange
            var employeeId = 1;
            var payrollDto = new PayrollDTO();
            _payrollProcessorRepositoryMock
                .Setup(repo => repo.ProcessPayrollAsync(employeeId, payrollDto))
                .ThrowsAsync(new Exception("Processing error"));

            // Act
            var result = await _payrollProcessorController.ProcessPayroll(employeeId, payrollDto);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("Processing error"));
        }

        [Test]
        public async Task VerifyPayroll_ExceptionThrown_ReturnsBadRequest()
        {
            // Arrange
            var payrollId = 101;
            _payrollProcessorRepositoryMock
                .Setup(repo => repo.VerifyPayrollAsync(payrollId))
                .ThrowsAsync(new Exception("Verification error"));

            // Act
            var result = await _payrollProcessorController.VerifyPayroll(payrollId);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("Verification error"));
        }
    }
}
