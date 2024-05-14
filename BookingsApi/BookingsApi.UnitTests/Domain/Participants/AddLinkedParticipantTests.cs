using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;

namespace BookingsApi.UnitTests.Domain.Participants
{
    public class LinkedParticipantTests
    {
        private Participant _individual;
        private Participant _linkedIndividual;

        [SetUp]
        public void SetUp()
        {
            _individual = new ParticipantBuilder().IndividualParticipantApplicant;
            _linkedIndividual = new ParticipantBuilder().IndividualParticipantRespondent;
        }
        
        [Test]
        public void Should_Add_Link()
        {
            _individual.AddLink(_linkedIndividual.Id, LinkedParticipantType.Interpreter);
            var linkedId = _individual.LinkedParticipants.Select(x => x.LinkedId).ToList();

            linkedId[0].Should().Be(_linkedIndividual.Id);
        }
    }
}