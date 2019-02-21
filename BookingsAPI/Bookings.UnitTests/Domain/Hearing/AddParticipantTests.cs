using System;
using System.Linq;
using Bookings.Domain.Participants;
using Bookings.Domain.RefData;
using Bookings.Domain.Validations;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace Bookings.UnitTests.Domain.Hearing
{
    public class AddParticipantTests
    {
        [Test]
        public void should_add_new_participant_to_hearing()
        {
            var hearing = new VideoHearingBuilder().Build();
            var claimantCaseRole = new CaseRole(1, "Claimant");
            var claimantSolicitorHearingRole = new HearingRole(2, "Solicitor");

            var newPerson = new PersonBuilder(true).Build();
            var claimantSolicitorParticipant = Builder<Representative>.CreateNew().WithFactory(() =>
                new Representative(newPerson, claimantSolicitorHearingRole, claimantCaseRole)
            ).Build();

            var beforeAddCount = hearing.GetParticipants().Count;
            hearing.AddParticipant(claimantSolicitorParticipant);
            var afterAddCount = hearing.GetParticipants().Count;

            afterAddCount.Should().BeGreaterThan(beforeAddCount);
        }

        [Test]
        public void should_not_add_existing_participant_to_hearing()
        {
            var hearing = new VideoHearingBuilder().Build();
            var participant = hearing.GetParticipants().First();
            var beforeAddCount = hearing.GetParticipants().Count;

            Action action = () => hearing.AddParticipant(participant);
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Any(x => x.Message == "Participant already exists in the hearing").Should().BeTrue();
            
            var afterAddCount = hearing.GetParticipants().Count;
            afterAddCount.Should().Be(beforeAddCount);
        }
    }
}