using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using Bookings.Api.Contract.Responses;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Testing.Common.Builders.Api;

namespace Bookings.IntegrationTests.Controllers.HearingsControllerTests
{
    public class UpdateHearingDetailsTests : ControllerTestsBase
    {
        private readonly HearingsEndpoints _endpoints = new ApiUriFactory().HearingsEndpoints;
        private Guid _newHearingId;

        [SetUp]
        public void Setup()
        {
            _newHearingId = Guid.Empty;
        }

        [Test]
        public async Task should_update_hearing_return_bad_request_when_hearing_id_is_invalid()
        {
            var hearingId = Guid.Empty;
            var request = BuildRequest();
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            
            var uri = _endpoints.UpdateHearingDetails(hearingId);
            var response = await SendPutRequestAsync(uri, httpContent);
            TestContext.WriteLine($"Status Code: {response.StatusCode}");
            
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var badRequestJson = await response.Content.ReadAsStringAsync();
            var badRequestObject = JObject.Parse(badRequestJson);
            badRequestObject["hearingId"].Should().NotBeEmpty();
        }
        
        [Test]
        public async Task should_update_hearing_return_bad_request_when_request_model_is_invalid()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            _newHearingId = seededHearing.Id;
            var request = BuildRequest();
            request.HearingVenueName = string.Empty;
            request.ScheduledDuration = 0;
            request.ScheduledDateTime = DateTime.Now.AddDays(-5);
            
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            
            var uri = _endpoints.UpdateHearingDetails(_newHearingId);
            var response = await SendPutRequestAsync(uri, httpContent);
            TestContext.WriteLine($"Status Code: {response.StatusCode}");
            
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            
            var venueNotFoundResponse = await response.Content.ReadAsStringAsync();
            var badRequestJson = JObject.Parse(venueNotFoundResponse);
            badRequestJson["HearingVenueName"].Should().NotBeEmpty();
            badRequestJson["ScheduledDuration"].Should().NotBeEmpty();
            badRequestJson["ScheduledDateTime.Date"].Should().NotBeEmpty();
        }
        
        [Test]
        public async Task should_update_hearing_return_not_found_when_hearing_does_not_exist()
        {
            var hearingId = Guid.Parse("87C2A1DD-F9A3-1111-1111-03C026BAE678");
            var request = BuildRequest();
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            
            var uri = _endpoints.UpdateHearingDetails(hearingId);
            var response = await SendPutRequestAsync(uri, httpContent);
            TestContext.WriteLine($"Status Code: {response.StatusCode}");
            
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Test]
        public async Task should_update_hearing_return_not_found_when_venue_does_not_exist()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            _newHearingId = seededHearing.Id;
            var request = BuildRequest();
            request.HearingVenueName = "Does Not Exist";
            
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            
            var uri = _endpoints.UpdateHearingDetails(_newHearingId);
            var response = await SendPutRequestAsync(uri, httpContent);
            TestContext.WriteLine($"Status Code: {response.StatusCode}");
            
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            
            var venueNotFoundResponse = await response.Content.ReadAsStringAsync();
            var badRequestJson = JObject.Parse(venueNotFoundResponse);
            badRequestJson["HearingVenueName"].Should().NotBeEmpty();
        }

        [Test]
        public async Task should_update_hearing_return_accepted_when_hearing_is_updated()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            _newHearingId = seededHearing.Id;
            var request = BuildRequest();
            
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            
            var uri = _endpoints.UpdateHearingDetails(_newHearingId);
            var response = await SendPutRequestAsync(uri, httpContent);
            TestContext.WriteLine($"Status Code: {response.StatusCode}");
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.Accepted);
            
            var json = await response.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<HearingDetailsResponse>(json);

            model.ScheduledDuration.Should().Be(request.ScheduledDuration);
            model.HearingVenueName.Should().Be(request.HearingVenueName);
            model.ScheduledDateTime.Should().Be(request.ScheduledDateTime);
        }
        
        private UpdateHearingRequest BuildRequest()
        {
            return new UpdateHearingRequest
            {
                ScheduledDateTime = DateTime.Today.AddDays(3).AddHours(11).AddMinutes(45),
                ScheduledDuration = 100,
                HearingVenueName = "Manchester Civil and Family Justice Centre"
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