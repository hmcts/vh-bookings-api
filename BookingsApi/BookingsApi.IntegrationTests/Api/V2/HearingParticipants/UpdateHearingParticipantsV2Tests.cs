using System.Linq;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
namespace BookingsApi.IntegrationTests.Api.V2.HearingParticipants;

public class UpdateHearingParticipantsV2Tests : ApiTest
{
    [Test]
    public async Task should_update_individual_participant_in_a_hearing_and_return_200()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearing(options
            => { options.Case = new Case("Case1 Num", "Case1 Name"); },false, BookingStatus.Created);
        
        var participantToUpdate = hearing.Participants.First(e => e.HearingRole.UserRole.IsIndividual);
        var request = new UpdateHearingParticipantsRequestV2
        {
            ExistingParticipants = new List<UpdateParticipantRequestV2>()
            {
                new ()
                {
                    ParticipantId = participantToUpdate.Id,
                    DisplayName = participantToUpdate.DisplayName,
                    Title = participantToUpdate.Person.Title
                }
            } 
        };
        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateHearingParticipants(hearing.Id),RequestBody.Set(request));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
    }

}