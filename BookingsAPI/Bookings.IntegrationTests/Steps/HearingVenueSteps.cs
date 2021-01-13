using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using Bookings.Api.Contract.Responses;
using Bookings.IntegrationTests.Contexts;
using FluentAssertions;
using TechTalk.SpecFlow;
using static Testing.Common.Builders.Api.ApiUriFactory.HearingVenueEndpoints;

namespace Bookings.IntegrationTests.Steps
{
    [Binding]
    public sealed class HearingVenueBaseSteps : BaseSteps
    {
        public HearingVenueBaseSteps(TestContext context) : base(context)
        {
        }

        [Given(@"I have a get all hearing venues available for booking request")]
        public void GivenIHaveAGetAllHearingVenuesAvailableForBookingRequest()
        {           
            Context.Uri = GetVenues;
            Context.HttpMethod = HttpMethod.Get;
        }

        [Then(@"hearing venues should be retrieved")]
        public async Task ThenHearingVenuesShouldBeRetrieved()
        {
            var json = await Context.Response.Content.ReadAsStringAsync();
            var model = RequestHelper.Deserialise<List<HearingVenueResponse>>(json);
            foreach (var venue in model)
            {
                venue.Id.Should().BePositive();
                venue.Name.Should().NotBeNullOrEmpty();
            }            
        }
    }
}
