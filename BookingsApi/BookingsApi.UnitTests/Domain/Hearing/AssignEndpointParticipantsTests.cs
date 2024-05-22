using BookingsApi.Domain;

namespace BookingsApi.UnitTests.Domain.Hearing
{
    public class AssignEndpointParticipantsTests
    {
        [Test]
        public void should_assign_endpoint_participant()
        {
            var ep = new Endpoint("DisplayName", "sip@address.com", "1111");
            
            var dA = new ParticipantBuilder().RepresentativeParticipantRespondent;
            var rep = new ParticipantBuilder().RepresentativeParticipantRespondent;
            var inter = new ParticipantBuilder().RepresentativeParticipantRespondent;
            
            ep.AssignDefenceAdvocate(dA);
            ep.AssignRepresentative(rep);
            ep.AssignIntermediary(inter);
            
            ep.GetDefenceAdvocate().Should().Be(dA);
            ep.GetRepresentative().Should().Be(rep);
            ep.GetIntermediary().Should().Be(inter);
        }
        
    }
}