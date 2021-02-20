using System.Linq;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.Validations;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace BookingsApi.UnitTests.Domain.Participants
{
    public class LinkedParticipantTests
    {
        private Participant _individual;
        private Participant _linkedIndividual;

        [SetUp]
        public void SetUp()
        {
            _individual = new ParticipantBuilder().IndividualParticipantClaimant;
            _linkedIndividual = new ParticipantBuilder().IndividualParticipantDefendant;
        }
        
        [Test]
        public void Should_Add_Link()
        {
            _individual.AddLink(_linkedIndividual.Id, LinkedParticipantType.Interpreter);
            var linkedId = _individual.LinkedParticipants.Select(x => x.LinkedId);

            linkedId.Should().BeEquivalentTo(_linkedIndividual.Id);
        }
    }
}