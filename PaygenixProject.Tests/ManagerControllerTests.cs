using Microsoft.AspNetCore.Mvc;
using Moq;
using NewPayGenixAPI.Controllers;
using NewPayGenixAPI.DTO;
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
        private ManagerController _controller;

        [SetUp]
        public void Setup()
        {
            _managerRepositoryMock = new Mock<IManagerRepository>();
            _controller = new ManagerController(_managerRepositoryMock.Object);
        }

        #region UpdateLeaveRequestStatus Tests

        [Test]
        public async Task UpdateLeaveRequestStatus_ReturnsOk_WhenStatusIsValid()
        {
            // Arrange
            int leaveRequestId = 1;
            string status = "Approved";

            _managerRepositoryMock
                .Setup(repo => repo.UpdateLeaveRequestStatusAsync(leaveRequestId, status))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateLeaveRequestStatus(leaveRequestId, status);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual($"Leave request {leaveRequestId} status updated to {status}.", okResult.Value);
        }

        [Test]
        public async Task UpdateLeaveRequestStatus_ReturnsBadRequest_WhenStatusIsInvalid()
        {
            // Arrange
            int leaveRequestId = 1;
            string invalidStatus = "Pending";

            // Act
            var result = await _controller.UpdateLeaveRequestStatus(leaveRequestId, invalidStatus);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Invalid status. Valid values are: Approved, Rejected.", badRequestResult.Value);
        }

        [Test]
        public async Task UpdateLeaveRequestStatus_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            int leaveRequestId = 1;
            string status = "Approved";

            _managerRepositoryMock
                .Setup(repo => repo.UpdateLeaveRequestStatusAsync(leaveRequestId, status))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.UpdateLeaveRequestStatus(leaveRequestId, status);

            // Assert
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual($"Internal server error: Database error", objectResult.Value);
        }

        #endregion

        #region GetPayrollsByManager Tests

        [Test]
        public async Task GetPayrollsByManager_ReturnsOk_WhenPayrollsExist()
        {
            // Arrange
            int managerUserId = 1;
            var payrolls = new List<PayrollDTO>
            {
                new PayrollDTO { PayrollID = 1, EmployeeID = 1, NetPay = 5000 },
                new PayrollDTO { PayrollID = 2, EmployeeID = 2, NetPay = 6000 }
            };

            _managerRepositoryMock
                .Setup(repo => repo.GetPayrollsByManagerAsync(managerUserId))
                .ReturnsAsync(payrolls);

            // Act
            var result = await _controller.GetPayrollsByManager(managerUserId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(payrolls, okResult.Value);
        }

        [Test]
        public async Task GetPayrollsByManager_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            int managerUserId = 1;

            _managerRepositoryMock
                .Setup(repo => repo.GetPayrollsByManagerAsync(managerUserId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetPayrollsByManager(managerUserId);

            // Assert
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual($"Internal server error: Database error", objectResult.Value);
        }

        #endregion

        #region GetLeaveRequestsByManager Tests

        [Test]
        public async Task GetLeaveRequestsByManager_ReturnsOk_WhenLeaveRequestsExist()
        {
            // Arrange
            int managerUserId = 1;
            var leaveRequests = new List<LeaveRequestDTO>
            {
                new LeaveRequestDTO { LeaveRequestID = 1, EmployeeID = 1, Status = "Pending" },
                new LeaveRequestDTO { LeaveRequestID = 2, EmployeeID = 2, Status = "Approved" }
            };

            _managerRepositoryMock
                .Setup(repo => repo.GetLeaveRequestsByManagerAsync(managerUserId))
                .ReturnsAsync(leaveRequests);

            // Act
            var result = await _controller.GetLeaveRequestsByManager(managerUserId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(leaveRequests, okResult.Value);
        }

        [Test]
        public async Task GetLeaveRequestsByManager_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            int managerUserId = 1;

            _managerRepositoryMock
                .Setup(repo => repo.GetLeaveRequestsByManagerAsync(managerUserId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetLeaveRequestsByManager(managerUserId);

            // Assert
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual($"Internal server error: Database error", objectResult.Value);
        }

        #endregion
    }
}