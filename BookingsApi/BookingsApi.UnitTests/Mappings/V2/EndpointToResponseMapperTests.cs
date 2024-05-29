using System.Collections.Generic;
using BookingsApi.Common.Services;
using BookingsApi.Contract.V2.Enums;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Mappings.V2;

namespace BookingsApi.UnitTests.Mappings.V2
{
    public class EndpointToResponseMapperTests
    {
        [TestCase]
        public void Should_run()
        {
            var sipAddStream = "TestSipStream";
            var randomGen = new Mock<IRandomGenerator>();
            var endpointRequest = new EndpointRequestV2 { EndpointParticipants = new List<EndpointParticipantsRequestV2>
            {
                new () { ContactEmail = "TestEmail", Type = LinkedParticipantTypeV2.DefenceAdvocate }
            
            }, DisplayName = "TestDispName" };

            var result = EndpointToResponseV2Mapper.MapRequestToNewEndpointDto(endpointRequest,randomGen.Object, sipAddStream);

            result.Should().NotBeNull();
            result.Sip.EndsWith(sipAddStream).Should().BeTrue();
            result.DisplayName.Should().Be(endpointRequest.DisplayName);
            result.EndpointParticipants[0].ContactEmail.Should().Be(endpointRequest.EndpointParticipants[0].ContactEmail);

        }

        [Test]
        public void Should_map_endpoint_to_endpointresponse()
        {
            var participant = new ParticipantBuilder().Build();

            var source = new Endpoint("displayName", "sip", "pin", (participant[0], LinkedParticipantType.DefenceAdvocate));

            var result = EndpointToResponseV2Mapper.MapEndpointToResponse(source);

            result.Id.Should().Be(source.Id);
            result.DisplayName.Should().Be(source.DisplayName);
            result.Sip.Should().Be(source.Sip);
            result.Pin.Should().Be(source.Pin);
            result.EndpointParticipants.Should().Contain(e => e.ParticipantId == participant[0].Id && e.LinkedParticipantType == LinkedParticipantTypeV2.DefenceAdvocate);
        }
    }
}
