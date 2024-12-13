using Microsoft.AspNetCore.Mvc;
using Moq;
using NewPayGenixAPI.Controllers;
using NewPayGenixAPI.DTO;
using NewPayGenixAPI.Models;
using NewPayGenixAPI.Repositories;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaygenixProject.Tests
{
    [TestFixture]
    public class EmployeeControllerTests
    {
        private Mock<IEmployeeRepository> _employeeRepositoryMock;
        private EmployeeController _employeeController;

        [SetUp]
        public void SetUp()
        {
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _employeeController = new EmployeeController(_employeeRepositoryMock.Object);
        }

        [Test]
        public async Task GetEmployeeDetails_ValidId_ReturnsOkResultWithEmployee()
        {
            // Arrange
            var employee = new Employee { EmployeeID = 1, FirstName = "John", LastName = "Doe" };
            _employeeRepositoryMock.Setup(repo => repo.GetEmployeeDetailsAsync(1)).ReturnsAsync(employee);

            // Act
            var result = await _employeeController.GetEmployeeDetails(1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo(employee));
        }

        [Test]
        public async Task GetEmployeeDetails_InvalidId_ReturnsNotFound()
        {
            // Arrange
            _employeeRepositoryMock.Setup(repo => repo.GetEmployeeDetailsAsync(99)).ReturnsAsync((Employee)null);

            // Act
            var result = await _employeeController.GetEmployeeDetails(99);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task UpdatePersonalInfo_ValidId_ReturnsOkResult()
        {
            // Arrange
            var employeeDto = new EmployeeDTO
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                PhoneNumber = "1234567890",
                UserID = 2
            };

            _employeeRepositoryMock.Setup(repo => repo.UpdateEmployeePersonalInfoAsync(It.IsAny<Employee>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _employeeController.UpdatePersonalInfo(1, employeeDto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo("Employee details updated successfully"));
        }

        
        [Test]
        public async Task GetPayStubs_ValidId_ReturnsOkResultWithPayrolls()
        {
            // Arrange
            var payrolls = new List<Payroll>
        {
            new Payroll
            {
                PayrollID = 1,
                EmployeeID = 1,
                BasicSalary = 50000,
                HRA = 10000,
                LTA = 5000,
                TravellingAllowance = 3000,
                DA = 7000,
                GrossPay = 75000,
                PF = 2000,
                TDS = 3000,
                ESI = 1500,
                Deduction = 6500,
                NetPay = 68500,
                StartPeriod = new DateTime(2024, 1, 1),
                EndPeriod = new DateTime(2024, 1, 31),
                GeneratedDate = DateTime.UtcNow
            },
            new Payroll
            {
                PayrollID = 2,
                EmployeeID = 1,
                BasicSalary = 52000,
                HRA = 11000,
                LTA = 6000,
                TravellingAllowance = 3200,
                DA = 7500,
                GrossPay = 78700,
                PF = 2100,
                TDS = 3100,
                ESI = 1600,
                Deduction = 6800,
                NetPay = 71900,
                StartPeriod = new DateTime(2024, 2, 1),
                EndPeriod = new DateTime(2024, 2, 28),
                GeneratedDate = DateTime.UtcNow
            }
        };

            _employeeRepositoryMock.Setup(repo => repo.GetPayStubsAsync(1)).ReturnsAsync(payrolls);

            // Act
            var result = await _employeeController.GetPayStubs(1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo(payrolls));
        }


        [Test]
        public async Task RequestLeave_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var leaveRequestDto = new LeaveRequestDTO
            {
                EmployeeID = 1,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(5),
                LeaveType = "Sick Leave"
            };

            _employeeRepositoryMock.Setup(repo => repo.RequestLeaveAsync(It.IsAny<LeaveRequest>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _employeeController.RequestLeave(leaveRequestDto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo("Leave Requested"));
        }
    }
}
