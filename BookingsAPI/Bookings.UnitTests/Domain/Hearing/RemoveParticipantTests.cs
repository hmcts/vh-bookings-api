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
    public class RemoveParticipantTests
    {
        [Test]
        public void should_remove_existing_participant_from_hearing()
        {
            var hearing = new VideoHearingBuilder().Build();
            var participant = hearing.GetParticipants().First();

            hearing.RemoveParticipant(participant);
        }
        
        [Test]
        public void should_not_remove_participant_that_does_not_exist_in_hearing()
        {
            var hearing = new VideoHearingBuilder().Build();
            
            var claimantCaseRole = new CaseRole(1, "Claimant");
            var claimantSolicitorHearingRole = new HearingRole(2, "Solicitor");
            var newPerson = new PersonBuilder(true).Build();
            var participant = Builder<Representative>.CreateNew().WithFactory(() =>
                new Representative(newPerson, claimantSolicitorHearingRole, claimantCaseRole)
            ).Build();
            
            Action action = () => hearing.RemoveParticipant(participant);
            action.Should().Throw<DomainRuleException>().And.ValidationFailures.Any(x =>
                x.Name == "Participant" && x.Message == "Participant does not exist on the hearing").Should().BeTrue();
        }

        [Test]
        public void should_not_remove_participant_when_count_is_below_two()
        {
            var hearing = new VideoHearingBuilder().Build();
            var currentCount = hearing.GetParticipants().Count;
            while (currentCount > 2)
            {
                var part = hearing.GetParticipants().First();
                hearing.RemoveParticipant(part);
                currentCount = hearing.GetParticipants().Count;
            }

            var participant = hearing.GetParticipants().First();
            hearing.RemoveParticipant(participant);
            Action action = () => hearing.RemoveParticipant(participant);
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Any(x => x.Message == "A hearing must have at least one participant").Should().BeTrue();
        }
    }
}