using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.AcceptanceTests.Api.V1.HearingVenues;

public class GetHearingVenuesTests : ApiTest
{
    [Test]
    public async Task should_get_all_hearing_venues()
    {
        // arrange / act
        var allVenues = await BookingsApiClient.GetHearingVenuesAsync();

        // assert
        allVenues.Should().NotBeNullOrEmpty();
        allVenues.Should().AllSatisfy(hearingVenueResponse =>
        {
            hearingVenueResponse.Id.Should().NotBe(0);
            hearingVenueResponse.Name.Should().NotBeNullOrWhiteSpace();
        });
    }
}