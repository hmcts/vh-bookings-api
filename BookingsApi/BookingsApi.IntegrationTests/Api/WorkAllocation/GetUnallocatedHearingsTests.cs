using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Responses;
using BookingsApi.IntegrationTests.Helper;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Api;

namespace BookingsApi.IntegrationTests.Api.WorkAllocation;

public class GetUnallocatedHearingsTests : ApiTest
{
    [Test]
    public async Task should_return_unallocated_hearings_with_generic_case_type()
    {
        // arrange
        using var client = Application.CreateClient();
        var hearing = await TestDataManager.SeedVideoHearing();
        var uri = ApiUriFactory.WorkAllocationEndpoints.GetUnallocatedHearings;
        
        // act
        var result = await client.GetAsync(uri);

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await ApiClientResponse.GetResponses<List<HearingDetailsResponse>>(result.Content);
        response.Find(x => x.Id == hearing.Id).Should().NotBeNull();
        response.Should().Contain(x => x.Id == hearing.Id);
    }

    [TearDown]
    public async Task TearDown()
    {
        await TestDataManager.ClearSeededHearings();
    }
}