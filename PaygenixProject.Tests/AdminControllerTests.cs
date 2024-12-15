using Microsoft.AspNetCore.Mvc;
using Moq;
using NewPayGenixAPI.Controllers;
using NewPayGenixAPI.DTO;
using NewPayGenixAPI.Models;
using NewPayGenixAPI.Repositories;
using NUnit.Framework;
using PaygenixProject.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaygenixProject.Tests
{
    [TestFixture]
    public class AdminControllerTests
    {
        private Mock<IAdminRepository> _adminRepositoryMock;
        private Mock<EmailService> _emailServiceMock;
        private AdminController _adminController;

        [SetUp]
        public void SetUp()
        {
            _adminRepositoryMock = new Mock<IAdminRepository>();
            _emailServiceMock = new Mock<EmailService>();
            _adminController = new AdminController(_adminRepositoryMock.Object, _emailServiceMock.Object);
        }

        [Test]
        public async Task GetAllEmployees_ReturnsOkResult_WithEmployees()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { EmployeeID = 1, FirstName = "John", LastName = "Doe" },
                new Employee { EmployeeID = 2, FirstName = "Jane", LastName = "Smith" }
            };
            _adminRepositoryMock.Setup(repo => repo.GetAllEmployeesAsync()).ReturnsAsync(employees);

            // Act
            var result = await _adminController.GetAllEmployees();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(employees, okResult.Value);
        }

        //[Test]
        //public async Task GetEmployeeById_EmployeeExists_ReturnsOkResult()
        //{
        //    // Arrange
        //    var employee = new Employee { EmployeeID = 1, FirstName = "John", LastName = "Doe" };
        //    _adminRepositoryMock.Setup(repo => repo.GetEmployeeByIdAsync(1)).ReturnsAsync(employee);

        //    // Act
        //    var result = await _adminController.GetEmployeeById(1);

        //    // Assert
        //    Assert.IsInstanceOf<OkObjectResult>(result);
        //    var okResult = result as OkObjectResult;
        //    Assert.AreEqual(employee, okResult.Value);
        //}

        //[Test]
        //public async Task GetEmployeeById_EmployeeDoesNotExist_ReturnsNotFound()
        //{
        //    // Arrange
        //    _adminRepositoryMock.Setup(repo => repo.GetEmployeeByIdAsync(1)).ReturnsAsync((Employee)null);

        //    // Act
        //    var result = await _adminController.GetEmployeeById(1);

        //    // Assert
        //    Assert.IsInstanceOf<NotFoundObjectResult>(result);
        //}

        [Test]
        public async Task UpdateEmployee_ValidEmployee_ReturnsOkResult()
        {
            // Arrange
            var employee = new Employee { EmployeeID = 1, FirstName = "John", LastName = "Doe" };
            var employeeDto = new EmployeeDTO
            {
                FirstName = "UpdatedName",
                LastName = "Doe",
                Email = "updated@example.com",
                PhoneNumber = "1234567890",
                Position = "Manager",
                Department = "HR"
            };
            _adminRepositoryMock.Setup(repo => repo.GetEmployeeByIdAsync(1)).ReturnsAsync(employee);
            _adminRepositoryMock.Setup(repo => repo.UpdateEmployeeAsync(employee)).Returns(Task.CompletedTask);

            // Act
            var result = await _adminController.UpdateEmployee(1, employeeDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);

            _adminRepositoryMock.Verify(repo => repo.UpdateEmployeeAsync(It.Is<Employee>(e => e.FirstName == "UpdatedName")), Times.Once);
        }

        [Test]
        public async Task DeleteEmployee_ValidId_ReturnsOkResult()
        {
            // Arrange
            _adminRepositoryMock.Setup(repo => repo.DeleteEmployeeAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _adminController.DeleteEmployee(1);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);

            _adminRepositoryMock.Verify(repo => repo.DeleteEmployeeAsync(1), Times.Once);
        }

        [Test]
        public async Task AddPayroll_ValidPayroll_ReturnsCreatedResult()
        {
            // Arrange
            var payrollDto = new PayrollDTO
            {
                EmployeeID = 1,
                BasicSalary = 50000,
                GrossPay = 60000,
                NetPay = 45000,
                StartPeriod = DateTime.Now,
                EndPeriod = DateTime.Now.AddMonths(1),
                GeneratedDate = DateTime.Now
            };
            var payroll = new Payroll
            {
                PayrollID = 1,
                EmployeeID = 1,
                BasicSalary = 50000,
                GrossPay = 60000,
                NetPay = 45000
            };
            _adminRepositoryMock.Setup(repo => repo.AddPayrollAsync(It.IsAny<Payroll>())).Returns(Task.CompletedTask);

            // Act
            var result = await _adminController.AddPayroll(payrollDto);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result);
        }
    }
}
