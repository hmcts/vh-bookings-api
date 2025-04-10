using System.Collections.Generic;
using System.Net;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Helper;
using BookingsApi.Domain;
using BookingsApi.Domain.Helper;
using BookingsApi.Domain.Validations;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Mappings;
using BookingsApi.Mappings.V1;
using Microsoft.AspNetCore.Mvc;
using Testing.Common.Assertions;

namespace BookingsApi.UnitTests.Controllers.WorkAllocationsController
{
    public class AllocateHearingManuallyTests : WorkAllocationsControllerTest
    {
        private List<VideoHearing> _hearings;
        
        [SetUp]
        public void SetUp()
        {
            var today = DateTime.UtcNow.Date.AddHours(11).AddMinutes(45).ToUniversalTime();
            var yesterdayHearing = new VideoHearingBuilder().WithCase().WithAllocatedJusticeUser().Build();
            yesterdayHearing.SetProtected(nameof(yesterdayHearing.ScheduledDateTime), today.AddDays(-1));
            _hearings = new List<VideoHearing>
            {
                new VideoHearingBuilder(today).WithCase().WithAllocatedJusticeUser().Build(),
                new VideoHearingBuilder(today).WithCase().WithAllocatedJusticeUser().Build(),
                new VideoHearingBuilder(today).WithCase().WithAllocatedJusticeUser().Build(),
                new VideoHearingBuilder(today).WithCase().WithAllocatedJusticeUser().Build(),
                new VideoHearingBuilder(today).WithCase().WithAllocatedJusticeUser().Build(),
                yesterdayHearing
            };
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
            response!.Count.Should().Be(_hearings.Count);

            var expected = checkForClashesResponse.Select(HearingAllocationResultDtoToAllocationResponseMapper.Map).ToList();
            response.Should().BeEquivalentTo(expected);

            // ensure hearing allocation event is published for all hearings today
            var message = ServiceBus.ReadMessageFromQueue();
            message.IntegrationEvent.Should().BeOfType<HearingsAllocatedIntegrationEvent>();
            var allocationEvent = message.IntegrationEvent.As<HearingsAllocatedIntegrationEvent>();
            allocationEvent.Hearings.Select(x=> x.HearingId).Should().BeEquivalentTo(_hearings.SkipLast(1).Select(x=> x.Id).ToList());
            allocationEvent.AllocatedCso.Username.Should().Be(_hearings[0].AllocatedTo.Username);
        }

        [Test]
        public async Task Should_Return_OK_And_Not_Publish_When_Hearings_Are_Not_Today()
        {
            var tomorrow = DateTime.UtcNow.Date.AddDays(1).AddHours(11).AddMinutes(45);
            var yesterdayHearing = new VideoHearingBuilder().WithCase().WithAllocatedJusticeUser().Build();
            yesterdayHearing.SetProtected(nameof(yesterdayHearing.ScheduledDateTime), tomorrow.AddDays(-2));
            _hearings = new List<VideoHearing>
            {
                new VideoHearingBuilder(tomorrow).WithCase().WithAllocatedJusticeUser().Build(),
                new VideoHearingBuilder(tomorrow).WithCase().WithAllocatedJusticeUser().Build(),
                new VideoHearingBuilder(tomorrow).WithCase().WithAllocatedJusticeUser().Build(),
                new VideoHearingBuilder(tomorrow).WithCase().WithAllocatedJusticeUser().Build(),
                new VideoHearingBuilder(tomorrow).WithCase().WithAllocatedJusticeUser().Build(),
                yesterdayHearing
            };
            
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
            response!.Count.Should().Be(_hearings.Count);

            var expected = checkForClashesResponse.Select(HearingAllocationResultDtoToAllocationResponseMapper.Map).ToList();
            response.Should().BeEquivalentTo(expected);

            // ensure hearing allocation event is published for all hearings today
            var message = ServiceBus.ReadMessageFromQueue();
            message.Should().BeNull();
        }
    }
}
