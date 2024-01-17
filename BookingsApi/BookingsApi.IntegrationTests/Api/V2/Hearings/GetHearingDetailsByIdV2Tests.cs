using BookingsApi.Contract.V2.Responses;
using BookingsApi.Mappings.Common;
using BookingsApi.Mappings.V2;

namespace BookingsApi.IntegrationTests.Api.V2.Hearings;

public class GetHearingDetailsByIdV2Tests : ApiTest
{
    [Test]
    public async Task should_return_bad_request_when_an_invalid_id_is_provided()
    {
        // arrange
        var hearingId = "notaguid";
        

        // act
        using var client = Application.CreateClient();
        var result = await client.GetAsync(ApiUriFactory.HearingsEndpointsV2.GetHearingDetailsById(hearingId));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors["hearingId"][0].Should()
            .Be($"The value '{hearingId}' is not valid.");
    }

    [Test]
    public async Task should_return_not_found_when_a_hearing_is_not_found_with_the_provided_id()
    {
        // arrange
        var hearingId = Guid.Empty.ToString();
        

        // act
        using var client = Application.CreateClient();
        var result = await client.GetAsync(ApiUriFactory.HearingsEndpointsV2.GetHearingDetailsById(hearingId));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task should_return_a_hearing_when_matched_with_a_given_id()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(configureOptions: options =>
        {
            options.AddPanelMember = true;
            options.EndpointsToAdd = 1;
        });
        var hearingId = hearing.Id;

        // act
        using var client = Application.CreateClient();
        var result = await client.GetAsync(ApiUriFactory.HearingsEndpointsV2.GetHearingDetailsById(hearingId.ToString()));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(result.Content);
        hearingResponse.Should().NotBeNull();
        hearingResponse.Id.Should().Be(hearingId);
        hearingResponse.HearingVenueCode.Should().Be(hearing.HearingVenue.VenueCode);
        hearingResponse.HearingVenueName.Should().Be(hearing.HearingVenue.Name);
        hearingResponse.IsHearingVenueScottish.Should().Be(hearing.HearingVenue.IsScottish);
        
        hearingResponse.ServiceId.Should().Be(hearing.CaseType.ServiceId);
        hearingResponse.ServiceName.Should().Be(hearing.CaseType.Name);
        
        hearingResponse.ScheduledDateTime.Should().Be(hearing.ScheduledDateTime.ToUniversalTime());
        hearingResponse.ScheduledDuration.Should().BePositive();
        hearingResponse.HearingRoomName.Should().NotBeNullOrEmpty();
        hearingResponse.OtherInformation.Should().NotBeNullOrEmpty();
        hearingResponse.CreatedBy.Should().NotBeNullOrEmpty();
        hearingResponse.AudioRecordingRequired.Should().BeTrue();
        hearingResponse.MultiDayHearingEndTime.Should().BeNull();

        foreach (var @case in hearing.GetCases())
        {
            hearingResponse.Cases.Should().ContainEquivalentOf(new CaseResponseV2
            {
                IsLeadCase = @case.IsLeadCase,
                Name = @case.Name,
                Number = @case.Number
            });
        }


        foreach (var participant in hearing.GetParticipants())
        {
            hearingResponse.Participants.Should()
                .ContainEquivalentOf(new ParticipantToResponseV2Mapper().MapParticipantToResponse(participant));
        }

        foreach (var endpoint in hearing.GetEndpoints())
        {
            hearingResponse.Endpoints.Should().ContainEquivalentOf(new EndpointResponseV2
            {
                DisplayName = endpoint.DisplayName,
                DefenceAdvocateId = endpoint.DefenceAdvocate?.Id,
                Id = endpoint.Id,
                Pin = endpoint.Pin,
                Sip = endpoint.Sip,
            });
        }

        foreach (var judiciaryParticipant in hearing.GetJudiciaryParticipants())
        {
            hearingResponse.JudiciaryParticipants.Should()
                .ContainEquivalentOf(new JudiciaryParticipantToResponseMapper().MapJudiciaryParticipantToResponse(judiciaryParticipant));
        }
    }

    [Test]
    public async Task should_return_a_multi_day_hearing()
    {
        // arrange
        var dates = new List<DateTime>
        {
            DateTime.Today.AddDays(1).AddHours(10).AddMinutes(0),
            DateTime.Today.AddDays(2).AddHours(10).AddMinutes(0),
            DateTime.Today.AddDays(3).AddHours(10).AddMinutes(0)
        };
        const int scheduledDuration = 45;
        var expectedMultiDayHearingEndTime = dates.Max().AddMinutes(scheduledDuration);

        var multiDayHearings = await Hooks.SeedMultiDayHearing(dates, scheduledDuration: scheduledDuration);
        var firstDayHearing = multiDayHearings[0];

        // act
        using var client = Application.CreateClient();
        var result = await client.GetAsync(ApiUriFactory.HearingsEndpointsV2.GetHearingDetailsById(firstDayHearing.Id.ToString()));
        
        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(result.Content);
        hearingResponse.Should().NotBeNull();
        hearingResponse.Id.Should().Be(firstDayHearing.Id);
        hearingResponse.MultiDayHearingEndTime.Should().Be(expectedMultiDayHearingEndTime);
    }
}