using BookingsApi.Domain;

namespace BookingsApi.UnitTests.Domain.Hearing
{
    public class AssignDefenceAdvocateTests
    {
        [Test]
        public void should_assign_defence_advocate()
        {
            var ep = new Endpoint(Guid.NewGuid().ToString(),"DisplayName", "sip@address.com", "1111", null);
            var dA = new ParticipantBuilder().RepresentativeParticipantRespondent;
            
            ep.AssignDefenceAdvocate(dA);
            
            ep.DefenceAdvocate.Should().Be(dA);
        }
    }
}