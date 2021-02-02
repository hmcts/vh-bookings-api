using Bookings.Domain;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace BookingsApi.UnitTests.Domain.Hearing
{
    public class AssignDefenceAdvocateTests
    {
        [Test]
        public void should_assign_defence_advocate()
        {
            var ep = new Endpoint("DisplayName", "sip@address.com", "1111", null);
            var dA = new ParticipantBuilder().RepresentativeParticipantDefendant;
            
            ep.AssignDefenceAdvocate(dA);
            
            ep.DefenceAdvocate.Should().Be(dA);
        }
    }
}