using Microsoft.AspNetCore.Mvc;
using Moq;
using NewPayGenixAPI.Controllers;
using NewPayGenixAPI.Models;
using NewPayGenixAPI.Repositories;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaygenixProject.Tests
{

    [TestFixture]
    public class ManagerControllerTests
    {
        private Mock<IManagerRepository> _managerRepositoryMock;
        private Mock<IAdminRepository> _adminRepositoryMock;
        private ManagerController _managerController;

        [SetUp]
        public void SetUp()
        {
            _managerRepositoryMock = new Mock<IManagerRepository>();
            _adminRepositoryMock = new Mock<IAdminRepository>();
            _managerController = new ManagerController(_managerRepositoryMock.Object, _adminRepositoryMock.Object);
        }

        [Test]
        public async Task GetTeamPayrolls_ReturnsOkResult_WithPayrolls()
        {
            // Arrange
            var payrolls = new List<Payroll>
        {
            new Payroll { PayrollID = 1, EmployeeID = 1, NetPay = 50000, StartPeriod = DateTime.UtcNow.AddMonths(-1), EndPeriod = DateTime.UtcNow },
            new Payroll { PayrollID = 2, EmployeeID = 2, NetPay = 60000, StartPeriod = DateTime.UtcNow.AddMonths(-1), EndPeriod = DateTime.UtcNow }
        };
            _managerRepositoryMock.Setup(repo => repo.GetTeamPayrollsAsync()).ReturnsAsync(payrolls);

            // Act
            var result = await _managerController.GetTeamPayrolls();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo(payrolls));
        }

        [Test]
        public async Task GetLeaveRequestsByEmployeeId_ValidId_ReturnsOkResult()
        {
            // Arrange
            var leaveRequests = new List<LeaveRequest>
        {
            new LeaveRequest { LeaveRequestID = 1, EmployeeID = 1, Status = "Pending" },
            new LeaveRequest { LeaveRequestID = 2, EmployeeID = 1, Status = "Approved" }
        };
            _managerRepositoryMock.Setup(repo => repo.GetLeaveRequestsByEmployeeIdAsync(1)).ReturnsAsync(leaveRequests);

            // Act
            var result = await _managerController.GetLeaveRequestsByEmployeeId(1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo(leaveRequests));
        }

        [Test]
        public async Task GetLeaveRequestsByEmployeeId_NoLeaveRequests_ReturnsNotFound()
        {
            // Arrange
            _managerRepositoryMock.Setup(repo => repo.GetLeaveRequestsByEmployeeIdAsync(1)).ReturnsAsync(new List<LeaveRequest>());

            // Act
            var result = await _managerController.GetLeaveRequestsByEmployeeId(1);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("No leave requests found for this employee."));
        }

        [Test]
        public async Task UpdateLeaveRequestStatus_ValidStatus_ReturnsOkResult()
        {
            // Arrange
            var leaveRequestId = 1;
            var status = "Approved";
            _managerRepositoryMock.Setup(repo => repo.UpdateLeaveRequestStatusAsync(leaveRequestId, status)).Returns(Task.CompletedTask);

            // Act
            var result = await _managerController.UpdateLeaveRequestStatus(leaveRequestId, status);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo($"Leave request {leaveRequestId} status updated to {status}."));
        }

        [Test]
        public async Task UpdateLeaveRequestStatus_InvalidStatus_ReturnsBadRequest()
        {
            // Arrange
            var leaveRequestId = 1;
            var status = "InvalidStatus";

            // Act
            var result = await _managerController.UpdateLeaveRequestStatus(leaveRequestId, status);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("Invalid status. Valid values are: Approved, Rejected."));
        }
    }
}