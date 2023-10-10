using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.Validations;

namespace BookingsApi.IntegrationTests.Api.V1.HearingParticipants;

public class RemoveParticipantFromHearingTests : ApiTest
{
    [Test]
    public async Task should_not_remove_participant_when_hearing_is_about_to_start()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearing(options =>
            {
                options.Case = new Case("Case1 Num", "Case1 Name"); 
                options.ScheduledDate = DateTime.UtcNow.AddMinutes(25);
            },
            BookingStatus.Created);

        var participant = hearing.GetParticipants().First(x => x is Individual);
        
        // act
        using var client = Application.CreateClient();
        var result = await client
            .DeleteAsync(ApiUriFactory.ParticipantsEndpoints.RemoveParticipantFromHearing(hearing.Id, participant.Id));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors.SelectMany(x => x.Value).Should()
            .Contain(DomainRuleErrorMessages.CannotRemoveParticipantCloseToStartTime);
    }
}