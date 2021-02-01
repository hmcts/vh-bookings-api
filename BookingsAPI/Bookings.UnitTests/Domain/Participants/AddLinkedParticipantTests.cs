using System.Linq;
using Bookings.Domain.Enumerations;
using Bookings.Domain.Participants;
using Bookings.Domain.Validations;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace Bookings.UnitTests.Domain.Participants
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
        
        [Test]
        public void Should_Throw_Exception_When_Link_Already_Exists()
        {
            _individual.AddLink(_linkedIndividual.Id, LinkedParticipantType.Interpreter);

            _individual.Invoking(x => x.AddLink(_linkedIndividual.Id, LinkedParticipantType.Interpreter)).Should()
                .Throw<DomainRuleException>();
        }
    }
}