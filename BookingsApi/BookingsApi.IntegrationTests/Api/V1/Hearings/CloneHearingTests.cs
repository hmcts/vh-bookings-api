using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Validations.V1;
using Constants = BookingsApi.Contract.V1.Constants;

namespace BookingsApi.IntegrationTests.Api.V1.Hearings;

public class CloneHearingTests : ApiTest
{
    [Test]
    public async Task should_return_all_cloned_hearings_for_the_dates_with_unspecified_duration()
    {
        // arrange
        var startingDate = DateTime.UtcNow.AddMinutes(5);
        var hearing1 = await Hooks.SeedVideoHearing(isMultiDayFirstHearing:true, configureOptions: options =>
        {
            options.ScheduledDate = startingDate;
        });
        var groupId = hearing1.SourceId;

        var dates = new List<DateTime> {startingDate.AddDays(2), startingDate.AddDays(3)};

        // act
        using var client = Application.CreateClient();
        var request = new CloneHearingRequest { Dates = dates }; // No duration specified - should use the default
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.CloneHearing(hearing1.Id), RequestBody.Set(request));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var clonedHearingsList = await ApiClientResponse.GetResponses<List<HearingDetailsResponse>>(result.Content);
        clonedHearingsList.Count.Should().Be(dates.Count);
        var first = clonedHearingsList.Single(x => x.ScheduledDateTime == dates[0]);
        var second = clonedHearingsList.Single(x => x.ScheduledDateTime == dates[1]);
        first.Endpoints.Should().NotBeEquivalentTo(second.Endpoints);
        
        clonedHearingsList.TrueForAll(x => x.GroupId == groupId).Should().BeTrue();

        first.ScheduledDuration.Should().Be(Constants.CloneHearings.DefaultScheduledDuration);
        second.ScheduledDuration.Should().Be(Constants.CloneHearings.DefaultScheduledDuration);
    }

    [Test]
    public async Task should_return_all_cloned_hearings_for_the_dates_with_specified_duration()
    {
        // arrange
        var startingDate = DateTime.UtcNow.AddMinutes(5);
        var hearing1 = await Hooks.SeedVideoHearing(isMultiDayFirstHearing:true, configureOptions: options =>
        {
            options.ScheduledDate = startingDate;
        });
        var groupId = hearing1.SourceId;

        var dates = new List<DateTime> {startingDate.AddDays(2), startingDate.AddDays(3)};
        const int specifiedDuration = 120;

        // act
        using var client = Application.CreateClient();
        var request = new CloneHearingRequest
        {
            Dates = dates,
            ScheduledDuration = specifiedDuration // Duration specifeid
        };
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.CloneHearing(hearing1.Id), RequestBody.Set(request));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var clonedHearingsList = await ApiClientResponse.GetResponses<List<HearingDetailsResponse>>(result.Content);
        clonedHearingsList.Count.Should().Be(dates.Count);
        var first = clonedHearingsList.Single(x => x.ScheduledDateTime == dates[0]);
        var second = clonedHearingsList.Single(x => x.ScheduledDateTime == dates[1]);
        first.Endpoints.Should().NotBeEquivalentTo(second.Endpoints);
        
        clonedHearingsList.TrueForAll(x => x.GroupId == groupId).Should().BeTrue();

        first.ScheduledDuration.Should().Be(specifiedDuration);
        second.ScheduledDuration.Should().Be(specifiedDuration);
    }

    [Test]
    public async Task should_return_validation_error_when_validation_fails()
    {
        // arrange
        var startingDate = DateTime.UtcNow.AddMinutes(5);
        var hearing1 = await Hooks.SeedVideoHearing(isMultiDayFirstHearing:true, configureOptions: options =>
        {
            options.ScheduledDate = startingDate;
        });

        var dates = new List<DateTime> {startingDate.AddDays(2), startingDate.AddDays(3)};
        const int specifiedDuration = -1; // Invalid value

        // act
        using var client = Application.CreateClient();
        var request = new CloneHearingRequest
        {
            Dates = dates,
            ScheduledDuration = specifiedDuration
        };
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.CloneHearing(hearing1.Id), RequestBody.Set(request));
        
        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[nameof(request.ScheduledDuration)][0].Should()
            .Be(CloneHearingRequestValidation.InvalidScheduledDuration);
    }
}