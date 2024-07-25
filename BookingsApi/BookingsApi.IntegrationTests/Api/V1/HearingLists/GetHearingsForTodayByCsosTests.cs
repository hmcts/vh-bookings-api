using BookingsApi.Contract.V1.Responses;
using BookingsApi.Domain.Enumerations;

namespace BookingsApi.IntegrationTests.Api.V1.HearingLists;

public class GetHearingsForTodayByCsosTests : ApiTest
{
    [Test]
    public async Task should_return_empty_list_when_no_cso_ids_provided()
    {
        // arrange
        var csoIds = new List<Guid>();
        
        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingListsEndpoints.GetHearingsForTodayByCsos, RequestBody.Set(csoIds));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        var hearings = await ApiClientResponse.GetResponses<List<HearingDetailsResponse>>(result.Content);
        hearings.Should().BeEmpty();
    }
    
    [Test]
    public async Task should_return_hearings_allocated_to_cso_ids()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearing(status:BookingStatus.Created, configureOptions: options =>
        {
            options.ScheduledDate = DateTime.UtcNow;
        });
        var justiceUser = await Hooks.SeedJusticeUser("user@test.com", "Test", "User");
        await Hooks.AddAllocation(hearing, justiceUser);
        var csoIds = new List<Guid> {justiceUser.Id};
        
        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingListsEndpoints.GetHearingsForTodayByCsos, RequestBody.Set(csoIds));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        var hearings = await ApiClientResponse.GetResponses<List<HearingDetailsResponse>>(result.Content);
        hearings.Should().NotBeEmpty();
        hearings[0].Id.Should().Be(hearing.Id);
        hearings[0].AllocatedToId.Should().Be(justiceUser.Id);
        hearings[0].AllocatedToUsername.Should().Be(justiceUser.Username);
    }
}