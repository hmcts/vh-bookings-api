using BookingsApi.Common.Services;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using BookingsApi.Mappings.V1;
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
            var endpointRequest = new EndpointRequestV2 { DefenceAdvocateContactEmail = "TestUserName", DisplayName = "TestDispName" };

            var result = EndpointToResponseV2Mapper.MapRequestToNewEndpointDto(endpointRequest,randomGen.Object, sipAddStream);

            result.Should().NotBeNull();
            result.Sip.EndsWith(sipAddStream).Should().BeTrue();
            result.DisplayName.Should().Be(endpointRequest.DisplayName);
            result.ContactEmail.Should().Be(endpointRequest.DefenceAdvocateContactEmail);

        }

        [Test]
        public void Should_map_endpoint_to_endpoint_response()
        {
            var participant = new ParticipantBuilder().Build();

            var source = new Endpoint("displayName", "sip", "pin", participant[0]);
            var interpreterLanguage = new InterpreterLanguage(1, "spa", "Spanish", "WelshValue", InterpreterType.Verbal, true);
            source.UpdateLanguagePreferences(interpreterLanguage, null);
            
            var result = EndpointToResponseV2Mapper.MapEndpointToResponse(source);

            result.Id.Should().Be(source.Id);
            result.DisplayName.Should().Be(source.DisplayName);
            result.Sip.Should().Be(source.Sip);
            result.Pin.Should().Be(source.Pin);
            result.DefenceAdvocateId.Should().Be(participant[0].Id);
            result.InterpreterLanguage.Should().NotBeNull();
            result.InterpreterLanguage.Should().BeEquivalentTo(InterpreterLanguageToResponseMapper.MapInterpreterLanguageToResponse(interpreterLanguage));
        }

        [Test]
        public void Should_map_endpoint_without_interpreter_language_to_endpoint_response()
        {
            // Arrange
            var participant = new ParticipantBuilder().Build();

            var endpoint = new Endpoint("displayName", "sip", "pin", participant[0]);
            endpoint.UpdateLanguagePreferences(null, "OtherLanguage");
            
            // Act
            var result = EndpointToResponseV2Mapper.MapEndpointToResponse(endpoint);
            
            // Assert
            result.InterpreterLanguage.Should().BeNull();
            result.OtherLanguage.Should().Be(endpoint.OtherLanguage);
        }
    }
}
