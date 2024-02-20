using BookingsApi.Contract.V1.Enums;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Validations.V1;
using Testing.Common.Builders.Domain;

namespace BookingsApi.IntegrationTests.Api.V1.HearingParticipants;

public class UpdateHearingParticipantsTests : ApiTest
{
    [TestCase("UserName")]
    [TestCase("")]
    [TestCase(null)]
    public async Task should_change_a_judge_on_a_confirmed_hearing(string updatedBy)
    {
        // arrange
        var genericJudge = await Hooks.SeedGenericJudgePerson();
        var hearing = await Hooks.SeedVideoHearing(options
            => { options.Case = new Case("UpdateParticipantJudge", "UpdateParticipantJudge"); }, Domain.Enumerations.BookingStatus.Created);
        var judge = hearing.Participants.First(e => e.HearingRole.IsJudge());
        var request = new UpdateHearingParticipantsRequest
        {
            RemovedParticipantIds = new List<Guid>{ judge.Id },
            NewParticipants = new List<ParticipantRequest>
            {
                new()
                {
                    ContactEmail = genericJudge.ContactEmail,
                    DisplayName = "Judge",  
                    Username = genericJudge.Username,
                    HearingRoleName = "Judge",
                    CaseRoleName = "Judge",
                    FirstName = genericJudge.FirstName,
                    LastName = genericJudge.LastName,
                }
            },
            UpdatedBy = updatedBy
        };

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpoints.UpdateHearingParticipants(hearing.Id),RequestBody.Set(request));
        var updatedHearing = await client.GetAsync(ApiUriFactory.HearingsEndpoints.GetHearingDetailsById(hearing.Id.ToString()));
        var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponse>(updatedHearing.Content);
        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        hearingResponse.Participants.Should().NotContain(p => p.Id == judge.Id);
        hearingResponse.Participants.Should().Contain(p => p.Username == genericJudge.Username);
        hearingResponse.Status.Should().Be(BookingStatus.Created);
        hearingResponse.UpdatedBy.Should().Be(string.IsNullOrEmpty(request.UpdatedBy) ? "System" : request.UpdatedBy);
    }
    
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

    [Test]
    public async Task should_return_bad_request_when_hearing_id_is_invalid()
    {
        // arrange
        var hearingId = Guid.Empty;
        var request = new UpdateHearingParticipantsRequest { RemovedParticipantIds = new List<Guid>()};
        
        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpoints.UpdateHearingParticipants(hearingId),RequestBody.Set(request));
        
        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors.SelectMany(x => x.Value).Should()
            .Contain($"Please provide a valid {nameof(hearingId)}");
    }

    [Test]
    public async Task should_return_bad_request_when_request_is_invalid()
    {
        // arrange
        var hearingId = Guid.NewGuid();
        var request = new UpdateHearingParticipantsRequest { RemovedParticipantIds = new List<Guid>()};
        
        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpoints.UpdateHearingParticipants(hearingId),RequestBody.Set(request));
        
        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors.SelectMany(x => x.Value).Should()
            .Contain(UpdateHearingParticipantsRequestValidation.NoParticipantsErrorMessage);
    }
}