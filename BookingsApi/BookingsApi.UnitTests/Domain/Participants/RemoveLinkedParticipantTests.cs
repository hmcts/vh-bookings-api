using System;
using System.Linq;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.Validations;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace BookingsApi.UnitTests.Domain.Participants
{
    public class RemoveLinkedParticipantTests
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
        public void Should_Remove_Link()
        {
            _individual.AddLink(_linkedIndividual.Id, LinkedParticipantType.Interpreter);
            var linkedId = _individual.LinkedParticipants.Select(x => x.LinkedId).FirstOrDefault();
            linkedId.Should().Be(_linkedIndividual.Id);

            var linkedParticipant = _individual.LinkedParticipants.FirstOrDefault(x => x.LinkedId == linkedId);
            _individual.RemoveLink(linkedParticipant);
            
            var removedLinkedId = _individual.LinkedParticipants.Select(x => x.LinkedId);
            removedLinkedId.Should().BeNullOrEmpty();
        }
        
        [Test]
        public void Should_Throw_Exception_When_Link_Does_Not_Exists()
        {
            var linkedParticipant = new LinkedParticipant(_linkedIndividual.Id, Guid.NewGuid(), LinkedParticipantType.Interpreter);
            
            _individual.Invoking(x => x.RemoveLink(linkedParticipant)).Should()
                .Throw<DomainRuleException>();
        }
    }
}