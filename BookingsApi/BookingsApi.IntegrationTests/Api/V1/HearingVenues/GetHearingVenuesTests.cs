using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.IntegrationTests.Helper;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Api;

namespace BookingsApi.IntegrationTests.Api.V1.HearingVenues;

public class GetHearingVenuesTests : ApiTest
{
    [Test]
    public async Task should_get_all_hearing_venues()
    {
        // arrange
        using var client = Application.CreateClient();

        // act
        var result = await client.GetAsync(ApiUriFactory.HearingVenueEndpoints.GetVenues);

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var hearingVenueResponses = await ApiClientResponse.GetResponses<List<HearingVenueResponse>>(result.Content);
        hearingVenueResponses.Should().NotBeEmpty();
        hearingVenueResponses.Exists(x => x.Name == "Birmingham Civil and Family Justice Centre").Should().BeTrue();
    }
}