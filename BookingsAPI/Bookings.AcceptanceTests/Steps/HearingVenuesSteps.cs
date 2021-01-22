﻿using System.Collections.Generic;
using AcceptanceTests.Common.Api.Helpers;
using Bookings.AcceptanceTests.Contexts;
using Bookings.Api.Contract.Responses;
using FluentAssertions;
using TechTalk.SpecFlow;
using static Testing.Common.Builders.Api.ApiUriFactory.HearingVenueEndpoints;

namespace Bookings.AcceptanceTests.Steps
{
    [Binding]
    public sealed class HearingVenuesSteps
    {
        private readonly TestContext _context;

        public HearingVenuesSteps(TestContext context)
        {
            _context = context;
        }

        [Given(@"I have a get all hearing venues available for booking request")]
        public void GivenIHaveAGetAllHearingVenuesAvailableForBookingRequest()
        {
            _context.Request = _context.Get(GetVenues);
        }

        [Then(@"hearing venues should be retrieved")]
        public void ThenHearingVenuesShouldBeRetrieved()
        {
            var model = RequestHelper.DeserialiseSnakeCaseJsonToResponse<List<HearingVenueResponse>>(_context.Response.Content);
            model.Should().NotBeNullOrEmpty();
        }
    }
}