using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Domain.Enumerations;
using BookingsApi.IntegrationTests.Helper;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Api;

namespace BookingsApi.IntegrationTests.Api.V1.GetHearingsByUsername;

public class GetConfirmedHearingsByUsernameForTodayTests : ApiTest
{
    [Test]
    public async Task should_get_all_confirmed_hearings_for_today()
    {
        var hearing1 = await Hooks.SeedVideoHearing(status: BookingStatus.Created,
                    configureOptions: options => { options.ScheduledDate = System.DateTime.UtcNow; });
        await Hooks.CloneVideoHearing(hearing1.Id, new List<System.DateTime> { System.DateTime.UtcNow }, BookingStatus.Created);
        await Hooks.CloneVideoHearing(hearing1.Id, new List<System.DateTime> { System.DateTime.UtcNow }, BookingStatus.Created);
        await Hooks.CloneVideoHearing(hearing1.Id, new List<System.DateTime> { System.DateTime.UtcNow.AddDays(1) }, BookingStatus.Created);
        var username = hearing1.Participants.First().Person.Username;

        using var client = Application.CreateClient();

        var result = await client.GetAsync(ApiUriFactory.HearingsEndpoints.GetConfirmedHearingsByUsernameForToday(username));

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var hearingVenueResponses = await ApiClientResponse.GetResponses<List<HearingDetailsResponse>>(result.Content);
        hearingVenueResponses.All(x => x.ScheduledDateTime.Date == System.DateTime.UtcNow.Date).Should().BeTrue();
    }

    [Test]
    public async Task should_return_notfound_when_no_confirmed_hearings_for_today()
    {
        var hearing1 = await Hooks.SeedVideoHearing(status: BookingStatus.Created,
                    configureOptions: options => { options.ScheduledDate = System.DateTime.UtcNow.AddDays(1); });
        var username = hearing1.Participants.First().Person.Username;

        using var client = Application.CreateClient();

        var result = await client.GetAsync(ApiUriFactory.HearingsEndpoints.GetConfirmedHearingsByUsernameForToday(username));

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}