using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using BookingsApi.Mappings;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using Bookings.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.DAL.Queries;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace BookingsApi.UnitTests.Controllers.HearingsController
{
    public class UpdateHearingDetailsTests : HearingsControllerTests
    {
        [Test]
        public async Task should_send_message_to_bqs_with_updated_hearing()
        {
            var videoHearing = GetHearing("Original Hearing");
            videoHearing.UpdateStatus(BookingStatus.Created, "initial", null);
            var request = new UpdateHearingRequest
            {
                OtherInformation = videoHearing.OtherInformation + " Updated",
                ScheduledDuration = 999,
                UpdatedBy = "updated by test",
                ScheduledDateTime = DateTime.Today.AddDays(2),
                HearingRoomName = "Updated room name",
                HearingVenueName = "Updated venue name",
                QuestionnaireNotRequired = false,
                AudioRecordingRequired = true,
                Cases = null
            };
            var hearingVenueOriginal = videoHearing.HearingVenue;
            var newVenue = new HearingVenue(111, request.HearingVenueName);
            var updatedHearing = GetHearing("Case Update Test");
            updatedHearing.SetProtected(nameof(updatedHearing.Id), videoHearing.Id);
            updatedHearing.UpdateHearingDetails(newVenue,
                request.ScheduledDateTime, request.ScheduledDuration, request.HearingRoomName, request.OtherInformation,
                request.UpdatedBy, new List<Case>(), request.QuestionnaireNotRequired.Value,
                request.AudioRecordingRequired.Value);

            QueryHandlerMock
                .SetupSequence(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
                .ReturnsAsync(videoHearing).ReturnsAsync(updatedHearing);

            QueryHandlerMock
                .Setup(x => x.Handle<GetHearingVenuesQuery, List<HearingVenue>>(It.IsAny<GetHearingVenuesQuery>()))
                .ReturnsAsync(new List<HearingVenue> {hearingVenueOriginal, newVenue});
            var expectedResult = new HearingToDetailResponseMapper().MapHearingToDetailedResponse(updatedHearing);
            
            var result = await Controller.UpdateHearingDetails(videoHearing.Id, request);
            var ob = result.As<OkObjectResult>();
            ob.Should().NotBeNull();
            ob.Value.As<HearingDetailsResponse>().Should().BeEquivalentTo(expectedResult);
            
            var message = SbQueueClient.ReadMessageFromQueue();
            var payload = message.IntegrationEvent.As<HearingDetailsUpdatedIntegrationEvent>();
            payload.Hearing.HearingId.Should().Be(updatedHearing.Id);
            payload.Hearing.RecordAudio.Should().Be(request.AudioRecordingRequired.Value);
            payload.Hearing.ScheduledDuration.Should().Be(request.ScheduledDuration);
            payload.Hearing.ScheduledDateTime.Should().Be(request.ScheduledDateTime);
            payload.Hearing.HearingVenueName.Should().Be(request.HearingVenueName);
        }
    }
}