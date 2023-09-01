using BookingsApi.Domain.Participants;
using BookingsApi.Domain.Validations;

namespace BookingsApi.UnitTests.Domain.Participants
{
    public class UpdateRepresentativeDetailsTests
    {
        [Test]
        public void Should_update_participant_with_user_role_representative_details()
        {
            var participant = new ParticipantBuilder().RepresentativeParticipantRespondent;
            var representative = (Representative)participant;
            string representee = "Representee Edit";
           
            representative.UpdateRepresentativeDetails(representee);
            representative.Representee.Should().Be(representee);
                      
        }

        [Test]
        public void Should_throw_exception_when_validation_fails()
        {
            var participant = new ParticipantBuilder().RepresentativeParticipantRespondent;
            var representativeParticipant = (Representative)participant;
            string representee = "";
            var beforeUpdatedDate = representativeParticipant.UpdatedDate;

            Action action = () => representativeParticipant.UpdateRepresentativeDetails(representee);
            action.Should().Throw<DomainRuleException>()
                .And.ValidationFailures.Should()
                .Contain(x => x.Name == "Representee");

            representativeParticipant.UpdatedDate.Should().Be(beforeUpdatedDate);
        }
    }
}
