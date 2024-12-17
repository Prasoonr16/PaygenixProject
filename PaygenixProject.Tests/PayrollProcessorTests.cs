using Microsoft.AspNetCore.Mvc;
using Moq;
using NewPayGenixAPI.Controllers;
using NewPayGenixAPI.DTO;
using NewPayGenixAPI.Models;
using NewPayGenixAPI.Repositories;
using NUnit.Framework;
using PaygenixProject.Repositories;
using System;
using System.Threading.Tasks;

namespace PaygenixProject.Tests
{
    [TestFixture]
    public class PayrollProcessorControllerTests
    {
<<<<<<< HEAD
        
=======
        private Mock<IPayrollProcessorRepository> _payrollProcessorRepositoryMock;
        private Mock<IAdminRepository> _adminRepositoryMock;
        private Mock<IEmailService> _emailServiceMock;
        private PayrollProcessorController _controller;

        [SetUp]
        public void SetUp()
        {
            _payrollProcessorRepositoryMock = new Mock<IPayrollProcessorRepository>();
            _adminRepositoryMock = new Mock<IAdminRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _controller = new PayrollProcessorController(_payrollProcessorRepositoryMock.Object, _adminRepositoryMock.Object, _emailServiceMock.Object);
        }

        #region GetPayrollByEmployeeID

        [Test]
        public async Task GetPayrollByEmployeeID_ReturnsOk_WhenPayrollExists()
        {
            // Arrange
            var employeeId = 1;
            var payroll = new List<Payroll> { new Payroll { EmployeeID = 1, NetPay = 5000 } };

            _payrollProcessorRepositoryMock
                .Setup(repo => repo.GetPayrollByEmployeeIdAsync(employeeId))
                .ReturnsAsync(payroll);

            // Act
            var result = await _controller.GetPayrollByEmployeeID(employeeId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(payroll, okResult.Value);
        }

        [Test]
        public async Task GetPayrollByEmployeeID_ReturnsNotFound_WhenPayrollDoesNotExist()
        {
            // Arrange
            var employeeId = 1;

            _payrollProcessorRepositoryMock
                .Setup(repo => repo.GetPayrollByEmployeeIdAsync(employeeId))
                .ReturnsAsync((List<Payroll>)null);

            // Act
            var result = await _controller.GetPayrollByEmployeeID(employeeId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("Payroll not found", notFoundResult.Value);
        }

        #endregion

        #region FetchPayrollsByPeriod

        [Test]
        public async Task FetchPayrollsByPeriod_ReturnsBadRequest_WhenStartPeriodIsLaterThanEndPeriod()
        {
            // Arrange
            var startPeriod = DateTime.UtcNow;
            var endPeriod = startPeriod.AddDays(-1);

            // Act
            var result = await _controller.FetchPayrollsByPeriod(startPeriod, endPeriod);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Invalid date range. StartPeriod must be earlier than EndPeriod.", badRequestResult.Value);
        }

        [Test]
        public async Task FetchPayrollsByPeriod_ReturnsOk_WhenPayrollsExist()
        {
            // Arrange
            var startPeriod = DateTime.UtcNow.AddMonths(-1);
            var endPeriod = DateTime.UtcNow;
            var payrolls = new List<PayrollDTO> { new PayrollDTO { EmployeeID = 1, NetPay = 5000 } };

            _payrollProcessorRepositoryMock
                .Setup(repo => repo.FetchPayrollsByPeriodAsync(startPeriod, endPeriod))
                .ReturnsAsync(payrolls);

            // Act
            var result = await _controller.FetchPayrollsByPeriod(startPeriod, endPeriod);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(payrolls, okResult.Value);
        }

        [Test]
        public async Task FetchPayrollsByPeriod_ReturnsNotFound_WhenNoPayrollsExist()
        {
            // Arrange
            var startPeriod = DateTime.UtcNow.AddMonths(-1);
            var endPeriod = DateTime.UtcNow;
            var payrolls = new List<PayrollDTO>();

            _payrollProcessorRepositoryMock
                .Setup(repo => repo.FetchPayrollsByPeriodAsync(startPeriod, endPeriod))
                .ReturnsAsync(payrolls);

            // Act
            var result = await _controller.FetchPayrollsByPeriod(startPeriod, endPeriod);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"No payrolls found for the period {startPeriod:yyyy-MM-dd} to {endPeriod:yyyy-MM-dd}.", notFoundResult.Value);
        }

        #endregion

        #region ProcessPayroll

        [Test]
        public async Task ProcessPayroll_ReturnsOk_WhenPayrollIsProcessedSuccessfully()
        {
            // Arrange
            var payrollId = 1;
            var payroll = new PayrollDTO { PayrollID = payrollId, NetPay = 1000 };

            _payrollProcessorRepositoryMock
                .Setup(repo => repo.ProcessPayrollByIdAsync(payrollId))
                .ReturnsAsync(payroll);

            // Act
            var result = await _controller.ProcessPayroll(payrollId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            //Assert.AreEqual("Payroll processed successfully.", ((dynamic)okResult.Value).Message);
        }

        [Test]
        public async Task ProcessPayroll_ReturnsNotFound_WhenPayrollDoesNotExist()
        {
            // Arrange
            var payrollId = 1;

            _payrollProcessorRepositoryMock
                .Setup(repo => repo.ProcessPayrollByIdAsync(payrollId))
                .ReturnsAsync((PayrollDTO)null);

            // Act
            var result = await _controller.ProcessPayroll(payrollId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"Payroll with ID {payrollId} not found.", notFoundResult.Value);
        }

        #endregion

        #region VerifyPayroll

        [Test]
        public async Task VerifyPayroll_ReturnsOk_WhenPayrollIsVerifiedSuccessfully()
        {
            // Arrange
            var payrollId = 1;
            var employee = new EmployeeDTO { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };
            var netPay = 1000m;

            _payrollProcessorRepositoryMock
                .Setup(repo => repo.VerifyPayrollAsync(payrollId))
                .ReturnsAsync(true);

            _payrollProcessorRepositoryMock
                .Setup(repo => repo.GetEmployeeByPayrollIdAsync(payrollId))
                .ReturnsAsync((employee, netPay));

            _emailServiceMock
                .Setup(service => service.SendEmailAsync(employee.Email, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.VerifyPayroll(payrollId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual("Payroll verified and email sent successfully.", okResult.Value);
        }

        [Test]
        public async Task VerifyPayroll_ReturnsBadRequest_WhenPayrollVerificationFails()
        {
            // Arrange
            var payrollId = 1;

            _payrollProcessorRepositoryMock
                .Setup(repo => repo.VerifyPayrollAsync(payrollId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.VerifyPayroll(payrollId);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Payroll verification failed", badRequestResult.Value);
        }

        #endregion
>>>>>>> 16cb98c03f946f84f99482e04cdf28055c5970d9
    }
}