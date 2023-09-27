using BookingsApi.Contract.V1.Enums;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Validations.V1;

namespace BookingsApi.IntegrationTests.Api.V1.HearingParticipants;

public class UpdateHearingParticipantsTests : ApiTest
{
        
    [Test]
    public async Task should_remove_a_judge_from_the_confirmed_hearing()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearing(options
            => { options.Case = new Case("UpdateParticipantsRemoveParticipant", "UpdateParticipantsRemoveParticipant"); }, Domain.Enumerations.BookingStatus.Created);
        var judge = hearing.Participants.First(e => e.HearingRole.IsJudge());
        var request = new UpdateHearingParticipantsRequest { RemovedParticipantIds = new List<Guid>{ judge.Id } };

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpoints.UpdateHearingParticipants(hearing.Id),RequestBody.Set(request));
        var updatedHearing = await client.GetAsync(ApiUriFactory.HearingsEndpoints.GetHearingDetailsById(hearing.Id.ToString()));
        var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponse>(updatedHearing.Content);
        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        hearingResponse.Participants.Should().NotContain(p => p.Id == judge.Id);
        hearingResponse.Status.Should().Be(BookingStatus.ConfirmedWithoutJudge);
    }
}