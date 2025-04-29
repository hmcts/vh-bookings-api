namespace BookingsApi.IntegrationTests.Api.V1.Hearings;

public class AnonymiseParticipantAndCaseByHearingIdTests : ApiTest
{
    [Test]
    public async Task should_return_204_and_anonymise_participant_and_case()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2();
        var caseNames = hearing.GetCases().Select(x => x.Name).ToList();
        var displayNames = hearing.Participants.Select(x => x.DisplayName).ToList();
        // act
        var client = GetBookingsApiClient();
        await client.AnonymiseParticipantAndCaseByHearingIdAsync([hearing.Id]);

        // assert
        var response = await client.GetHearingDetailsByIdV2Async(hearing.Id);
        
        response.Should().NotBeNull();
        // none of the case names should be the same
        response.Cases.Should().NotContain(x => caseNames.Contains(x.Name));
        // none of the participants should have the same display name
        response.Participants.Should().NotContain(x => displayNames.Contains(x.DisplayName));
        
    }
}