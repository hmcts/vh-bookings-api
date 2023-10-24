using BookingsApi.Contract.V1.Responses;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Mappings.V1;

namespace BookingsApi.IntegrationTests.Api.V1.Hearings;

public class GetHearingDetailsByIdTests : ApiTest
{
    [Test]
    public async Task should_return_bad_request_when_an_invalid_id_is_provided()
    {
        // arrange
        var hearingId = "notaguid";
        

        // act
        using var client = Application.CreateClient();
        var result = await client.GetAsync(ApiUriFactory.HearingsEndpoints.GetHearingDetailsById(hearingId));

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
        var result = await client.GetAsync(ApiUriFactory.HearingsEndpoints.GetHearingDetailsById(hearingId));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task should_return_a_hearing_when_matched_with_a_given_id()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearing(options =>
        {
            options.EndpointsToAdd = 3;
        });
        var hearingId = hearing.Id;

        // act
        using var client = Application.CreateClient();
        var result = await client.GetAsync(ApiUriFactory.HearingsEndpoints.GetHearingDetailsById(hearingId.ToString()));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponse>(result.Content);
        hearingResponse.Should().NotBeNull();
        hearingResponse.Id.Should().Be(hearingId);
        hearingResponse.HearingVenueName.Should().Be(hearing.HearingVenue.Name);
        hearingResponse.HearingTypeName.Should().Be(hearing.HearingType.Name);
        hearingResponse.CaseTypeName.Should().Be(hearing.CaseType.Name);
        
        hearingResponse.ScheduledDateTime.Should().Be(hearing.ScheduledDateTime.ToUniversalTime());
        hearingResponse.ScheduledDuration.Should().BePositive();
        hearingResponse.HearingRoomName.Should().NotBeNullOrEmpty();
        hearingResponse.OtherInformation.Should().NotBeNullOrEmpty();
        hearingResponse.CreatedBy.Should().NotBeNullOrEmpty();
        hearingResponse.AudioRecordingRequired.Should().BeTrue();

        foreach (var @case in hearing.GetCases())
        {
            hearingResponse.Cases.Should().ContainEquivalentOf(new CaseResponse
                
            {
                IsLeadCase = @case.IsLeadCase,
                Name = @case.Name,
                Number = @case.Number
            });
        }


        foreach (var participant in hearing.GetParticipants())
        {
            hearingResponse.Participants.Should()
                .ContainEquivalentOf(new ParticipantToResponseMapper().MapParticipantToResponse(participant));
        }

        foreach (var endpoint in hearing.GetEndpoints())
        {
            hearingResponse.Endpoints.Should().ContainEquivalentOf(new EndpointResponse
            {
                DisplayName = endpoint.DisplayName,
                DefenceAdvocateId = endpoint.DefenceAdvocate?.Id,
                Id = endpoint.Id,
                Pin = endpoint.Pin,
                Sip = endpoint.Sip,
            });
        }
    }
}