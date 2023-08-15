using BookingsApi.Common.Services;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Domain;
using BookingsApi.Mappings.V1;

namespace BookingsApi.UnitTests.Mappings.V1
{
    
    public class EndpointToResponseMapperTests
    {
        [TestCase]
        public void Should_run()
        {
            var sipAddStream = "TestSipStream";
            var randomGen = new Mock<IRandomGenerator>();
            var endpointRequest = new EndpointRequest { DefenceAdvocateContactEmail = "TestUserName", DisplayName = "TestDispName" };

            var result = EndpointToResponseMapper.MapRequestToNewEndpointDto(endpointRequest,randomGen.Object, sipAddStream);

            result.Should().NotBeNull();
            result.Sip.EndsWith(sipAddStream).Should().BeTrue();
            result.DisplayName.Should().Be(endpointRequest.DisplayName);
            result.ContactEmail.Should().Be(endpointRequest.DefenceAdvocateContactEmail);

        }

        [Test]
        public void Should_map_endpoint_to_endpointresponse()
        {
            var participant = new ParticipantBuilder().Build();

            var source = new Endpoint("displayName", "sip", "pin", participant[0]);

            var result = EndpointToResponseMapper.MapEndpointToResponse(source);

            result.Id.Should().Be(source.Id);
            result.DisplayName.Should().Be(source.DisplayName);
            result.Sip.Should().Be(source.Sip);
            result.Pin.Should().Be(source.Pin);
            result.DefenceAdvocateId.Should().Be(participant[0].Id);
        }
    }
}
