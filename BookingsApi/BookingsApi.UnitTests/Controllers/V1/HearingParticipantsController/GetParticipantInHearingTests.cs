using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.DAL.Queries.V1;
using BookingsApi.Domain.Participants;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Testing.Common.Assertions;

namespace BookingsApi.UnitTests.Controllers.V1.HearingParticipantsController
{
    public class GetParticipantInHearingTests : HearingParticipantsControllerTest
    {
        [Test]
        public async Task Should_get_participant_in_hearing_for_given_hearing_and_participantid()
        {
            participantId = Participants[0].Id;

            var response = await Controller.GetParticipantInHearing(hearingId, participantId);

            response.Should().NotBeNull();
            var objectResult = (OkObjectResult)response;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var participantResponse = (ParticipantResponse)objectResult.Value;
            participantResponse.Should().NotBeNull();
            participantResponse.DisplayName.Should().Be("Test Participant");
            participantResponse.CaseRoleName.Should().Be("TestCaseRole");
            participantResponse.Id.Should().Be(participantId);
            QueryHandler.Verify(q => q.Handle<GetParticipantsInHearingQuery, List<Participant>>(It.IsAny<GetParticipantsInHearingQuery>()), Times.Once);
        }

        [Test]
        public async Task Should_return_badrequest_for_given_invalid_hearingid()
        {
            hearingId = Guid.Empty;

            var result = await Controller.GetParticipantInHearing(hearingId, participantId);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
            QueryHandler.Verify(q => q.Handle<GetParticipantsInHearingQuery, List<Participant>>(It.IsAny<GetParticipantsInHearingQuery>()), Times.Never);
        }

        [Test]
        public async Task Should_return_badrequest_for_given_invalid_participantid()
        {
            participantId = Guid.Empty;

            var result = await Controller.GetParticipantInHearing(hearingId, participantId);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(participantId), $"Please provide a valid {nameof(participantId)}");
            QueryHandler.Verify(q => q.Handle<GetParticipantsInHearingQuery, List<Participant>>(It.IsAny<GetParticipantsInHearingQuery>()), Times.Never);
        }

        [Test]
        public async Task Should_return_notfound_with_no_matching_participant()
        {
            var result = await Controller.GetParticipantInHearing(hearingId, participantId);

            result.Should().NotBeNull();
            var objectResult = (NotFoundResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            QueryHandler.Verify(q => q.Handle<GetParticipantsInHearingQuery, List<Participant>>(It.IsAny<GetParticipantsInHearingQuery>()), Times.Once);
        }
    }
}
