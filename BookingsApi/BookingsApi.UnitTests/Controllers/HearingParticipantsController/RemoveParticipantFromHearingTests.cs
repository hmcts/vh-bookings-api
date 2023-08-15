using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Queries;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using Microsoft.AspNetCore.Mvc;
using Testing.Common.Assertions;

namespace BookingsApi.UnitTests.Controllers.HearingParticipantsController
{
    public class RemoveParticipantFromHearingTests : HearingParticipantsControllerTest
    {
        [Test]
        public async Task Should_remove_participant_from_hearing_for_given_hearing_and_participantid()
        {
            participantId = Participants[0].Id;
            
            var response = await Controller.RemoveParticipantFromHearing(hearingId, participantId);

            response.Should().NotBeNull();
            var result = (NoContentResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
            QueryHandler.Verify(q => q.Handle<GetParticipantsInHearingQuery, List<Participant>>(It.IsAny<GetParticipantsInHearingQuery>()), Times.Once);
            QueryHandler.Verify(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()), Times.Once);
            CommandHandler.Verify(c => c.Handle(It.IsAny<RemoveParticipantFromHearingCommand>()), Times.Once);
            EventPublisher.Verify(e => e.PublishAsync(It.IsAny<ParticipantRemovedIntegrationEvent>()), Times.Never);
        }

        [Test]
        public async Task Should_remove_participant_and_publish_event_from_hearing_for_given_hearing_and_participantid()
        {
            participantId = Participants[0].Id;

            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(GetVideoHearing(true));

            var response = await Controller.RemoveParticipantFromHearing(hearingId, participantId);

            response.Should().NotBeNull();
            var result = (NoContentResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
            QueryHandler.Verify(q => q.Handle<GetParticipantsInHearingQuery, List<Participant>>(It.IsAny<GetParticipantsInHearingQuery>()), Times.Once);
            QueryHandler.Verify(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()), Times.Once);
            CommandHandler.Verify(c => c.Handle(It.IsAny<RemoveParticipantFromHearingCommand>()), Times.Once);
            EventPublisher.Verify(e => e.PublishAsync(It.IsAny<ParticipantRemovedIntegrationEvent>()), Times.Once);
        }

        [Test]
        public async Task Should_return_badrequest_for_given_invalid_hearingid()
        {
            hearingId = Guid.Empty;

            var result = await Controller.RemoveParticipantFromHearing(hearingId, participantId);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
        }

        [Test]
        public async Task Should_return_notfound_with_no_matching_videohearing()
        {
            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync((VideoHearing)null);

            var result = await Controller.RemoveParticipantFromHearing(hearingId, participantId);

            result.Should().NotBeNull();
            var objectResult = (NotFoundResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            QueryHandler.Verify(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()), Times.Once);
        }

        [Test]
        public async Task Should_return_notfound_with_no_matching_participant()
        {
            participantId = Guid.NewGuid();

            var result = await Controller.RemoveParticipantFromHearing(hearingId, participantId);

            result.Should().NotBeNull();
            var objectResult = (NotFoundResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            QueryHandler.Verify(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()), Times.Once);
        }

        [Test]
        public async Task Should_return_badrequest_for_given_invalid_participantid()
        {
            participantId = Guid.Empty;

            var result = await Controller.RemoveParticipantFromHearing(hearingId, participantId);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(participantId), $"Please provide a valid {nameof(participantId)}");
        }
    }
}
