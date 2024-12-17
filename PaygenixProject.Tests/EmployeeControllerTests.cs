using Microsoft.AspNetCore.Mvc;
using Moq;
using NewPayGenixAPI.Controllers;
using NewPayGenixAPI.DTO;
using NewPayGenixAPI.Models;
using NewPayGenixAPI.Repositories;
using NUnit.Framework;
using PaygenixProject.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaygenixProject.Tests
{
    [TestFixture]
    public class EmployeeControllerTests
    {
        private Mock<IEmployeeRepository> _employeeRepositoryMock;
        private Mock<IAdminRepository> _adminRepositoryMock;
        private EmployeeController _controller;

        [SetUp]
        public void Setup()
        {
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _adminRepositoryMock = new Mock<IAdminRepository>();

            _controller = new EmployeeController(_employeeRepositoryMock.Object, _adminRepositoryMock.Object);
        }

        #region GetEmployeeDetails Tests

        [Test]
        public async Task GetEmployeeDetails_ReturnsOk_WhenEmployeeExists()
        {
            // Arrange
            int employeeId = 1;
            var employee = new Employee
            {
                EmployeeID = employeeId,
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                PhoneNumber = "1234567890"
            };

            _employeeRepositoryMock.Setup(repo => repo.GetEmployeeDetailsAsync(employeeId))
                .ReturnsAsync(employee);

            // Act
            var result = await _controller.GetEmployeeDetails(employeeId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(employee, okResult.Value);
        }

        [Test]
        public async Task GetEmployeeDetails_ReturnsNotFound_WhenEmployeeDoesNotExist()
        {
            // Arrange
            int employeeId = 2;
            _employeeRepositoryMock.Setup(repo => repo.GetEmployeeDetailsAsync(employeeId))
                .ReturnsAsync((Employee)null);

            // Act
            var result = await _controller.GetEmployeeDetails(employeeId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("Employee not found", notFoundResult.Value);
        }

        [Test]
        public async Task GetEmployeeDetails_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            int employeeId = 3;
            _employeeRepositoryMock.Setup(repo => repo.GetEmployeeDetailsAsync(employeeId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetEmployeeDetails(employeeId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.AreEqual("Internal server error: Database error", statusCodeResult.Value);
        }

        #endregion

        #region AddEmployee Tests

        [Test]
        public async Task AddEmployee_ReturnsCreatedAtAction_WhenEmployeeIsAdded()
        {
            // Arrange
            var employeeDto = new EmployeeDTO
            {
                FirstName = "Jane",
                LastName = "Doe",
                Email = "janedoe@example.com",
                PhoneNumber = "9876543210",
                Position = "ASE",
                Department = "IT",
                HireDate = DateTime.UtcNow,
                ActiveStatus = "Active",
                UserID = 1,
                ManagerUserID = 3
            };

            var employee = new Employee
            {
                //EmployeeID = 1,
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                Email = employeeDto.Email,
                PhoneNumber = employeeDto.PhoneNumber,
                Position = employeeDto.Position,
                Department = employeeDto.Department,
                HireDate = employeeDto.HireDate,
                ActiveStatus = employeeDto.ActiveStatus,
                UserID = employeeDto.UserID,
                ManagerUserID = employeeDto.ManagerUserID
            };

            _employeeRepositoryMock.Setup(repo => repo.AddEmployeeAsync(It.IsAny<Employee>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddEmployee(employeeDto);

            // Assert
            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            Assert.AreEqual(201, createdAtActionResult.StatusCode);
            Assert.AreEqual(employee.EmployeeID, ((Employee)createdAtActionResult.Value).EmployeeID);
        }

        [Test]
        public async Task AddEmployee_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var employeeDto = new EmployeeDTO
            {
                FirstName = "Error",
                LastName = "Test",
                Email = "error@example.com",
                PhoneNumber = "1234567890",
                HireDate = DateTime.UtcNow,
                ActiveStatus = "Active",
                UserID = 1,
                ManagerUserID = 0
            };

            _employeeRepositoryMock.Setup(repo => repo.AddEmployeeAsync(It.IsAny<Employee>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.AddEmployee(employeeDto);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.AreEqual("Internal server error: Database error", statusCodeResult.Value);
        }

        #endregion

        #region UpdatePersonalInfo Tests

        [Test]
        public async Task UpdatePersonalInfo_ReturnsOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            int employeeId = 1;
            var employeeDto = new EmployeeDTO
            {
                FirstName = "Updated",
                LastName = "Employee",
                Email = "updated@example.com",
                PhoneNumber = "1231231234",
                UserID = 2
            };

            var existingEmployee = new Employee
            {
                EmployeeID = employeeId,
                FirstName = "Old",
                LastName = "Employee",
                Email = "old@example.com",
                PhoneNumber = "0000000000"
            };

            _employeeRepositoryMock.Setup(repo => repo.GetEmployeeDetailsAsync(employeeId))
                .ReturnsAsync(existingEmployee);
            _employeeRepositoryMock.Setup(repo => repo.UpdateEmployeePersonalInfoAsync(It.IsAny<Employee>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdatePersonalInfo(employeeId, employeeDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual("Employee details updated successfully", okResult.Value);
        }

        [Test]
        public async Task UpdatePersonalInfo_ReturnsNotFound_WhenEmployeeDoesNotExist()
        {
            // Arrange
            int employeeId = 2;
            var employeeDto = new EmployeeDTO();

            _employeeRepositoryMock.Setup(repo => repo.GetEmployeeDetailsAsync(employeeId))
                .ReturnsAsync((Employee)null);

            // Act
            var result = await _controller.UpdatePersonalInfo(employeeId, employeeDto);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("Employee not found", notFoundResult.Value);
        }

        [Test]
        public async Task UpdatePersonalInfo_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            int employeeId = 3;
            var employeeDto = new EmployeeDTO();

            _employeeRepositoryMock.Setup(repo => repo.GetEmployeeDetailsAsync(employeeId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.UpdatePersonalInfo(employeeId, employeeDto);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.AreEqual("Internal server error: Database error", statusCodeResult.Value);
        }

        #endregion

        #region GetPayStubs Tests

        [Test]
        public async Task GetPayStubs_ReturnsOk_WhenPayStubsExist()
        {
            // Arrange
            int employeeId = 1;
            var payStubs = new List<Payroll>
            {
                new Payroll { PayrollID = 1, EmployeeID = employeeId, StartPeriod = DateTime.Now, NetPay = 5000 },
                new Payroll { PayrollID = 2, EmployeeID = employeeId, StartPeriod = DateTime.Now.AddMonths(-1), NetPay = 5200 }
            };

            _employeeRepositoryMock.Setup(repo => repo.GetPayStubsAsync(employeeId))
                .ReturnsAsync(payStubs);

            // Act
            var result = await _controller.GetPayStubs(employeeId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(payStubs, okResult.Value);
        }

        [Test]
        public async Task GetPayStubs_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            int employeeId = 1;
            _employeeRepositoryMock.Setup(repo => repo.GetPayStubsAsync(employeeId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetPayStubs(employeeId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.AreEqual("Internal server error: Database error", statusCodeResult.Value);
        }

        #endregion

        #region RequestLeave Tests

        [Test]
        public async Task RequestLeave_ReturnsOk_WhenLeaveRequestIsSuccessful()
        {
            // Arrange
            var leaveRequestDto = new LeaveRequestDTO
            {
                EmployeeID = 1,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(5),
                LeaveType = "Sick Leave"
            };

            _employeeRepositoryMock.Setup(repo => repo.RequestLeaveAsync(It.IsAny<LeaveRequest>()))
                .Returns(Task.CompletedTask);

            _adminRepositoryMock.Setup(repo => repo.LogAuditTrailAsync(It.IsAny<AuditTrail>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RequestLeave(leaveRequestDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual("Leave Requested", okResult.Value);
        }

        [Test]
        public async Task RequestLeave_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var leaveRequestDto = new LeaveRequestDTO
            {
                EmployeeID = 1,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(5),
                LeaveType = "Sick Leave"
            };

            _employeeRepositoryMock.Setup(repo => repo.RequestLeaveAsync(It.IsAny<LeaveRequest>()))
                .ThrowsAsync(new Exception("Database error"));

            _adminRepositoryMock.Setup(repo => repo.LogAuditTrailAsync(It.IsAny<AuditTrail>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RequestLeave(leaveRequestDto);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.AreEqual("Internal server error: Database error", statusCodeResult.Value);
        }

        #endregion

        #region GenerateComplianceReport Tests

        [Test]
        public async Task GenerateComplianceReport_ReturnsCreatedAtAction_WhenReportIsGenerated()
        {
            // Arrange
            var reportDto = new ComplianceReportDTO
            {
                EmployeeID = 1,
                ReportDate = DateTime.UtcNow,
                StartPeriod = DateTime.Now.AddMonths(-1),
                EndPeriod = DateTime.Now,
                IssuesFound = "Late attendance",
                ComplianceStatus = "Pending",
                ResolvedStatus = "Pending",
                Comments = "Pending resolution"
            };

            var generatedReport = new ComplianceReport
            {
                //ReportID = 1,
                ReportDate = DateTime.UtcNow,
                EmployeeID = reportDto.EmployeeID,
                StartPeriod = reportDto.StartPeriod,
                EndPeriod = reportDto.EndPeriod,
                ComplianceStatus = "Pending",
                ResolvedStatus = "Pending",
                IssuesFound = "Late attendance",
                Comments = reportDto.Comments
            };

            _employeeRepositoryMock.Setup(repo => repo.GenerateComplianceReportAsync(It.IsAny<ComplianceReport>()))
                .Returns(Task.CompletedTask);

            _adminRepositoryMock.Setup(repo => repo.LogAuditTrailAsync(It.IsAny<AuditTrail>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.GenerateComplianceReport(reportDto);

            // Assert
            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            Assert.AreEqual(201, createdAtActionResult.StatusCode);
            Assert.AreEqual(generatedReport.ReportID, ((ComplianceReport)createdAtActionResult.Value).ReportID);
        }

        [Test]
        public async Task GenerateComplianceReport_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var reportDto = new ComplianceReportDTO
            {
                EmployeeID = 1,
                StartPeriod = DateTime.Now.AddMonths(-1),
                EndPeriod = DateTime.Now,
                IssuesFound = "Late attendance",
                Comments = "Pending resolution"
            };

            _employeeRepositoryMock.Setup(repo => repo.GenerateComplianceReportAsync(It.IsAny<ComplianceReport>()))
                .ThrowsAsync(new Exception("Database error"));

            _adminRepositoryMock.Setup(repo => repo.LogAuditTrailAsync(It.IsAny<AuditTrail>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.GenerateComplianceReport(reportDto);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.AreEqual("Internal server error: Database error", statusCodeResult.Value);
        }

        #endregion

        #region GetEmployeeDetailsByUserID Tests

        [Test]
        public async Task GetEmployeeDetailsByUserID_ReturnsOk_WhenEmployeeExists()
        {
            // Arrange
            int userId = 1;
            var employee = new Employee { EmployeeID = 1, FirstName = "John", LastName = "Doe" };

            _employeeRepositoryMock.Setup(repo => repo.GetEmployeeDetailsByUserIDAsync(userId))
                .ReturnsAsync(employee);

            // Act
            var result = await _controller.GetEmployeeDetailsByUserID(userId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(employee, okResult.Value);
        }

        [Test]
        public async Task GetEmployeeDetailsByUserID_ReturnsNotFound_WhenEmployeeDoesNotExist()
        {
            // Arrange
            int userId = 1;
            _employeeRepositoryMock.Setup(repo => repo.GetEmployeeDetailsByUserIDAsync(userId))
                .ReturnsAsync((Employee)null);

            // Act
            var result = await _controller.GetEmployeeDetailsByUserID(userId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("Employee not found", notFoundResult.Value);
        }

        [Test]
        public async Task GetEmployeeDetailsByUserID_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            int userId = 1;
            _employeeRepositoryMock.Setup(repo => repo.GetEmployeeDetailsByUserIDAsync(userId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetEmployeeDetailsByUserID(userId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.AreEqual("Internal server error: Database error", statusCodeResult.Value);
        }

        #endregion

        #region GetPayStubsByUserID Tests

        [Test]
        public async Task GetPayStubsByUserID_ReturnsOk_WhenPayStubsExist()
        {
            // Arrange
            int userId = 1;
            var payStubs = new List<Payroll>
            {
                new Payroll { PayrollID = 1, EmployeeID = userId, NetPay = 5000 },
                new Payroll { PayrollID = 2, EmployeeID = userId, NetPay = 5200 }
            };

            _employeeRepositoryMock.Setup(repo => repo.GetPayStubsByUserIDAsync(userId))
                .ReturnsAsync(payStubs);

            // Act
            var result = await _controller.GetPayStubsByUserID(userId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(payStubs, okResult.Value);
        }

        [Test]
        public async Task GetPayStubsByUserID_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            int userId = 1;
            _employeeRepositoryMock.Setup(repo => repo.GetPayStubsByUserIDAsync(userId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetPayStubsByUserID(userId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.AreEqual("Internal server error: Database error", statusCodeResult.Value);
        }

        #endregion

        #region GetLeaveRequestsByEmployeeId Tests

        [Test]
        public async Task GetLeaveRequestsByEmployeeId_ReturnsOk_WhenLeaveRequestsExist()
        {
            // Arrange
            int employeeId = 1;
            var leaveRequests = new List<LeaveRequest>
            {
                new LeaveRequest { LeaveRequestID = 1, EmployeeID = employeeId, Status = "Pending" },
                new LeaveRequest { LeaveRequestID = 2, EmployeeID = employeeId, Status = "Approved" }
            };

            _employeeRepositoryMock.Setup(repo => repo.GetLeaveRequestsByEmployeeIdAsync(employeeId))
                .ReturnsAsync(leaveRequests);

            // Act
            var result = await _controller.GetLeaveRequestsByEmployeeId(employeeId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(leaveRequests, okResult.Value);
        }

        [Test]
        public async Task GetLeaveRequestsByEmployeeId_ReturnsNotFound_WhenNoLeaveRequestsExist()
        {
            // Arrange
            int employeeId = 1;
            _employeeRepositoryMock.Setup(repo => repo.GetLeaveRequestsByEmployeeIdAsync(employeeId))
                .ReturnsAsync(new List<LeaveRequest>());

            // Act
            var result = await _controller.GetLeaveRequestsByEmployeeId(employeeId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"No leave requests found for EmployeeID {employeeId}.", notFoundResult.Value);
        }

        #endregion

        #region GetLeaveRequestsByUserId Tests

        [Test]
        public async Task GetLeaveRequestsByUserId_ReturnsOk_WhenLeaveRequestsExist()
        {
            // Arrange
            int userId = 1;
            var leaveRequests = new List<LeaveRequest>
            {
                new LeaveRequest { LeaveRequestID = 1, EmployeeID = userId, Status = "Pending" }
            };

            _employeeRepositoryMock.Setup(repo => repo.GetLeaveRequestsByUserIdAsync(userId))
                .ReturnsAsync(leaveRequests);

            // Act
            var result = await _controller.GetLeaveRequestsByUserId(userId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(leaveRequests, okResult.Value);
        }

        [Test]
        public async Task GetLeaveRequestsByUserId_ReturnsNotFound_WhenNoLeaveRequestsExist()
        {
            // Arrange
            int userId = 1;
            _employeeRepositoryMock.Setup(repo => repo.GetLeaveRequestsByUserIdAsync(userId))
                .ReturnsAsync(new List<LeaveRequest>());

            // Act
            var result = await _controller.GetLeaveRequestsByUserId(userId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("No leave requests found.", notFoundResult.Value);
        }

        #endregion
    }
}
