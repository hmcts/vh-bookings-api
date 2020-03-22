using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Bookings.Api.Contract.Responses;
using Bookings.DAL.Exceptions;
using Bookings.DAL.Queries;
using Bookings.Domain.Participants;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Testing.Common.Assertions;

namespace Bookings.UnitTests.Controllers.HearingParticipantsController
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
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
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
