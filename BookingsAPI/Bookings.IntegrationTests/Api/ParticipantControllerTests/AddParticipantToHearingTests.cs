using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using Bookings.DAL;
using Bookings.Domain;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Testing.Common.Builders.Api;
using Testing.Common.Builders.Api.Request;

namespace Bookings.IntegrationTests.Api.ParticipantControllerTests
{
    public class AddParticipantToHearingTests : ControllerTestsBase
    {
        private readonly ParticipantsEndpoints _endpoints = new ApiUriFactory().ParticipantsEndpoints;
        private Guid _newHearingId;

        [SetUp]
        public void Setup()
        {
            _newHearingId = Guid.Empty;
        }

        [Test]
        public async Task should_get_bad_request_status_when_hearing_id_is_invalid()
        {
            var hearingId = Guid.Empty;
            var request = BuildRequest();
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var uri = _endpoints.AddParticipantsToHearing(hearingId);

            var response = await SendPutRequestAsync(uri, httpContent);
            TestContext.WriteLine($"Status Code: {response.StatusCode}");

            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var badRequestJson = await response.Content.ReadAsStringAsync();
            var badRequestObject = JObject.Parse(badRequestJson);
            badRequestObject["hearingId"].Should().NotBeEmpty();
        }

        [Test]
        public async Task should_get_bad_request_status_when_request_body_is_invalid()
        {
            var hearingId = Guid.NewGuid();
            var request = BuildRequest();
            request.Participants[0].Title = string.Empty;

            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var uri = _endpoints.AddParticipantsToHearing(hearingId);

            var response = await SendPutRequestAsync(uri, httpContent);
            TestContext.WriteLine($"Status Code: {response.StatusCode}");

            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task should_get_not_found_status_when_hearing_does_not_exist()
        {
            var hearingId = Guid.NewGuid();
            var request = BuildRequest();
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var uri = _endpoints.AddParticipantsToHearing(hearingId);

            var response = await SendPutRequestAsync(uri, httpContent);
            TestContext.WriteLine($"Status Code: {response.StatusCode}");

            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task should_add_participants_no_content_status()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;
            var request = BuildRequest();
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var uri = _endpoints.AddParticipantsToHearing(_newHearingId);

            var response = await SendPutRequestAsync(uri, httpContent);
            TestContext.WriteLine($"Status Code: {response.StatusCode}");

            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            Hearing hearingFromDb;
            using (var db = new BookingsDbContext(BookingsDbContextOptions))
            {
                hearingFromDb = db.VideoHearings
                    .Include(x => x.Participants).ThenInclude(x => x.Person).AsNoTracking()
                    .Single(x => x.Id == _newHearingId);
            }

            foreach (var participantRequest in request.Participants)
            {
                hearingFromDb.GetParticipants().Any(x => x.Person.Username == participantRequest.Username).Should()
                    .BeTrue();
            }
        }

        private AddParticipantsToHearingRequest BuildRequest()
        {
            var newParticipant = new ParticipantRequestBuilder("Defendant", "Defendant LIP").Build();

            return new AddParticipantsToHearingRequest
            {
                Participants = new List<ParticipantRequest> {newParticipant}
            };
        }

        [TearDown]
        public async Task TearDown()
        {
            if (_newHearingId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test hearing {_newHearingId}");
                await Hooks.RemoveVideoHearing(_newHearingId);
            }
        }
    }
}