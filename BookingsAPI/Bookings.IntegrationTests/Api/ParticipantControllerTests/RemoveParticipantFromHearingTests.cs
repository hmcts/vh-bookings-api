using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bookings.DAL;
using Bookings.Domain;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Testing.Common.Builders.Api;

namespace Bookings.IntegrationTests.Api.ParticipantControllerTests
{
    public class RemoveParticipantFromHearingTests : ControllerTestsBase
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
            var uri = _endpoints.RemoveParticipantFromHearing(hearingId, participantId);
            var response = await SendDeleteRequestAsync(uri);
            TestContext.WriteLine($"Status Code: {response.StatusCode}");
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Test]
        public async Task should_get_bad_request_status_when_participant_id_is_invalid()
        {
            var hearingId = Guid.NewGuid();
            var participantId = Guid.Empty;
            var uri = _endpoints.RemoveParticipantFromHearing(hearingId, participantId);
            var response = await SendDeleteRequestAsync(uri);
            TestContext.WriteLine($"Status Code: {response.StatusCode}");
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Test]
        public async Task should_get_not_found_status_when_participant_does_not_exist()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;
            var uri = _endpoints.RemoveParticipantFromHearing(_newHearingId, Guid.NewGuid());
            var response = await SendDeleteRequestAsync(uri);
            
            TestContext.WriteLine($"Status Code: {response.StatusCode}");
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Test]
        public async Task should_remove_participant_no_content_status()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;
            var participant = seededHearing.GetParticipants().First();
            var uri = _endpoints.RemoveParticipantFromHearing(_newHearingId, participant.Id);
            var response = await SendDeleteRequestAsync(uri);
            
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

            hearingFromDb.GetParticipants().Any(x => x.Id == participant.Id).Should().BeFalse();
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