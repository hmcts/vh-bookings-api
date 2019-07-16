using System.Collections.Generic;
using Bookings.AcceptanceTests.Contexts;
using Bookings.Api.Contract.Responses;
using FluentAssertions;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;

namespace Bookings.AcceptanceTests.Steps
{
    [Binding]
    public sealed class HearingVenuesSteps
    {
        private readonly TestContext _context;
        private readonly HearingVenueEndpoints _endpoints = new ApiUriFactory().HearingVenueEndpoints;

        public HearingVenuesSteps(TestContext context)
        {
            _context = context;
        }

        [Given(@"I have a get all hearing venues available for booking request")]
        public void GivenIHaveAGetAllHearingVenuesAvailableForBookingRequest()
        {
            _context.Request = _context.Get(_endpoints.GetVenues);
        }

        [Then(@"hearing venues should be retrieved")]
        public void ThenHearingVenuesShouldBeRetrieved()
        {
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<HearingVenueResponse>>(_context.Json);
            model.Should().NotBeNullOrEmpty();
        }
    }
}