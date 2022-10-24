using System;
using BookingsApi.Contract.Requests;
using BookingsApi.Controllers;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Commands.Core;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Responses;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using BookingsApi.Validations;
using FluentAssertions;
using Testing.Common.Assertions;

namespace BookingsApi.UnitTests.Controllers
{
    public class WorkHoursControllerTests
    {

        private WorkHoursController _controller;
        private Mock<ICommandHandler> _commandHandlerMock;
        private Mock<IQueryHandler> _queryHandlerMock;

        private string _username;

        [SetUp]
        public void SetUp()
        {
            _username = "username@email.com";

            _commandHandlerMock = new Mock<ICommandHandler>();
            _queryHandlerMock = new Mock<IQueryHandler>();
            
            _controller = new WorkHoursController(_commandHandlerMock.Object, _queryHandlerMock.Object);
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
        public async Task GetVhoWorkHours_InvokesGetVhoWorkAllocation_AndReturnsResult()
        {
            // Arrange
            var userName = "test.user@hearings.reform.hmcts.net";
            var queryResponse = new List<VhoWorkHours>{Mock.Of<VhoWorkHours>()};
            _queryHandlerMock
                .Setup(x => x.Handle<GetVhoWorkHoursQuery, List<VhoWorkHours>>(It.IsAny<GetVhoWorkHoursQuery>()))
                .ReturnsAsync(queryResponse);
            // Act
            var response = await _controller.GetVhoWorkAvailabilityHours(userName) as OkObjectResult;
            // Assert
            _queryHandlerMock.Verify(x => x.Handle<GetVhoWorkHoursQuery, List<VhoWorkHours>>(It.IsAny<GetVhoWorkHoursQuery>()), Times.Once);
            Assert.IsInstanceOf<OkObjectResult>(response);
            Assert.IsInstanceOf<List<VhoWorkHoursResponse>>(response.Value);
        }
        
                
        [Test]
        public async Task GetVhoWorkHours_InvokesGetVhoWorkAllocation_AndReturnsBadRequest()
        {
            // Arrange
            var userName = "test.user@@hearings.reform.hmcts.net";
            // Act
            var response = await _controller.GetVhoWorkAvailabilityHours(userName);
            // Assert
            _queryHandlerMock.Verify(x => x.Handle<GetVhoWorkHoursQuery, List<VhoWorkHours>>(It.IsAny<GetVhoWorkHoursQuery>()), Times.Never);
            Assert.IsInstanceOf<BadRequestObjectResult>(response);
        }
        
                        
        [Test]
        public async Task GetVhoWorkHours_InvokesGetVhoWorkAllocation_AndReturnsNotFound()
        {
            // Arrange
            var userName = "test.user@hearings.reform.hmcts.net";
            // Act
            var response = await _controller.GetVhoWorkAvailabilityHours(userName) as NotFoundObjectResult;
            // Assert
            _queryHandlerMock.Verify(x => x.Handle<GetVhoWorkHoursQuery, List<VhoWorkHours>>(It.IsAny<GetVhoWorkHoursQuery>()), Times.Once);
            Assert.IsInstanceOf<NotFoundObjectResult>(response);
            response?.Value.Should().Be("Vho user not found");
        }
 
        [Test]
        public async Task GetVhoNonAvailableWorkHours_InvokesGetVhoWorkAllocation_AndReturnsResult()
        {
            // Arrange
            var userName = "test.user@hearings.reform.hmcts.net";
            var queryResponse = new List<VhoNonAvailability>{Mock.Of<VhoNonAvailability>()};
            _queryHandlerMock
                .Setup(x => x.Handle<GetVhoNonAvailableWorkHoursQuery, List<VhoNonAvailability>>(It.IsAny<GetVhoNonAvailableWorkHoursQuery>()))
                .ReturnsAsync(queryResponse);
            // Act
            var response = await _controller.GetVhoNonAvailabilityHours(userName) as OkObjectResult;
            // Assert
            _queryHandlerMock.Verify(x => x.Handle<GetVhoNonAvailableWorkHoursQuery, List<VhoNonAvailability>>(It.IsAny<GetVhoNonAvailableWorkHoursQuery>()), Times.Once);
            Assert.IsInstanceOf<OkObjectResult>(response);
            Assert.IsInstanceOf<List<VhoNonAvailabilityWorkHoursResponse>>(response.Value);
        }
        
                
        [Test]
        public async Task GetVhoNonAvailableWorkHours_InvokesGetVhoWorkAllocation_AndReturnsBadRequest()
        {
            // Arrange
            var userName = "test.user@@hearings.reform.hmcts.net";
            // Act
            var response = await _controller.GetVhoNonAvailabilityHours(userName);
            // Assert
            _queryHandlerMock.Verify(x => x.Handle<GetVhoNonAvailableWorkHoursQuery, List<VhoNonAvailability>>(It.IsAny<GetVhoNonAvailableWorkHoursQuery>()), Times.Never);
            Assert.IsInstanceOf<BadRequestObjectResult>(response);
        }
        
                        
        [Test]
        public async Task GetVhoNonAvailableWorkHours_InvokesGetVhoWorkAllocation_AndReturnsNotFound()
        {
            // Arrange
            var userName = "test.user@hearings.reform.hmcts.net";
            // Act
            var response = await _controller.GetVhoNonAvailabilityHours(userName) as NotFoundObjectResult;
            // Assert
            _queryHandlerMock.Verify(x =>  x.Handle<GetVhoNonAvailableWorkHoursQuery, List<VhoNonAvailability>>(It.IsAny<GetVhoNonAvailableWorkHoursQuery>()), Times.Once);
            Assert.IsInstanceOf<NotFoundObjectResult>(response);
            response?.Value.Should().Be("Vho user not found");
        }

        [Test]
        public async Task SaveNonWorkingHours_ReturnsErrors_WhenValidationFails()
        {
            // Arrange
            var uploadNonWorkingHoursRequests = new List<UploadNonWorkingHoursRequest>
            {
                new UploadNonWorkingHoursRequest(_username, new DateTime(2022, 2, 1), new DateTime(2022, 1, 1))
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
                new UploadNonWorkingHoursRequest(_username, new DateTime(2022, 1, 1), new DateTime(2022, 2, 1))
            };

            // Act
            var response = (await _controller.SaveNonWorkingHours(uploadNonWorkingHoursRequests)) as OkObjectResult;

            // Assert
            _commandHandlerMock.Verify(x => x.Handle(It.IsAny<UploadNonWorkingHoursCommand>()), Times.Once);
            Assert.IsInstanceOf<OkObjectResult>(response);
            Assert.IsInstanceOf<List<string>>(response.Value);
        }

        [Test]
        public async Task UpdateVhoNonAvailabilityHours_Returns_NoContent()
        {
            // Arrange
            var request = new UpdateNonWorkingHoursRequest
            {
                Hours = new List<NonWorkingHours>
                {
                    new() { Id = 1, StartTime = new DateTime(2022, 1, 1), EndTime = new DateTime(2022, 1, 2) }
                }
            };

            // Act
            var response = await _controller.UpdateVhoNonAvailabilityHours(request);

            // Assert
            var objectResult = (NoContentResult)response;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }

        [Test]
        public async Task UpdateVhoNonAvailabilityHours_With_EndTime_Before_StartTime_Returns_BadRequest()
        {
            // Arrange
            var request = new UpdateNonWorkingHoursRequest
            {
                Hours = new List<NonWorkingHours>
                {
                    new() { Id = 1, StartTime = new DateTime(2022, 1, 1, 6, 0, 0, DateTimeKind.Utc), EndTime = new DateTime(2022, 1, 1, 10, 0, 0, DateTimeKind.Utc) },
                    new() { Id = 2, StartTime = new DateTime(2022, 1, 2, 8, 0, 0, DateTimeKind.Utc), EndTime = new DateTime(2022, 1, 2, 6, 0, 0, DateTimeKind.Utc) },
                    new() { Id = 3, StartTime = new DateTime(2022, 1, 2, 8, 0, 0, DateTimeKind.Utc), EndTime = new DateTime(2022, 1, 1, 6, 0, 0, DateTimeKind.Utc) }
                }
            };
            
            // Act
            var response = await _controller.UpdateVhoNonAvailabilityHours(request);

            // Assert
            var objectResult = (BadRequestObjectResult)response;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage("Hours[1].EndTime", "EndTime must be after StartTime");
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage("Hours[2].EndTime", "EndTime must be after StartTime");
        }
        
        [Test]
        public async Task UpdateVhoNonAvailabilityHours_With_EndTime_EqualTo_StartTime_Returns_BadRequest()
        {
            // Arrange
            var request = new UpdateNonWorkingHoursRequest
            {
                Hours = new List<NonWorkingHours>
                {
                    new() { Id = 1, StartTime = new DateTime(2022, 1, 1, 8, 0, 0, DateTimeKind.Utc), EndTime = new DateTime(2022, 1, 1, 8, 0, 0, DateTimeKind.Utc) },
                    new() { Id = 2, StartTime = new DateTime(2022, 1, 2, 6, 0, 0, DateTimeKind.Utc), EndTime = new DateTime(2022, 1, 2, 6, 0, 0, DateTimeKind.Utc) },
                    new() { Id = 3, StartTime = new DateTime(2022, 1, 3, 6, 0, 0, DateTimeKind.Utc), EndTime = new DateTime(2022, 1, 3, 10, 0, 0, DateTimeKind.Utc) }
                }
            };
            
            // Act
            var response = await _controller.UpdateVhoNonAvailabilityHours(request);

            // Assert
            var objectResult = (BadRequestObjectResult)response;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage("Hours[0].EndTime", "EndTime must be after StartTime");
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage("Hours[1].EndTime", "EndTime must be after StartTime");
        }

        [Test]
        public async Task UpdateVhoNonAvailabilityHours_With_Overlapping_Times_For_Single_User_Returns_BadRequest()
        {
            // TODO - may require mocks
            
            // Arrange
            // Act
            // Assert
        }

        [Test]
        public async Task UpdateVhoNonAvailabilityHours_With_NonExistent_Hours_Returns_BadRequest()
        {
            // TODO - eg hour ids that don't exist in the db
            // May require mocks
        }
    }
}
