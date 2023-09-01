using System.Collections.Generic;
using System.Net;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using BookingsApi.Domain.Validations;
using BookingsApi.Mappings;
using BookingsApi.Mappings.V1;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Testing.Common.Assertions;

namespace BookingsApi.UnitTests.Controllers.WorkAllocationsController
{
    public class AllocateHearingAutomaticallyTests : WorkAllocationsControllerTest
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
            };
            
            _justiceUser.JusticeUserRoles = new List<JusticeUserRole>
            {
                new JusticeUserRole { UserRole = new UserRole((int)UserRoleId.Vho, "Video Hearings Office") }
            };
        }

        [Test]
        public async Task Should_Return_NotFound_When_No_Justice_User_Returned()
        {
            // Arrange
            var hearingId = Guid.NewGuid();
            HearingAllocationServiceMock
                .Setup(x => x.AllocateAutomatically(hearingId))
                .ReturnsAsync((JusticeUser)null);

            // Act
            var response = await Controller.AllocateHearingAutomatically(hearingId);

            // Assert
            var result = (NotFoundResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
