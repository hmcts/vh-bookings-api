using Bookings.API.Mappings;
using Bookings.Domain;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace Bookings.UnitTests.Mappings
{
    public class EndpointToResponseMapperTests
    {
        [Test]
        public void Should_map_endpoint_to_endpointresponse()
        {
            var participant = new ParticipantBuilder().Build();

            var source = new Endpoint("displayName","sip","pin", participant[0]);

            var result = EndpointToResponseMapper.MapEndpointToResponse(source);

            result.Id.Should().Be(source.Id);
            result.DisplayName.Should().Be(source.DisplayName);
            result.Sip.Should().Be(source.Sip);
            result.Pin.Should().Be(source.Pin);
            result.DefenceAdvocateId.Should().Be(participant[0].Id);
        }
    }
}
