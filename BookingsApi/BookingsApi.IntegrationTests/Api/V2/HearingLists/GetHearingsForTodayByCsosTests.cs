using BookingsApi.Contract.V2.Responses;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Domain.Enumerations;

namespace BookingsApi.IntegrationTests.Api.V2.HearingLists;

public class GetHearingsForTodayByCsosTests : ApiTest
{
    [Test]
    public async Task should_return_empty_list_when_no_filter_provided()
    {
        // arrange
        var request = new HearingsForTodayByAllocationRequestV2 {CsoIds = [], Unallocated = null};
        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingListsEndpoints.GetHearingsForTodayByCsos, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors["request"][0].Should().Be("Provide at least one filter type");
    }
    
    [Test]
    public async Task should_return_hearings_allocated_to_cso_ids()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(status:BookingStatus.Created, configureOptions: options =>
        {
            options.ScheduledDate = DateTime.UtcNow;
        });
        var hearingUnallocated = await Hooks.SeedVideoHearingV2(status:BookingStatus.Created, configureOptions: options =>
        {
            options.ScheduledDate = DateTime.UtcNow;
        });
        var justiceUser = await Hooks.SeedJusticeUser("user@test.com", "Test", "User");
        await Hooks.AddAllocation(hearing, justiceUser);
        var csoIds = new List<Guid> {justiceUser.Id};
        var request = new HearingsForTodayByAllocationRequestV2 {CsoIds = csoIds, Unallocated = null};
        
        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingListsEndpoints.GetHearingsForTodayByCsos, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        var hearings = await ApiClientResponse.GetResponses<List<HearingDetailsResponseV2>>(result.Content);
        hearings.Should().NotBeEmpty();
        hearings[0].Id.Should().Be(hearing.Id);
        hearings[0].AllocatedToId.Should().Be(justiceUser.Id);
        hearings[0].AllocatedToUsername.Should().Be(justiceUser.Username);
        hearings[0].AllocatedToName.Should().Be($"{justiceUser.FirstName} {justiceUser.Lastname}");
        hearings.Exists(x => x.Id == hearingUnallocated.Id).Should().BeFalse();
    }
    
    [Test]
    public async Task should_return_unallocated_hearings()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(status:BookingStatus.Created, configureOptions: options =>
        {
            options.ScheduledDate = DateTime.UtcNow;
        });
        var justiceUser = await Hooks.SeedJusticeUser("user@test.com", "Test", "User");
        await Hooks.AddAllocation(hearing, justiceUser);

        var hearingUnallocated = await Hooks.SeedVideoHearingV2(status:BookingStatus.Created, configureOptions: options =>
        {
            options.ScheduledDate = DateTime.UtcNow;
        });
        var request = new HearingsForTodayByAllocationRequestV2 {CsoIds = [], Unallocated = true};
        
        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingListsEndpoints.GetHearingsForTodayByCsos, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        var hearings = await ApiClientResponse.GetResponses<List<HearingDetailsResponseV2>>(result.Content);
        hearings.Should().NotBeEmpty();
        hearings.Exists(x => x.Id == hearing.Id).Should().BeFalse();
        hearings.Exists(x => x.Id == hearingUnallocated.Id).Should().BeTrue();
    }

    [Test]
    public async Task should_return_hearings_allocated_to_cso_and_unallocated_hearings()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(status: BookingStatus.Created,
            configureOptions: options => { options.ScheduledDate = DateTime.UtcNow; });
        var justiceUser = await Hooks.SeedJusticeUser("user@test.com", "Test", "User");
        await Hooks.AddAllocation(hearing, justiceUser);
        var csoIds = new List<Guid> {justiceUser.Id};

        var hearingUnallocated = await Hooks.SeedVideoHearingV2(status: BookingStatus.Created,
            configureOptions: options => { options.ScheduledDate = DateTime.UtcNow; });
        var request = new HearingsForTodayByAllocationRequestV2 { CsoIds = csoIds, Unallocated = true };

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingListsEndpoints.GetHearingsForTodayByCsos, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        var hearings = await ApiClientResponse.GetResponses<List<HearingDetailsResponseV2>>(result.Content);
        hearings.Should().NotBeEmpty();
        hearings.Exists(x => x.Id == hearing.Id).Should().BeTrue();
        hearings.Exists(x => x.Id == hearingUnallocated.Id).Should().BeTrue();
    }
}