using System.Collections.Generic;
using System.Threading.Tasks;
using Bookings.Api.Contract.Responses;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Api;

namespace Bookings.IntegrationTests.Api
{
    public class HearingVenuesControllerTests : ControllerTestsBase
    {
        private readonly HearingVenueEndpoints _endpoints = new ApiUriFactory().HearingVenueEndpoints;
        
        [Test]
        public async Task should_get_venues_ok_status()
        {
            var uri = _endpoints.GetVenues;
            var response = await SendGetRequestAsync(uri);
            TestContext.WriteLine($"Status Code: {response.StatusCode}");
            response.IsSuccessStatusCode.Should().BeTrue();
            var json = await response.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<HearingVenueResponse>>(json);

            model.Should().NotBeEmpty();
        }
    }
}