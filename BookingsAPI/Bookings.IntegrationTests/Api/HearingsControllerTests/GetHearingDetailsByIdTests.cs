using System;
using System.Net;
using System.Threading.Tasks;
using Bookings.Api.Contract.Responses;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Api;

namespace Bookings.IntegrationTests.Controllers.HearingsControllerTests
{
    public class GetHearingDetailsByIdTests : ControllerTestsBase
    {
        private readonly HearingsEndpoints _endpoints = new ApiUriFactory().HearingsEndpoints;
        private Guid _newHearingId;
        
        [SetUp]
        public void Setup()
        {
            _newHearingId = Guid.Empty;
        }
        
        [Test]
        public async Task should_get_hearing_details_bad_request_status()
        {
            var hearingId = Guid.Empty;
            var uri = _endpoints.GetHearingDetailsById(hearingId);
            var response = await SendGetRequestAsync(uri);
            TestContext.WriteLine($"Status Code: {response.StatusCode}");
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Test]
        public async Task should_get_hearing_details_not_found_status()
        {
            var hearingId = Guid.Parse("87C2A1DD-F9A3-1111-1111-03C026BAE678");
            var uri = _endpoints.GetHearingDetailsById(hearingId);
            var response = await SendGetRequestAsync(uri);
            TestContext.WriteLine($"Status Code: {response.StatusCode}");
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Test]
        public async Task should_get_hearing_details_ok_status()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;
            var uri = _endpoints.GetHearingDetailsById(_newHearingId);
            var response = await SendGetRequestAsync(uri);
            TestContext.WriteLine($"Status Code: {response.StatusCode}");
            response.IsSuccessStatusCode.Should().BeTrue();
            var json = await response.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<HearingDetailsResponse>(json);

            model.Should().NotBeNull();
            model.CaseTypeName.Should().NotBeNull();
            model.HearingTypeName.Should().NotBeNull();
            model.HearingVenueName.Should().NotBeNull();
            model.ScheduledDuration.Should().BeGreaterThan(0);
            model.ScheduledDateTime.Should().BeAfter(DateTime.MinValue);
            
            model.Cases.Should().NotBeEmpty();
            foreach (var caseResponse in model.Cases)
            {
                caseResponse.Name.Should().NotBeEmpty();
                caseResponse.Number.Should().NotBeEmpty();
            }
            
            model.Participants.Should().NotBeEmpty();
            foreach (var participant in model.Participants)
            {
                participant.Id.Should().NotBeEmpty();
                
                participant.Title.Should().NotBeEmpty();
                participant.FirstName.Should().NotBeEmpty();
                participant.LastName.Should().NotBeEmpty();
                participant.MiddleNames.Should().NotBeEmpty();
                
                participant.DisplayName.Should().NotBeEmpty();
                participant.Username.Should().NotBeEmpty();
                participant.ContactEmail.Should().NotBeEmpty();
                participant.TelephoneNumber.Should().NotBeEmpty();
                
                participant.CaseRoleName.Should().NotBeEmpty();
                participant.HearingRoleName.Should().NotBeEmpty();
                participant.UserRoleName.Should().NotBeEmpty();
            }
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