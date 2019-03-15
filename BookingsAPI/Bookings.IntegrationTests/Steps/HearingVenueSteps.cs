using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Bookings.Api.Contract.Responses;
using Bookings.IntegrationTests.Contexts;
using FluentAssertions;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;

namespace Bookings.IntegrationTests.Steps
{
    [Binding]
    public sealed class HearingVenueSteps : StepsBase
    {
        private readonly TestContext _apiTestContext;
        private readonly HearingVenueEndpoints _endpoints = new ApiUriFactory().HearingVenueEndpoints;

        public HearingVenueSteps(TestContext apiTestContext)
        {
            _apiTestContext = apiTestContext;
        }

        [Given(@"I have a get all hearing venues available for booking request")]
        public void GivenIHaveAGetAllHearingVenuesAvailableForBookingRequest()
        {           
            _apiTestContext.Uri = _endpoints.GetVenues;
            _apiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Then(@"hearing venues should be retrieved")]
        public async Task ThenHearingVenuesShouldBeRetrieved()
        {
            var json = await _apiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<HearingVenueResponse>>(json);
            foreach (var venue in model)
            {
                venue.Id.Should().BePositive();
                venue.Name.Should().NotBeNullOrEmpty();
            }            
        }
    }
}
