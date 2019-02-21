using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bookings.Api.Contract.Responses;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Api;

namespace Bookings.IntegrationTests.Api.ParticipantControllerTests
{
    public class GetParticipantInHearingTests : ControllerTestsBase
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
            var participantId = Guid.Empty;
            var uri = _endpoints.GetParticipantInHearing(hearingId, participantId);
            var response = await SendGetRequestAsync(uri);
            TestContext.WriteLine($"Status Code: {response.StatusCode}");
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Test]
        public async Task should_get_bad_request_status_when_participant_id_is_invalid()
        {
            var hearingId = Guid.NewGuid();
            var participantId = Guid.Empty;
            var uri = _endpoints.GetParticipantInHearing(hearingId, participantId);
            var response = await SendGetRequestAsync(uri);
            TestContext.WriteLine($"Status Code: {response.StatusCode}");
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Test]
        public async Task should_get_participant_not_found_status()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;
            var uri = _endpoints.GetParticipantInHearing(_newHearingId, Guid.NewGuid());
            var response = await SendGetRequestAsync(uri);
            
            TestContext.WriteLine($"Status Code: {response.StatusCode}");
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task should_get_participant_ok_status()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;
            var participant = seededHearing.GetParticipants().First();
            var person = participant.Person;
            var uri = _endpoints.GetParticipantInHearing(_newHearingId, participant.Id);
            var response = await SendGetRequestAsync(uri);
            
            TestContext.WriteLine($"Status Code: {response.StatusCode}");
            response.IsSuccessStatusCode.Should().BeTrue();
            var json = await response.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<ParticipantResponse>(json);

            
            model.Id.Should().Be(participant.Id);
                
            model.Title.Should().Be(person.Title);
            model.FirstName.Should().Be(person.FirstName);
            model.LastName.Should().Be(person.LastName);
            model.MiddleNames.Should().Be(person.MiddleNames);

            model.DisplayName.Should().Be(participant.DisplayName);
            model.Username.Should().Be(person.Username);
            model.ContactEmail.Should().Be(person.ContactEmail);
            model.TelephoneNumber.Should().Be(person.TelephoneNumber);

            model.CaseRoleName.Should().NotBeNullOrWhiteSpace();
            model.HearingRoleName.Should().NotBeNullOrWhiteSpace();
            model.UserRoleName.Should().NotBeNullOrWhiteSpace();
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