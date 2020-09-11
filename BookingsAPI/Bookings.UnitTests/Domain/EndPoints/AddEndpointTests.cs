using Bookings.Domain;
using Bookings.Domain.RefData;
using Bookings.UnitTests.Utilities;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace Bookings.UnitTests.Domain.EndPoints
{
    public class AddEndpointTests : TestBase
    {
        private Endpoint _endpoint;
        [SetUp]
        public void Initialise()
        {
            _endpoint = new Endpoint("DisplayName", "sip.address@email.net", "5555");
        }
        [Test]
        public void Should_assign_defence_advocate_to_the_hearing_endpoint()
        {
            var hearing = new VideoHearingBuilder().Build();
            var person = new Person("Mr.", "FirstName", "LastName", "firstName.lastName@email.com");
            var defenceAdvocate = hearing.AddRepresentative(person,
                new HearingRole(1, "HearingRoleName"), new CaseRole(1, "CaseRoleName"), "displayName", "reference001", "individual");
            _endpoint.AssignDefenceAdvocate(defenceAdvocate);
            _endpoint.DefenceAdvocate.Should().Be(defenceAdvocate);
        }
    }
}
