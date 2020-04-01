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
        public void Should_remove_existing_participant_from_hearing()
        {
            var hearing = new VideoHearingBuilder().Build();
            var beforeCount = hearing.GetParticipants().Count;
            var participant = hearing.GetParticipants().First();

            hearing.RemoveParticipant(participant);
            var afterCount =hearing.GetParticipants().Count;
            afterCount.Should().BeLessThan(beforeCount);
        }
        
        [Test]
        public void Should_not_remove_participant_that_does_not_exist_in_hearing()
        {
            var hearing = new VideoHearingBuilder().Build();
            
            var claimantCaseRole = new CaseRole(1, "Claimant");
            var claimantRepresentativeHearingRole = new HearingRole(2, "Representative");
            var newPerson = new PersonBuilder(true).Build();
            var participant = Builder<Representative>.CreateNew().WithFactory(() =>
                new Representative(newPerson, claimantRepresentativeHearingRole, claimantCaseRole)
            ).Build();
            
            Action action = () => hearing.RemoveParticipant(participant);
            action.Should().Throw<DomainRuleException>().And.ValidationFailures.Any(x =>
                x.Name == "Participant" && x.Message == "Participant does not exist on the hearing").Should().BeTrue();
        }

        [Test]
        public void Should_not_remove_participant_when_count_is_below_two()
        {
            var hearing = new VideoHearingBuilder().Build();
            var currentCount = hearing.GetParticipants().Count;
            while (currentCount > 1)
            {
                var part = hearing.GetParticipants().First();
                hearing.RemoveParticipant(part);
                currentCount = hearing.GetParticipants().Count;
            }

            var participant = hearing.GetParticipants().First();
            Action action = () => hearing.RemoveParticipant(participant);
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Any(x => x.Message == "A hearing must have at least one participant").Should().BeTrue();
        }
    }
}