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
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
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
        public async Task UpdateVhoNonAvailabilityHours_With_Valid_Request_Returns_NoContent()
        {
            // Arrange
            var username = "test.user@hearings.reform.hmcts.net";

            var request = new UpdateNonWorkingHoursRequest
            {
                Hours = new List<NonWorkingHours>
                {
                    new()
                    {
                        Id = 1, 
                        StartTime = new DateTime(2022, 1, 1, 6, 0, 0, DateTimeKind.Utc), 
                        EndTime = new DateTime(2022, 1, 2, 10, 0, 0, DateTimeKind.Utc)
                    }
                }
            };

            _queryHandlerMock
                .Setup(x => x.Handle<GetVhoNonAvailableWorkHoursQuery, List<VhoNonAvailability>>(It.IsAny<GetVhoNonAvailableWorkHoursQuery>()))
                .ReturnsAsync(new List<VhoNonAvailability>
                {
                    new(1)
                    {
                        StartTime = new DateTime(2022, 1, 1, 8, 0, 0, DateTimeKind.Utc), 
                        EndTime = new DateTime(2022, 1, 2, 11, 0, 0, DateTimeKind.Utc)
                    },
                    new(2)
                    { 
                        StartTime = new DateTime(2022, 1, 1, 6, 0, 0, DateTimeKind.Utc), 
                        EndTime = new DateTime(2022, 1, 2, 10, 0, 0, DateTimeKind.Utc),
                        Deleted = true
                    }
                });
            
            // Act
            var response = await _controller.UpdateVhoNonAvailabilityHours(_username, request);

            // Assert
            var objectResult = (NoContentResult)response;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
            
            _commandHandlerMock.Verify(x => x.Handle(It.IsAny<UpdateNonWorkingHoursCommand>()), Times.Once);
        }

        [Test]
        public async Task UpdateVhoNonAvailabilityHours_With_EndTime_Before_StartTime_Returns_BadRequest()
        {
            // Arrange
            var username = "test.user@hearings.reform.hmcts.net";
            
            var request = new UpdateNonWorkingHoursRequest
            {
                Hours = new List<NonWorkingHours>
                {
                    new()
                    {
                        Id = 1, 
                        StartTime = new DateTime(2022, 1, 1, 6, 0, 0, DateTimeKind.Utc), 
                        EndTime = new DateTime(2022, 1, 1, 10, 0, 0, DateTimeKind.Utc)
                    },
                    new()
                    {
                        Id = 2, 
                        StartTime = new DateTime(2022, 1, 2, 8, 0, 0, DateTimeKind.Utc), 
                        EndTime = new DateTime(2022, 1, 2, 6, 0, 0, DateTimeKind.Utc)
                    },
                    new()
                    {
                        Id = 3, 
                        StartTime = new DateTime(2022, 1, 2, 8, 0, 0, DateTimeKind.Utc), 
                        EndTime = new DateTime(2022, 1, 1, 6, 0, 0, DateTimeKind.Utc)
                    }
                }
            };
            
            _queryHandlerMock
                .Setup(x => x.Handle<GetVhoNonAvailableWorkHoursQuery, List<VhoNonAvailability>>(It.IsAny<GetVhoNonAvailableWorkHoursQuery>()))
                .ReturnsAsync(new List<VhoNonAvailability>
                {
                    new(1)
                    {
                        StartTime = new DateTime(2022, 1, 1, 6, 0, 0, DateTimeKind.Utc), 
                        EndTime = new DateTime(2022, 1, 1, 8, 0, 0, DateTimeKind.Utc)
                    },
                    new(2)
                    {
                        StartTime = new DateTime(2022, 1, 2, 6, 0, 0, DateTimeKind.Utc), 
                        EndTime = new DateTime(2022, 1, 2, 8, 0, 0, DateTimeKind.Utc)
                    },
                    new(3)
                    {
                        StartTime = new DateTime(2022, 1, 3, 6, 0, 0, DateTimeKind.Utc), 
                        EndTime = new DateTime(2022, 1, 3, 8, 0, 0, DateTimeKind.Utc)
                    }
                });
            
            // Act
            var response = await _controller.UpdateVhoNonAvailabilityHours(_username, request);

            // Assert
            var objectResult = (BadRequestObjectResult)response;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage("Hours[1].EndTime",  NonWorkingHoursRequestValidation.EndTimeErrorMessage);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage("Hours[2].EndTime", NonWorkingHoursRequestValidation.EndTimeErrorMessage);
            
            _commandHandlerMock.Verify(x => x.Handle(It.IsAny<UpdateNonWorkingHoursCommand>()), Times.Never);
        }
        
        [Test]
        public async Task UpdateVhoNonAvailabilityHours_With_EndTime_EqualTo_StartTime_Returns_BadRequest()
        {
            // Arrange
            var username = "test.user@hearings.reform.hmcts.net";
            
            var request = new UpdateNonWorkingHoursRequest
            {
                Hours = new List<NonWorkingHours>
                {
                    new()
                    {
                        Id = 1, 
                        StartTime = new DateTime(2022, 1, 1, 8, 0, 0, DateTimeKind.Utc), 
                        EndTime = new DateTime(2022, 1, 1, 8, 0, 0, DateTimeKind.Utc)
                    },
                    new()
                    {
                        Id = 2, 
                        StartTime = new DateTime(2022, 1, 2, 6, 0, 0, DateTimeKind.Utc), 
                        EndTime = new DateTime(2022, 1, 2, 6, 0, 0, DateTimeKind.Utc)
                    },
                    new()
                    {
                        Id = 3, 
                        StartTime = new DateTime(2022, 1, 3, 6, 0, 0, DateTimeKind.Utc), 
                        EndTime = new DateTime(2022, 1, 3, 10, 0, 0, DateTimeKind.Utc)
                    }
                }
            };
            
            _queryHandlerMock
                .Setup(x => x.Handle<GetVhoNonAvailableWorkHoursQuery, List<VhoNonAvailability>>(It.IsAny<GetVhoNonAvailableWorkHoursQuery>()))
                .ReturnsAsync(new List<VhoNonAvailability>
                {
                    new(1)
                    {
                        StartTime = new DateTime(2022, 1, 1, 6, 0, 0, DateTimeKind.Utc), 
                        EndTime = new DateTime(2022, 1, 1, 8, 0, 0, DateTimeKind.Utc)
                    },
                    new(2)
                    {
                        StartTime = new DateTime(2022, 1, 2, 6, 0, 0, DateTimeKind.Utc), 
                        EndTime = new DateTime(2022, 1, 2, 8, 0, 0, DateTimeKind.Utc)
                    },
                    new(3)
                    {
                        StartTime = new DateTime(2022, 1, 3, 6, 0, 0, DateTimeKind.Utc), 
                        EndTime = new DateTime(2022, 1, 3, 8, 0, 0, DateTimeKind.Utc)
                    }
                });
            
            // Act
            var response = await _controller.UpdateVhoNonAvailabilityHours(_username, request);

            // Assert
            var objectResult = (BadRequestObjectResult)response;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage("Hours[0].EndTime", NonWorkingHoursRequestValidation.EndTimeErrorMessage);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage("Hours[1].EndTime", NonWorkingHoursRequestValidation.EndTimeErrorMessage);
            
            _commandHandlerMock.Verify(x => x.Handle(It.IsAny<UpdateNonWorkingHoursCommand>()), Times.Never);
        }

        [Test]
        public async Task UpdateVhoNonAvailabilityHours_With_NonExistent_Hours_Returns_NotFound()
        {
            // Arrange
            var username = "test.user@hearings.reform.hmcts.net";
            var request = new UpdateNonWorkingHoursRequest
            {
                Hours = new List<NonWorkingHours>
                {
                    new()
                    {
                        Id = 99, 
                        StartTime = new DateTime(2022, 1, 1, 6, 0, 0, DateTimeKind.Utc), 
                        EndTime = new DateTime(2022, 1, 1, 10, 0, 0, DateTimeKind.Utc)
                    }
                }
            };
            
            _queryHandlerMock
                .Setup(x => x.Handle<GetVhoNonAvailableWorkHoursQuery, List<VhoNonAvailability>>(It.IsAny<GetVhoNonAvailableWorkHoursQuery>()))
                .ReturnsAsync(new List<VhoNonAvailability>
                {
                    new()
                    {
                        StartTime = new DateTime(2022, 1, 2, 6, 0, 0, DateTimeKind.Utc), 
                        EndTime = new DateTime(2022, 1, 2, 10, 0, 0, DateTimeKind.Utc)
                    }
                });
            
            // Act
            var response = await _controller.UpdateVhoNonAvailabilityHours(username, request);
            
            // Assert
            var objectResult = (NotFoundResult)response;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            
            _commandHandlerMock.Verify(x => x.Handle(It.IsAny<UpdateNonWorkingHoursCommand>()), Times.Never);
        }
        
        [Test]
        public async Task UpdateVhoNonAvailabilityHours_With_No_Existing_Hours_Returns_NotFound()
        {
            // Arrange
            var username = "test.user@hearings.reform.hmcts.net";
            var request = new UpdateNonWorkingHoursRequest
            {
                Hours = new List<NonWorkingHours>
                {
                    new()
                    {
                        Id = 99, 
                        StartTime = new DateTime(2022, 1, 1, 6, 0, 0, DateTimeKind.Utc), 
                        EndTime = new DateTime(2022, 1, 1, 10, 0, 0, DateTimeKind.Utc)
                    }
                }
            };
            
            _queryHandlerMock
                .Setup(x => x.Handle<GetVhoNonAvailableWorkHoursQuery, List<VhoNonAvailability>>(It.IsAny<GetVhoNonAvailableWorkHoursQuery>()))
                .ReturnsAsync((List<VhoNonAvailability>)null);
            
            // Act
            var response = await _controller.UpdateVhoNonAvailabilityHours(username, request);
            
            // Assert
            var objectResult = (NotFoundResult)response;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            
            _commandHandlerMock.Verify(x => x.Handle(It.IsAny<UpdateNonWorkingHoursCommand>()), Times.Never);
        }
        
        [Test]
        public async Task UpdateVhoNonAvailabilityHours_With_Overlapping_Times_Returns_BadRequest()
        {
            // Arrange
            var username = "test.user@hearings.reform.hmcts.net";
            var request = new UpdateNonWorkingHoursRequest
            {
                Hours = new List<NonWorkingHours>
                {
                    new()
                    {
                        Id = 1, 
                        StartTime = new DateTime(2022, 1, 1, 8, 0, 0, DateTimeKind.Utc), 
                        EndTime = new DateTime(2022, 1, 1, 12, 0, 0, DateTimeKind.Utc)
                    },
                    new()
                    {
                        Id = 2, 
                        StartTime = new DateTime(2022, 1, 1, 10, 0, 0, DateTimeKind.Utc), 
                        EndTime = new DateTime(2022, 1, 1, 13, 0, 0, DateTimeKind.Utc)
                    }
                }
            };
            
            _queryHandlerMock
                .Setup(x => x.Handle<GetVhoNonAvailableWorkHoursQuery, List<VhoNonAvailability>>(It.IsAny<GetVhoNonAvailableWorkHoursQuery>()))
                .ReturnsAsync(new List<VhoNonAvailability>
                {
                    new(1)
                    {
                        StartTime = new DateTime(2022, 1, 1, 8, 0, 0, DateTimeKind.Utc), 
                        EndTime = new DateTime(2022, 1, 1, 12, 0, 0, DateTimeKind.Utc)
                    },
                    new(2)
                    {
                        StartTime = new DateTime(2022, 1, 2, 10, 0, 0, DateTimeKind.Utc), 
                        EndTime = new DateTime(2022, 1, 2, 13, 0, 0, DateTimeKind.Utc)
                    },
                });
            
            // Act
            var response = await _controller.UpdateVhoNonAvailabilityHours(_username, request);
            
            // Assert
            var objectResult = (BadRequestObjectResult)response;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage("Hours", UpdateNonWorkingHoursRequestValidation.HoursOverlapErrorMessage);
            
            _commandHandlerMock.Verify(x => x.Handle(It.IsAny<UpdateNonWorkingHoursCommand>()), Times.Never);
        }

        [Test]
        public async Task UpdateVhoNonAvailabilityHours_With_Empty_Hours_List_In_Request_BadRequest()
        {
            // Arrange
            var username = "test.user@hearings.reform.hmcts.net";
            var request = new UpdateNonWorkingHoursRequest
            {
                Hours = new List<NonWorkingHours>()
            };

            // Act
            var response = await _controller.UpdateVhoNonAvailabilityHours(_username, request);
            
            // Assert
            var objectResult = (BadRequestObjectResult)response;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage("Hours", UpdateNonWorkingHoursRequestValidation.HoursEmptyErrorMessage);
            
            _commandHandlerMock.Verify(x => x.Handle(It.IsAny<UpdateNonWorkingHoursCommand>()), Times.Never);
        }
        
        [Test]
        public async Task UpdateVhoNonAvailabilityHours_With_Null_Hours_List_In_Request_BadRequest()
        {
            // Arrange
            var username = "test.user@hearings.reform.hmcts.net";
            var request = new UpdateNonWorkingHoursRequest
            {
                Hours = null
            };

            // Act
            var response = await _controller.UpdateVhoNonAvailabilityHours(_username, request);
            
            // Assert
            var objectResult = (BadRequestObjectResult)response;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage("Hours", UpdateNonWorkingHoursRequestValidation.HoursEmptyErrorMessage);
            
            _commandHandlerMock.Verify(x => x.Handle(It.IsAny<UpdateNonWorkingHoursCommand>()), Times.Never);
        }
        
        [Test]
        public async Task DeleteVhoNonAvailabilityHours_Not_Valid_Id()
        {
            // Act
            var response = await _controller.DeleteVhoNonAvailabilityHours(0);
            
            // Assert
            var objectResult = (BadRequestObjectResult)response;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            
            _commandHandlerMock.Verify(x => x.Handle(It.IsAny<DeleteNonWorkingHoursCommand>()), Times.Never);
        }
        
        
        [Test]
        public async Task DeleteVhoNonAvailabilityHours_Not_Found_Id()
        {
            _commandHandlerMock
                .Setup(x => x.Handle(It.IsAny<DeleteNonWorkingHoursCommand>()))
                .ThrowsAsync(new NonWorkingHoursNotFoundException(1));
            
            // Act
            var response = await _controller.DeleteVhoNonAvailabilityHours(1);
            
            // Assert
            response.Should().NotBeNull();
            // Assert
            var objectResult = (NotFoundObjectResult)response;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
        
        [Test]
        public async Task DeleteVhoNonAvailabilityHours_Valid_Id()
        {
            // Act
            var response = await _controller.DeleteVhoNonAvailabilityHours(1);
            
            // Assert
            response.Should().NotBeNull();
            // Assert
            var objectResult = (OkResult)response;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }
    }
}
