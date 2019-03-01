using System.Collections.Generic;
using Bookings.AcceptanceTests.Contexts;
using Bookings.Api.Contract.Responses;
using FluentAssertions;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;

namespace Bookings.AcceptanceTests.Steps
{
    [Binding]
    public sealed class HearingVenuesSteps : StepsBase
    {
        private readonly AcTestContext _acTestContext;
        private readonly HearingVenueEndpoints _endpoints = new ApiUriFactory().HearingVenueEndpoints;

        public HearingVenuesSteps(AcTestContext acTestContext)
        {
            _acTestContext = acTestContext;
        }

        [Given(@"I have a get all hearing venues available for booking request")]
        public void GivenIHaveAGetAllHearingVenuesAvailableForBookingRequest()
        {
            _acTestContext.Request = _acTestContext.Get(_endpoints.GetVenues);
        }

        [Then(@"hearing venues should be retrieved")]
        public void ThenHearingVenuesShouldBeRetrieved()
        {
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<HearingVenueResponse>>(_acTestContext.Json);
            model.Should().NotBeNullOrEmpty();
        }
    }
}
