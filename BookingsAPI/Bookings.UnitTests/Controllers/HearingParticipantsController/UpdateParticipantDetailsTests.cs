
using Bookings.Api.Contract.Requests;
using Bookings.DAL.Queries;
using Bookings.Domain;
using Bookings.Domain.RefData;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;
using Testing.Common.Assertions;

namespace Bookings.UnitTests.Controllers
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
                HouseNumber = "Update 1",
                Street = "Update Street",
                City = "Update City",
                County = "Update County",
                Postcode = "ED1 5NR",
                OrganisationName = "OrgName",
                Representee = "Rep",
                SolicitorsReference = "SolRef"
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
        public async Task Should_return_badrequest_for_given_request_with_invalid_address()
        {
            var hearing = GetVideoHearing();
            hearing.Participants[0].HearingRole = new HearingRole(1, "Name") { UserRole = new UserRole(1, "Individual"), };
            request.Street = string.Empty;
            request.Postcode = string.Empty;
            participantId = hearing.Participants[0].Id;

            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(hearing);

            var result = await Controller.UpdateParticipantDetails(hearingId, participantId, request);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Should_return_badrequest_for_given_request_with_invalid_representative_details()
        {
            var hearing = GetVideoHearing();
            hearing.Participants[0].HearingRole = new HearingRole(1, "Name") { UserRole = new UserRole(1, "Representative"), };
            request.SolicitorsReference = string.Empty;
            participantId = hearing.Participants[0].Id;

            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(hearing);

            var result = await Controller.UpdateParticipantDetails(hearingId, participantId, request);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }
    }
}
