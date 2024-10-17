using System.Collections.Generic;
using System.Net;
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
        public async Task Should_return_badrequest_for_given_invalid_hearingid()
        {
            hearingId = Guid.Empty;

            var result = await Controller.RemoveParticipantFromHearing(hearingId, participantId);

            result.Should().NotBeNull();
            var objectResult = (ObjectResult)result;
            ((ValidationProblemDetails)objectResult.Value).ContainsKeyAndErrorMessage(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
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
            var objectResult = (ObjectResult)result;
            ((ValidationProblemDetails)objectResult.Value).ContainsKeyAndErrorMessage(nameof(participantId), $"Please provide a valid {nameof(participantId)}");
        }
    }
}
