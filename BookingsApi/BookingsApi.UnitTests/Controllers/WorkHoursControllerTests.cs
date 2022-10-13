using BookingsApi.Contract.Requests;
using BookingsApi.Controllers;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Commands.Core;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingsApi.UnitTests.Controllers
{
    public class WorkHoursControllerTests
    {

        private WorkHoursController _controller;
        private Mock<ICommandHandler> _commandHandlerMock;

        private string _username;

        [SetUp]
        public void SetUp()
        {
            _username = "username@email.com";

            _commandHandlerMock = new Mock<ICommandHandler>();

            _controller = new WorkHoursController(_commandHandlerMock.Object);
        }

        [Test]
        public async Task SaveWorkHours_ReturnsErrors_WhenValidationFails()
        {
            // Arrange
            var uploadWorkAllocationRequests = new List<UploadWorkHoursRequest>
            {
                new UploadWorkHoursRequest
                {
                    Username = _username,
                    WorkingHours = new List<WorkingHours> {
                        new WorkingHours(1, null, 1, 1, 1)
                    }
                }
            };

            // Act
            var response = (await _controller.SaveWorkHours(uploadWorkAllocationRequests)) as BadRequestObjectResult;

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(response);
        }

        [Test]
        public async Task SaveWorkHours_CalllsUploadWorkAllocationCommand_AndReturnsResult()
        {
            // Arrange
            var uploadWorkAllocationRequests = new List<UploadWorkHoursRequest>
            {
                new UploadWorkHoursRequest
                {
                    Username = _username,
                    WorkingHours = new List<WorkingHours> {
                        new WorkingHours(1, 9, 0, 17, 0)
                    }
                }
            };

            // Act
            var response = (await _controller.SaveWorkHours(uploadWorkAllocationRequests)) as OkObjectResult;

            // Assert
            _commandHandlerMock.Verify(x => x.Handle(It.IsAny<UploadWorkHoursCommand>()), Times.Once);
            Assert.IsInstanceOf<OkObjectResult>(response);
            Assert.IsInstanceOf<List<string>>(response.Value);
        }

        [Test]
        public async Task SaveNonWorkingHours_ReturnsErrors_WhenValidationFails()
        {
            // Arrange
            var uploadNonWorkingHoursRequests = new List<UploadNonWorkingHoursRequest>
            {
                new UploadNonWorkingHoursRequest
                {
                    Username = _username,
                    NonWorkingHours = new List<NonWorkingHours> {
                        new NonWorkingHours(new DateTime(2022, 2, 1), new DateTime(2022, 1, 1))
                    }
                }
            };

            // Act
            var response = (await _controller.SaveNonWorkingHours(uploadNonWorkingHoursRequests)) as BadRequestObjectResult;

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(response);
        }

        [Test]
        public async Task SaveNonWorkingHours_CalllsUploadNonWorkingHoursCommand_AndReturnsResult()
        {
            // Arrange
            var uploadNonWorkingHoursRequests = new List<UploadNonWorkingHoursRequest>
            {
                new UploadNonWorkingHoursRequest
                {
                    Username = _username,
                    NonWorkingHours = new List<NonWorkingHours> {
                        new NonWorkingHours(new DateTime(2022, 1, 1), new DateTime(2022, 2, 1))
                    }
                }
            };

            // Act
            var response = (await _controller.SaveNonWorkingHours(uploadNonWorkingHoursRequests)) as OkObjectResult;

            // Assert
            _commandHandlerMock.Verify(x => x.Handle(It.IsAny<UploadNonWorkingHoursCommand>()), Times.Once);
            Assert.IsInstanceOf<OkObjectResult>(response);
            Assert.IsInstanceOf<List<string>>(response.Value);
        }
    }
}
