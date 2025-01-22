using BookingsApi.Domain;

namespace BookingsApi.UnitTests.Domain.Hearing
{
    public class AssignDefenceAdvocateTests : DomainTests
    {
        [Test]
        public async Task should_assign_defence_advocate()
        {
            var ep = new Endpoint(Guid.NewGuid().ToString(),"DisplayName", "sip@address.com", "1111", null);
            var dA = new ParticipantBuilder().RepresentativeParticipantRespondent;
            var originalUpdatedDate = ep.UpdatedDate;

            await ApplyDelay();
            ep.AssignDefenceAdvocate(dA);
            
            ep.DefenceAdvocate.Should().Be(dA);
            ep.UpdatedDate.Should().BeAfter(originalUpdatedDate);
        }

        [Test]
        public async Task should_not_assign_defence_advocate_when_not_changed()
        {
            var dA = new ParticipantBuilder().RepresentativeParticipantRespondent;
            var ep = new Endpoint(Guid.NewGuid().ToString(),"DisplayName", "sip@address.com", "1111", dA);
            var originalUpdatedDate = ep.UpdatedDate;

            await ApplyDelay();
            ep.AssignDefenceAdvocate(dA);

            ep.DefenceAdvocate.Should().Be(dA);
            ep.UpdatedDate.Should().Be(originalUpdatedDate);
        }
    }
}