using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Configuration;
using Moq;
using NewPayGenixAPI.Controllers;
using NewPayGenixAPI.Data;
using NewPayGenixAPI.DTO;
using NewPayGenixAPI.Models;
using NewPayGenixAPI.Repositories;
using NUnit.Framework;
using PaygenixProject.Models;
using PaygenixProject.Repositories;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace PaygenixProject.Tests
{
    [TestFixture]
    public class AdminControllerTests
    {
<<<<<<< HEAD
        
=======
        private Mock<IAdminRepository> _adminRepositoryMock;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<PaygenixDBContext> _contextMock;
        private AdminController _adminController;

        [SetUp]
        public void SetUp()
        {
            _adminRepositoryMock = new Mock<IAdminRepository>();
            _emailServiceMock = new Mock<IEmailService>();

            var options = new DbContextOptionsBuilder<PaygenixDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            _contextMock = new Mock<PaygenixDBContext>(options);

            _adminController = new AdminController(_adminRepositoryMock.Object, _emailServiceMock.Object, _contextMock.Object);

            // Create AdminController with mocked dependencies  
            //_adminController = new AdminController(_adminRepositoryMock.Object, _emailServiceMock.Object, _contextMock.Object);
        }

        // Test case for GetAllEmployees
        [Test]
        public async Task GetAllEmployees_ShouldReturnOkResult_WhenEmployeesExist()
        {
            // Arrange
            var mockEmployees = new List<Employee>
            {
                new Employee { EmployeeID = 1, FirstName = "John", LastName = "Doe" },
                new Employee { EmployeeID = 2, FirstName = "Jane", LastName = "Doe" }
            };
            _adminRepositoryMock.Setup(repo => repo.GetAllEmployeesAsync()).ReturnsAsync(mockEmployees);

            // Act
            var result = await _adminController.GetAllEmployees();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(mockEmployees, okResult.Value);
        }

        // Test case for UpdateEmployee
        [Test]
        public async Task UpdateEmployee_ShouldReturnOkResult_WhenEmployeeExists()
        {
            // Arrange
            var employeeDto = new EmployeeDTO
            {
                Position = "Manager",
                Department = "HR",
                HireDate = DateTime.Now,
                ActiveStatus = "Active",
                ManagerUserID = 1
            };
            var existingEmployee = new Employee { EmployeeID = 1, FirstName = "John", LastName = "Doe" };

            _adminRepositoryMock.Setup(repo => repo.GetEmployeeByIdAsync(1)).ReturnsAsync(existingEmployee);
            _adminRepositoryMock.Setup(repo => repo.UpdateEmployeeAsync(It.IsAny<Employee>())).Returns(Task.CompletedTask);

            // Act
            var result = await _adminController.UpdateEmployee(1, employeeDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual("Employee updated successfully!", okResult.Value);
        }

        [Test]
        public async Task UpdateEmployee_ShouldReturnNotFound_WhenEmployeeNotFound()
        {
            // Arrange
            var employeeDto = new EmployeeDTO { Position = "Manager", Department = "HR", HireDate = DateTime.Now };
            _adminRepositoryMock.Setup(repo => repo.GetEmployeeByIdAsync(1)).ReturnsAsync((Employee)null);

            // Act
            var result = await _adminController.UpdateEmployee(1, employeeDto);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("Employee not found", notFoundResult.Value);
        }

        // Test case for DeleteEmployee
        [Test]
        public async Task DeleteEmployee_ShouldReturnOkResult_WhenEmployeeIsDeleted()
        {
            // Arrange
            _adminRepositoryMock.Setup(repo => repo.DeleteEmployeeAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _adminController.DeleteEmployee(1);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual("Employee deleted successfully!", okResult.Value);
        }

       

        // Test case for GetAllUser
        [Test]
        public async Task GetAllUser_ShouldReturnOkResult_WhenUsersExist()
        {
            // Arrange
            var mockUsers = new[] {
                new User { UserID = 1, Username = "john_doe", Email = "john@example.com" },
                new User { UserID = 2, Username = "jane_doe", Email = "jane@example.com" }
            };

            _adminRepositoryMock.Setup(repo => repo.GetAllUserAsync()).ReturnsAsync(mockUsers);

            // Act
            var result = await _adminController.GetAllUser();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(mockUsers, okResult.Value);
        }

      

        // Test case for AddUser
        [Test]
        public async Task AddUser_ShouldReturnCreatedResult_WhenUserIsAddedSuccessfully()
        {
            // Arrange
            var userDto = new UserDTO
            {
                Username = "john_doe",
                PasswordHash = "hashedPassword123",
                RoleID = 1,
                CreatedDate = DateTime.Now,
                Email = "john@example.com"
            };

            var createdUser = new User { UserID = 1, Username = "john_doe", Email = "john@example.com" };

            _adminRepositoryMock.Setup(repo => repo.AddUserAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            // Act
            var result = await _adminController.AddUser(userDto);

            // Assert
            var createdAtActionResult = result as CreatedAtActionResult;
            //Assert.IsNotNull(createdAtActionResult);
            Assert.AreEqual(201, createdAtActionResult.StatusCode);
            //Assert.AreEqual(createdUser, createdAtActionResult.Value);
        }


        // Test case for UpdateUser
        [Test]
        public async Task UpdateUser_ShouldReturnOkResult_WhenUserIsUpdatedSuccessfully()
        {
            // Arrange
            var userDto = new UserDTO
            {
                UserID = 1,
                Username = "john_updated",
                Email = "john_updated@example.com",
                RoleID = 2
            };

            var existingUser = new User { UserID = 1, Username = "john_doe", Email = "john@example.com" };

            _adminRepositoryMock.Setup(repo => repo.GetUserByIdAsync(1)).ReturnsAsync(existingUser);
            _adminRepositoryMock.Setup(repo => repo.UpdateUserAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            // Act
            var result = await _adminController.UpdateUser(1, userDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual("User updated successfully!", okResult.Value);
        }

        [Test]
        public async Task UpdateUser_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userDto = new UserDTO { Username = "john_doe", Email = "john@example.com", RoleID = 1 };
            _adminRepositoryMock.Setup(repo => repo.GetUserByIdAsync(1)).ReturnsAsync((User)null);

            // Act
            var result = await _adminController.UpdateUser(1, userDto);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("Employee not found", notFoundResult.Value);
        }

       

        [Test]
        public async Task AddPayroll_ShouldReturnCreatedAtActionResult_WhenPayrollIsAddedSuccessfully()
        {
            
            var basicSalary = 10000;
            var pf = 0.12m * basicSalary;
            var tds = 0.10m * basicSalary;
            var esi = 0;
            // Arrange
            var payrollDto = new PayrollDTO
            {
                EmployeeID = 1,
                BasicSalary = basicSalary,
                HRA = 0.20m * basicSalary,
                TravellingAllowance = 0.10m * basicSalary,
                LTA = 0.15m * basicSalary,
                DA = 0.15m * basicSalary,
                TDS = tds,
                GrossPay = 0,
                PF = pf,
                ESI = esi,
                Deduction = pf + tds + esi,
                NetPay = 0,

                StartPeriod = DateTime.Now,
                EndPeriod = DateTime.Now.AddMonths(1),
                GeneratedDate = DateTime.Now,
            };

            var newPayroll = new Payroll
            {
                EmployeeID = 1,
                BasicSalary = payrollDto.BasicSalary,
                HRA = payrollDto.HRA,
                TravellingAllowance = payrollDto.HRA,
                LTA = payrollDto.LTA,
                DA = payrollDto.DA,
                TDS = payrollDto.TDS,
                GrossPay = 0,
                PF = payrollDto.PF,
                ESI = payrollDto.ESI,
                Deduction = payrollDto.Deduction,
                NetPay = 0,

                StartPeriod = DateTime.Now,
                EndPeriod = DateTime.Now.AddMonths(1),
                GeneratedDate = DateTime.Now,
            };

            _adminRepositoryMock.Setup(repo => repo.GetEmployeeByIdAsync(payrollDto.EmployeeID)).ReturnsAsync(new Employee { EmployeeID = 1 });
            _adminRepositoryMock.Setup(repo => repo.AddPayrollAsync(It.IsAny<Payroll>())).Returns(Task.CompletedTask);  // Return a completed task

            // Act
            var result = await _adminController.AddPayroll(payrollDto);

            // Assert
            var createdAtActionResult = result as CreatedAtActionResult;
            //Assert.IsNotNull(createdAtActionResult);
            Assert.AreEqual(201, createdAtActionResult.StatusCode);
            //Assert.AreEqual(newPayroll, createdAtActionResult.Value);
        }

        [Test]
        public async Task AddPayroll_ShouldReturnNotFound_WhenEmployeeIsNotFound()
        {
            var basicSalary = 10000;
            var pf = 0.12m * basicSalary;
            var tds = 0.10m * basicSalary;
            var esi = 0;
            // Arrange
            var payrollDto = new PayrollDTO
            {
                EmployeeID = 1,
                BasicSalary = basicSalary,
                HRA = 0.20m * basicSalary,
                TravellingAllowance = 0.10m * basicSalary,
                LTA = 0.15m * basicSalary,
                DA = 0.15m * basicSalary,
                TDS = tds,
                GrossPay = 0,
                PF = pf,
                ESI = esi,
                Deduction = pf+tds+esi,
                NetPay = 0,

                StartPeriod = DateTime.Now,
                EndPeriod = DateTime.Now.AddMonths(1)
            };


            _adminRepositoryMock.Setup(repo => repo.GetEmployeeByIdAsync(payrollDto.EmployeeID)).ReturnsAsync((Employee)null);

            // Act
            var result = await _adminController.AddPayroll(payrollDto);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("Employee not found.", notFoundResult.Value);
        }

      

        [Test]
        public async Task GetAllComplianceReports_ReturnsOkResult_WithComplianceReports()
        {
            // Arrange
            var reports = new List<ComplianceReport>
           {
                new ComplianceReport { ReportID = 1, ComplianceStatus = "Pending" },
                new ComplianceReport { ReportID = 2, ComplianceStatus = "Resolved" }
            };
            _adminRepositoryMock.Setup(repo => repo.GetAllComplianceReportAsync())
                .ReturnsAsync(reports);

            // Act
            var result = await _adminController.GetAllComplianceReports();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(reports, okResult.Value);
        }

        [Test]
        public async Task UpdateComplianceReport_ReturnsOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            int reportId = 1;
            var reportDTO = new ComplianceReportDTO { ComplianceStatus = "Resolved" };
            _adminRepositoryMock.Setup(repo => repo.UpdateComplianceReportAsync(reportId, reportDTO))
                .ReturnsAsync(true);

            // Act
            var result = await _adminController.UpdateComplianceReport(reportId, reportDTO);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task UpdateComplianceReport_ReturnsNotFound_WhenReportDoesNotExist()
        {
            // Arrange
            int reportId = 1;
            var reportDTO = new ComplianceReportDTO { ComplianceStatus = "Resolved" };
            _adminRepositoryMock.Setup(repo => repo.UpdateComplianceReportAsync(reportId, reportDTO))
                .ReturnsAsync(false);

            // Act
            var result = await _adminController.UpdateComplianceReport(reportId, reportDTO);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task GetAllAuditTrails_ReturnsOkResult_WithAuditTrails()
        {
            // Arrange
            var auditTrails = new List<AuditTrail>
            {
                new AuditTrail { AuditID = 1, Action = "Login", PerformedBy = "Admin" },
                new AuditTrail { AuditID = 2, Action = "Update", PerformedBy = "User1" }
            };
            _adminRepositoryMock.Setup(repo => repo.GetAllAuditTrailsAsync())
                .ReturnsAsync(auditTrails);

            // Act
            var result = await _adminController.GetAllAuditTrails();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(auditTrails, okResult.Value);
        }


        [Test]
        public async Task LogAuditTrail_ReturnsOk_WhenAuditTrailIsLogged()
        {
            // Arrange
            var auditTrail = new AuditTrail
            {
                Action = "Add User",
                PerformedBy = "Admin",
                Timestamp = DateTime.UtcNow,
                Details = "User added successfully"
            };

            _adminRepositoryMock.Setup(repo => repo.LogAuditTrailAsync(auditTrail))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _adminController.LogAuditTrail(auditTrail);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual("Audit trail logged successfully.", okResult.Value);
        }

       
>>>>>>> 16cb98c03f946f84f99482e04cdf28055c5970d9
    }

}

