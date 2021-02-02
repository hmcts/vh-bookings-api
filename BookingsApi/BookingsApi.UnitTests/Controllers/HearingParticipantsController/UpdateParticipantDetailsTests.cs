using System;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using Bookings.Domain;
using Bookings.Domain.Participants;
using Bookings.Domain.RefData;
using Bookings.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.DAL.Queries;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Testing.Common.Assertions;

namespace BookingsApi.UnitTests.Controllers.HearingParticipantsController
{
    public class UpdateParticipantDetailsTests : HearingParticipantsControllerTest
    {
        private UpdateParticipantRequest request;

        [SetUp]
        public void TestInitialize()
        {
            request = new UpdateParticipantRequest
            {
                Title = "Mr",
                DisplayName = "Update Display Name",
                TelephoneNumber = "11112222333",
                OrganisationName = "OrgName",
                Representee = "Rep",
            };
        }

        [Test]
        public async Task Should_return_badrequest_for_given_invalid_hearingid()
        {
            hearingId = Guid.Empty;

            var result = await Controller.UpdateParticipantDetails(hearingId, participantId, request);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
        }

        [Test]
        public async Task Should_return_badrequest_for_given_invalid_participantId()
        {
            participantId = Guid.Empty;

            var result = await Controller.UpdateParticipantDetails(hearingId, participantId, request);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(participantId), $"Please provide a valid {nameof(participantId)}");
        }

        [Test]
        public async Task Should_return_badrequest_for_given_invalid_request()
        {
            var result = await Controller.UpdateParticipantDetails(hearingId, participantId, new UpdateParticipantRequest());

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage("DisplayName", "Display name is required");
        }

        [Test]
        public async Task Should_return_notfound_with_no_matching_videohearing()
        {
            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync((VideoHearing)null);

            var result = await Controller.UpdateParticipantDetails(hearingId, participantId, request);

            result.Should().NotBeNull();
            var objectResult = (NotFoundResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            QueryHandler.Verify(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()), Times.Once);
        }

        [Test]
        public async Task Should_return_notfound_with_no_matching_participants()
        {
            var videoHearing = Builder<VideoHearing>.CreateNew().Build();

            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(videoHearing);

            var result = await Controller.UpdateParticipantDetails(hearingId, participantId, request);

            result.Should().NotBeNull();
            var objectResult = (NotFoundResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            QueryHandler.Verify(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()), Times.Once);
        }

        [Test]
        public async Task Should_pablish_event_if_state_is_created()
        {
            var videoHearing = GetVideoHearing(true);

            var participant = videoHearing.Participants[0];
            EventPublisher.Setup(x => x.PublishAsync(It.IsAny<ParticipantUpdatedIntegrationEvent>())).Verifiable();
            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(videoHearing);

            var result = await Controller.UpdateParticipantDetails(hearingId, participant.Id, request);

            result.Should().NotBeNull();
            var objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            EventPublisher.Verify(x => x.PublishAsync(It.IsAny<ParticipantUpdatedIntegrationEvent>()), Times.Once);
        }

        [Test]
        public async Task Should_return_badrequest_for_given_request_with_invalid_representative_details()
        {
            var hearing = GetVideoHearing();
            hearing.Participants[0].HearingRole = new HearingRole(1, "Name") { UserRole = new UserRole(1, "Representative"), };
            participantId = hearing.Participants[0].Id;

            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(hearing);
            request.Representee = string.Empty;
            var result = await Controller.UpdateParticipantDetails(hearingId, participantId, request);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }
    }
}
