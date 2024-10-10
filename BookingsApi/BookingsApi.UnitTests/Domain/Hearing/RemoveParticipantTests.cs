using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using BookingsApi.Domain.Validations;
using FizzWare.NBuilder;

namespace BookingsApi.UnitTests.Domain.Hearing
{
    public class RemoveParticipantTests
    {
        [Test]
        public void Should_remove_existing_participant_from_hearing()
        {
            var hearing = new VideoHearingBuilder().Build();
            var beforeCount = hearing.GetParticipants().Count;
            var participant = hearing.GetParticipants()[0];

            hearing.RemoveParticipant(participant);
            var afterCount =hearing.GetParticipants().Count;
            afterCount.Should().BeLessThan(beforeCount);
        }
        
        [Test]
        public void Should_not_remove_participant_that_does_not_exist_in_hearing()
        {
            var hearing = new VideoHearingBuilder().Build();
            
            var applicantRepresentativeHearingRole = new HearingRole(2, "Representative");
            var newPerson = new PersonBuilder(true).Build();
            var participant = Builder<Representative>.CreateNew().WithFactory(() =>
                new Representative(Guid.NewGuid().ToString(), newPerson, applicantRepresentativeHearingRole, "Applicant1", "Representee1")
            ).Build();
            
            Action action = () => hearing.RemoveParticipant(participant);
            action.Should().Throw<DomainRuleException>().And.ValidationFailures.Exists(x =>
                x.Name == "Participant" && x.Message == "Participant does not exist on the hearing").Should().BeTrue();
        }
    }
}