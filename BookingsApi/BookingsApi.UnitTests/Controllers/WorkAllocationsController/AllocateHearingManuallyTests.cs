using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Helper;
using BookingsApi.Domain;
using BookingsApi.Domain.Validations;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Mappings;
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
        private List<VideoHearing> _hearings;
        
        [SetUp]
        public void SetUp()
        {
            _hearings = new List<VideoHearing>
            {
                CreateHearingWithCase(),
                CreateHearingWithCase(),
                CreateHearingWithCase(),
                CreateHearingWithCase(),
                CreateHearingWithCase()
            };
        }

        private VideoHearing CreateHearingWithCase()
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.AddCase("1", "test case", true);
            return hearing;
        }

        [Test]
        public async Task Should_Return_Ok()
        {
            var userId = Guid.NewGuid();
            // Arrange
            HearingAllocationServiceMock
                .Setup(x => x.AllocateHearingsToCso(It.IsAny<List<Guid>>(), It.IsAny<Guid>()))
                .ReturnsAsync(_hearings);

            var checkForClashesResponse = _hearings.Select(x => new HearingAllocationResultDto
            {
                HearingId = x.Id,
                Duration = x.ScheduledDuration,
                ScheduledDateTime = x.ScheduledDateTime,
                CaseNumber = x.HearingCases.FirstOrDefault()?.Case.Number,
                CaseType = x.CaseType.Name,
                AllocatedCso = VideoHearingHelper.AllocatedVho(x),
                HasWorkHoursClash = null
            }).ToList();

            HearingAllocationServiceMock.Setup(x => x.CheckForAllocationClashes(_hearings)).Returns(checkForClashesResponse);
            
            var request = new UpdateHearingAllocationToCsoRequest
            {
                Hearings = _hearings.Select(h => h.Id).ToList(),
                CsoId = userId
            };
            
            // Act
            var result = await Controller.AllocateHearingManually(request);
            
            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var objectResult = (OkObjectResult)result;
            var response = (List<HearingAllocationsResponse>)objectResult.Value;
            response.Should().NotBeNull();
            response.Count.Should().Be(_hearings.Count);

            var expected = checkForClashesResponse.Select(HearingAllocationResultDtoToAllocationResponseMapper.Map).ToList();
            response.Should().BeEquivalentTo(expected);
            _eventPublisherMock.Verify(x=>x.PublishAsync(It.IsAny<IIntegrationEvent>()), Times.Once);
        }
        
        [Test]
        public async Task Should_Return_Bad_Request_When_Domain_Rule_Exception_Thrown()
        {
            var userId = Guid.NewGuid();
            // Arrange
            HearingAllocationServiceMock
                .Setup(x => x.AllocateHearingsToCso(It.IsAny<List<Guid>>(), It.IsAny<Guid>()))
                .ThrowsAsync(new DomainRuleException("Error", "Error Description"));
            
            UpdateHearingAllocationToCsoRequest request = new UpdateHearingAllocationToCsoRequest
            {
                Hearings = _hearings.Select(h => h.Id).ToList(),
                CsoId = userId
            };
            // Act
            var response = await Controller.AllocateHearingManually(request);

            // Assert
            var objectResult = (BadRequestObjectResult)response;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage("Error", "Error Description");
            _eventPublisherMock.Verify(x=>x.PublishAsync(It.IsAny<IIntegrationEvent>()), Times.Never);
        }
        
    }
}
