using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.SpecialMeasure;

namespace BookingsApi.UnitTests.Domain.Screenings;

public class RemoveParticipantTests
{
    [Test]
    public void should_remove_participant_from_screening()
    {
        var participant = new ParticipantBuilder().IndividualParticipantApplicant;
        var screening = new Screening(ScreeningType.Specific, participant);
        var screenFrom = new ParticipantBuilder().IndividualParticipantRespondent;
        
        screening.UpdateScreeningList([screenFrom], []);
        screening.RemoveParticipant(screenFrom);
        
        screening.GetParticipants().Should().NotContain(x => x.Participant == screenFrom);
    }
}