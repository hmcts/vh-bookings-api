using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using BookingsApi.Domain.Validations;
using BookingsApi.Mappings;
using BookingsApi.UnitTests.Controllers.HearingsController;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Testing.Common.Assertions;
using Testing.Common.Builders.Domain;

namespace BookingsApi.UnitTests.Controllers.WorkAllocationsController
{
    public class AllocateHearingManuallyTests : WorkAllocationsControllerTest
    {
        private JusticeUser _justiceUser;
        private List<VideoHearing> _hearings;
        
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

            _hearings = new List<VideoHearing>();
            _hearings.Add(new VideoHearingBuilder().Build());
            _hearings.Add(new VideoHearingBuilder().Build());
            _hearings.Add(new VideoHearingBuilder().Build());
            _hearings.Add(new VideoHearingBuilder().Build());
            _hearings.Add(new VideoHearingBuilder().Build());
            
        }
        
        [Test]
        public async Task Should_Return_Ok()
        {
            var userId = Guid.NewGuid();
            // Arrange
            HearingAllocationServiceMock
                .Setup(x => x.AllocateHearingsToCso(It.IsAny<List<Guid>>(), It.IsAny<Guid>()))
                .ReturnsAsync(_hearings);

            var expectedHearingsResponse = _hearings.Select(HearingToDetailsResponseMapper.Map).ToList();
            var expectedHearingsResponseJson = JsonConvert.SerializeObject(expectedHearingsResponse);

            UpdateHearingAllocationToCsoRequest request = new UpdateHearingAllocationToCsoRequest();
            request.Hearings = _hearings.Select(h => h.Id).ToList();
            request.CsoId = userId;
            // Act
            var response = await Controller.AllocateHearingManually(request);

            // Assert
            var result = (OkObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var actualHearingsResponseJson = JsonConvert.SerializeObject(result.Value);
            Assert.AreEqual(expectedHearingsResponseJson, actualHearingsResponseJson);
        }
        
        [Test]
        public async Task Should_Return_Bad_Request_When_Domain_Rule_Exception_Thrown()
        {
            var userId = Guid.NewGuid();
            // Arrange
            HearingAllocationServiceMock
                .Setup(x => x.AllocateHearingsToCso(It.IsAny<List<Guid>>(), It.IsAny<Guid>()))
                .ThrowsAsync(new DomainRuleException("Error", "Error Description"));
            
            UpdateHearingAllocationToCsoRequest request = new UpdateHearingAllocationToCsoRequest();
            request.Hearings = _hearings.Select(h => h.Id).ToList();
            request.CsoId = userId;
            // Act
            var response = await Controller.AllocateHearingManually(request);

            // Assert
            var objectResult = (BadRequestObjectResult)response;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage("Error", "Error Description");
        }
        
    }
}
