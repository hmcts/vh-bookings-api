using BookingsApi.Contract.V2.Responses;
using BookingsApi.Domain.Enumerations;

namespace BookingsApi.IntegrationTests.Api.V2.Hearings;

public class GetConfirmedHearingsByUsernameForTodayTestsV2 : ApiTest
{
    [Test]
    public async Task should_get_all_confirmed_hearings_for_today()
    {
        var hearing1 = await Hooks.SeedVideoHearingV2(status: BookingStatus.Created,
                    configureOptions: options => { options.ScheduledDate = DateTime.UtcNow; });
        await Hooks.CloneVideoHearing(hearing1.Id, new List<DateTime> { DateTime.UtcNow }, BookingStatus.Created);
        await Hooks.CloneVideoHearing(hearing1.Id, new List<DateTime> { DateTime.UtcNow }, BookingStatus.Created);
        await Hooks.CloneVideoHearing(hearing1.Id, new List<DateTime> { DateTime.UtcNow.AddDays(1) }, BookingStatus.Created);
        var username = hearing1.Participants[0].Person.Username;

        using var client = Application.CreateClient();

        var result = await client.GetAsync(ApiUriFactory.HearingsEndpointsV2.GetConfirmedHearingsByUsernameForToday(username));

        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var hearingVenueResponses = await ApiClientResponse.GetResponses<List<ConfirmedHearingsTodayResponseV2>>(result.Content);
        hearingVenueResponses.TrueForAll(x => x.ScheduledDateTime.Date == DateTime.UtcNow.Date).Should().BeTrue();
    }

    [Test]
    public async Task should_return_empty_list_when_no_confirmed_hearings_for_today()
    {
        var hearing1 = await Hooks.SeedVideoHearingV2(status: BookingStatus.Created,
                    configureOptions: options => { options.ScheduledDate = DateTime.UtcNow.AddDays(1); });
        var username = hearing1.Participants[0].Person.Username;

        using var client = Application.CreateClient();

        var result = await client.GetAsync(ApiUriFactory.HearingsEndpointsV2.GetConfirmedHearingsByUsernameForToday(username));

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await ApiClientResponse.GetResponses<List<ConfirmedHearingsTodayResponseV2>>(result.Content);
        response.Should().BeEmpty();
    }
}