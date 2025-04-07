using BookingsApi.Domain;

namespace BookingsApi.UnitTests.Domain.Hearing
{
    public class AssignDefenceAdvocateTests
    {
        [Test]
        public void should_assign_defence_advocate()
        {
            var ep = new Endpoint(Guid.NewGuid().ToString(),"DisplayName", "sip@address.com", "1111");
            var dA = new ParticipantBuilder().RepresentativeParticipantRespondent;
            ep.SetProtected(nameof(ep.UpdatedDate), DateTime.UtcNow.AddMinutes(-1));
            var originalUpdatedDate = ep.UpdatedDate;
            
            ep.AddLinkedParticipant(dA);
            var defenceAdvocate = ep.ParticipantsLinked[0];
            defenceAdvocate.Should().Be(dA);
            ep.UpdatedDate.Should().BeAfter(originalUpdatedDate);
        }

        [Test]
        public void should_not_assign_defence_advocate_when_not_changed()
        {
            var dA = new ParticipantBuilder().RepresentativeParticipantRespondent;
            var ep = new Endpoint(Guid.NewGuid().ToString(),"DisplayName", "sip@address.com", "1111");
            ep.AddLinkedParticipant(dA);
            ep.SetProtected(nameof(ep.UpdatedDate), DateTime.UtcNow.AddMinutes(-1));
            var originalUpdatedDate = ep.UpdatedDate;
            
            ep.AddLinkedParticipant(dA);
            var defenceAdvocate = ep.ParticipantsLinked[0];
            defenceAdvocate.Should().Be(dA);
            ep.UpdatedDate.Should().Be(originalUpdatedDate);
        }
    }
}