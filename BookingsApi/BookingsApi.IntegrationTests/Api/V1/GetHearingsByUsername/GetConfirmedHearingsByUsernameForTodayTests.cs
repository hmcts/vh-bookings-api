using BookingsApi.Contract.V1.Responses;
using BookingsApi.Domain.Enumerations;

namespace BookingsApi.IntegrationTests.Api.V1.GetHearingsByUsername;

public class GetConfirmedHearingsByUsernameForTodayTests : ApiTest
{
    [Test]
    public async Task should_get_all_confirmed_hearings_for_today()
    {
        var hearing1 = await Hooks.SeedVideoHearing(status: BookingStatus.Created,
                    configureOptions: options => { options.ScheduledDate = DateTime.UtcNow; });
        await Hooks.CloneVideoHearing(hearing1.Id, new List<DateTime> { DateTime.UtcNow }, BookingStatus.Created);
        await Hooks.CloneVideoHearing(hearing1.Id, new List<DateTime> { DateTime.UtcNow }, BookingStatus.Created);
        await Hooks.CloneVideoHearing(hearing1.Id, new List<DateTime> { DateTime.UtcNow.AddDays(1) }, BookingStatus.Created);
        var username = hearing1.Participants[0].Person.Username;

        using var client = Application.CreateClient();

        var result = await client.GetAsync(ApiUriFactory.HearingsEndpoints.GetConfirmedHearingsByUsernameForToday(username));

        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var hearingVenueResponses = await ApiClientResponse.GetResponses<List<ConfirmedHearingsTodayResponse>>(result.Content);
        hearingVenueResponses.TrueForAll(x => x.ScheduledDateTime.Date == DateTime.UtcNow.Date).Should().BeTrue();
    }

    [Test]
    public async Task should_return_notfound_when_no_confirmed_hearings_for_today()
    {
        var hearing1 = await Hooks.SeedVideoHearing(status: BookingStatus.Created,
                    configureOptions: options => { options.ScheduledDate = DateTime.UtcNow.AddDays(1); });
        var username = hearing1.Participants[0].Person.Username;

        using var client = Application.CreateClient();

        var result = await client.GetAsync(ApiUriFactory.HearingsEndpoints.GetConfirmedHearingsByUsernameForToday(username));

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var response = await ApiClientResponse.GetResponses<string>(result.Content);
        response.Should().Be($"{username.Trim().ToLower()} does not have any confirmed hearings today");
    }
}