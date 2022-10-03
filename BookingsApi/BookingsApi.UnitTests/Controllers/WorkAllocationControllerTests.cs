using BookingsApi.Contract.Requests;
using BookingsApi.Controllers;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Commands.Core;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingsApi.UnitTests.Controllers
{
    public class WorkAllocationControllerTests
    {

        private WorkAllocationController _controller;
        private Mock<ICommandHandler> _commandHandlerMock;

        private string _username;

        [SetUp]
        public void SetUp()
        {
            _username = "username@email.com";

            _commandHandlerMock = new Mock<ICommandHandler>();

            _controller = new WorkAllocationController(_commandHandlerMock.Object);
        }

        [Test]
        public async Task SaveWorkAllocations_ReturnsErrors_WhenValidationFails()
        {
            // Arrange
            var uploadWorkAllocationRequests = new List<UploadWorkAllocationRequest>
            {
                new UploadWorkAllocationRequest
                {
                    Username = _username,
                    WorkingHours = new List<WorkingHours> {
                        new WorkingHours(1, null, 1, 1, 1)
                    }
                }
            };

            // Act
            var response = (await _controller.SaveWorkAllocations(uploadWorkAllocationRequests)) as BadRequestObjectResult;

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(response);
        }

        [Test]
        public async Task SaveWorkAllocations_CalllsUploadWorkAllocationCommand_AndReturnsResult()
        {
            // Arrange
            var uploadWorkAllocationRequests = new List<UploadWorkAllocationRequest>
            {
                new UploadWorkAllocationRequest
                {
                    Username = _username,
                    WorkingHours = new List<WorkingHours> {
                        new WorkingHours(1, 9, 0, 17, 0)
                    }
                }
            };

            // Act
            var response = (await _controller.SaveWorkAllocations(uploadWorkAllocationRequests)) as OkResult;

            // Assert
            _commandHandlerMock.Verify(x => x.Handle(It.IsAny<UploadWorkAllocationCommand>()), Times.Once);
            Assert.IsInstanceOf<OkResult>(response);
        }
    }
}
