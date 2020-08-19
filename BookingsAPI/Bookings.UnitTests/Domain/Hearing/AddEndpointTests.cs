using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace Bookings.UnitTests.Domain.Hearing
{
    public class AddEndpointTests
    {
        [Test]
        public void Should_add_new_endpoint()
        {
            var hearing = new VideoHearingBuilder().Build();
            var beforeAddCount = hearing.GetEndpoints().Count;
            hearing.AddEndpoint(new Bookings.Domain.Endpoint("DisplayName", "sip@address.com", "1111") );
            var afterAddCount = hearing.GetEndpoints().Count;
            afterAddCount.Should().BeGreaterThan(beforeAddCount);
        }
    }
}


