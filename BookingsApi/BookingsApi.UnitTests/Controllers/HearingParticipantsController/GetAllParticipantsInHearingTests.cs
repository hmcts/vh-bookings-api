using System.Collections.Generic;
using System.Net;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Domain.Participants;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;
using Microsoft.AspNetCore.Mvc;
using Testing.Common.Assertions;

namespace BookingsApi.UnitTests.Controllers.HearingParticipantsController
{
    public class GetAllParticipantsInHearingTests : HearingParticipantsControllerTest
    { 
        [Test]
        public async Task Should_get_all_participants_in_hearing_for_given_hearingid()
        {

            var response = await Controller.GetAllParticipantsInHearing(hearingId);

            response.Should().NotBeNull();
            var objectResult = (OkObjectResult)response;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var participantResponses = (List<ParticipantResponse>)objectResult.Value;
            participantResponses.Count.Should().Be(3);
        }

        [Test]
        public async Task Should_return_badrequest_for_given_invalid_hearingid()
        {
            hearingId = Guid.Empty;

            var result = await Controller.GetAllParticipantsInHearing(hearingId);

            result.Should().NotBeNull();
            var objectResult = (ObjectResult)result;
            ((ValidationProblemDetails)objectResult.Value).ContainsKeyAndErrorMessage(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
        }

        [Test]
        public async Task Should_return_notfound_with_HearingNotFoundException()
        {
            hearingId = Guid.NewGuid();

            QueryHandler.Setup(q => q.Handle<GetParticipantsInHearingQuery, List<Participant>>(It.IsAny<GetParticipantsInHearingQuery>()))
                .ThrowsAsync(new HearingNotFoundException(hearingId));

            var result = await Controller.GetAllParticipantsInHearing(hearingId);

            result.Should().NotBeNull();
            var objectResult = (NotFoundResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
