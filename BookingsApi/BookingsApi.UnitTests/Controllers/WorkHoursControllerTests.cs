using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Commands.Core;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Controllers.V1;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using BookingsApi.Validations.V1;
using Testing.Common.Assertions;

namespace BookingsApi.UnitTests.Controllers
{
    public class WorkHoursControllerTests
    {

        private WorkHoursController _controller;
        private Mock<ICommandHandler> _commandHandlerMock;
        private Mock<IQueryHandler> _queryHandlerMock;
        private JusticeUser _justiceUser;

        private string _username;

        [SetUp]
        public void SetUp()
        {
            _username = "username@email.com";

            _commandHandlerMock = new Mock<ICommandHandler>();
            _queryHandlerMock = new Mock<IQueryHandler>();
            
            _justiceUser = new JusticeUser
            {
                FirstName = "FirstName",
                Lastname = "Lastname",
                Username = _username,
                ContactEmail = _username,
            };
            _justiceUser.JusticeUserRoles = new List<JusticeUserRole>
            {
                new JusticeUserRole { UserRole = new UserRole((int)UserRoleId.VhTeamLead, "Video Hearings Team Lead") }
            };
            _queryHandlerMock = new Mock<IQueryHandler>();
            _queryHandlerMock.Setup(x => x.Handle<GetJusticeUserByUsernameQuery, JusticeUser>(It.Is<GetJusticeUserByUsernameQuery>(
                    x => x.Username == _justiceUser.Username)))
                .ReturnsAsync(_justiceUser);
            
            _controller = new WorkHoursController(_commandHandlerMock.Object, _queryHandlerMock.Object);
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
            var objectResult = response as ObjectResult;
            objectResult.Should().NotBeNull();
            ((ValidationProblemDetails)objectResult.Value).ContainsKeyAndErrorMessage("username", $"Please provide a valid username");
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
            var objectResult = response as ObjectResult;
            objectResult.Should().NotBeNull();
            ((ValidationProblemDetails)objectResult.Value).ContainsKeyAndErrorMessage("username", $"Please provide a valid username");
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
            var response = await _controller.SaveNonWorkingHours(uploadNonWorkingHoursRequests);

            // Assert
            var objectResult = response as ObjectResult;
            objectResult.Should().NotBeNull();
            ((ValidationProblemDetails)objectResult.Value).ContainsKeyAndErrorMessage(_username, $"End time 01/01/2022 00:00:00 is before start time 01/02/2022 00:00:00.");
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
    }
}
