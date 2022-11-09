using System;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using BookingsApi.Domain.Validations;
using BookingsApi.Mappings;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Testing.Common.Assertions;

namespace BookingsApi.UnitTests.Controllers.HearingsController
{
    public class AllocateHearingAutomaticallyTests : HearingsControllerTests
    {
        private JusticeUser _justiceUser;
        
        [SetUp]
        public void SetUp()
        {
            _justiceUser = new JusticeUser
            {
                FirstName = "FirstName",
                Lastname = "Lastname",
                Username = "email.test@email.com",
                ContactEmail = "email.test@email.com",
                UserRole = new UserRole((int)UserRoleId.Vho, "Video Hearings CSO")
            };
        }
        
        [Test]
        public async Task Should_Return_Ok()
        {
            // Arrange
            var hearingId = Guid.NewGuid();
            HearingAllocationServiceMock
                .Setup(x => x.AllocateCso(hearingId))
                .ReturnsAsync(_justiceUser);
            
            var expectedJusticeUserResponse = JusticeUserToResponseMapper.Map(_justiceUser);
            var expectedJusticeUserResponseJson = JsonConvert.SerializeObject(expectedJusticeUserResponse);

            // Act
            var response = await Controller.AllocateHearingAutomatically(hearingId);

            // Assert
            var result = (OkObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var actualJusticeUserResponseJson = JsonConvert.SerializeObject(result.Value);
            Assert.AreEqual(expectedJusticeUserResponseJson, actualJusticeUserResponseJson);
        }

        [Test]
        public async Task Should_Return_Bad_Request_When_Domain_Rule_Exception_Thrown()
        {
            // Arrange
            var hearingId = Guid.NewGuid();
            HearingAllocationServiceMock
                .Setup(x => x.AllocateCso(hearingId))
                .ThrowsAsync(new DomainRuleException("Error", "Error Description"));

            // Act
            var response = await Controller.AllocateHearingAutomatically(hearingId);
            
            // Assert
            var objectResult = (BadRequestObjectResult)response;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage("Error", "Error Description");
        }

        [Test]
        public async Task Should_Return_NotFound_When_No_Justice_User_Returned()
        {
            // Arrange
            var hearingId = Guid.NewGuid();
            HearingAllocationServiceMock
                .Setup(x => x.AllocateCso(hearingId))
                .ReturnsAsync((JusticeUser)null);

            // Act
            var response = await Controller.AllocateHearingAutomatically(hearingId);

            // Assert
            var result = (NotFoundResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
