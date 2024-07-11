using BookingsApi.Common.Services;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using BookingsApi.Mappings.V1;

namespace BookingsApi.UnitTests.Mappings.V1
{
    
    public class EndpointToResponseMapperTests
    {
        [TestCase]
        public void Should_map_request_to_new_endpoint_dto()
        {
            var sipAddStream = "TestSipStream";
            var randomGen = new Mock<IRandomGenerator>();
            var endpointRequest = new EndpointRequest
            {
                DefenceAdvocateContactEmail = "TestUserName", 
                DisplayName = "TestDispName",
                InterpreterLanguageCode = "spa",
                OtherLanguage = "other language"
            };

            var result = EndpointToResponseMapper.MapRequestToNewEndpointDto(endpointRequest,randomGen.Object, sipAddStream);

            result.Should().NotBeNull();
            result.Sip.EndsWith(sipAddStream).Should().BeTrue();
            result.DisplayName.Should().Be(endpointRequest.DisplayName);
            result.ContactEmail.Should().Be(endpointRequest.DefenceAdvocateContactEmail);
            result.LanguageCode.Should().Be(endpointRequest.InterpreterLanguageCode);
            result.OtherLanguage.Should().Be(endpointRequest.OtherLanguage);
        }

        [Test]
        public void Should_map_endpoint_to_endpoint_response()
        {
            var participant = new ParticipantBuilder().Build();

            var source = new Endpoint("displayName", "sip", "pin", participant[0]);
            var interpreterLanguage = new InterpreterLanguage(1, "spa", "Spanish", "", InterpreterType.Verbal, true);
            source.UpdateLanguagePreferences(interpreterLanguage, "");

            var result = EndpointToResponseMapper.MapEndpointToResponse(source);

            result.Id.Should().Be(source.Id);
            result.DisplayName.Should().Be(source.DisplayName);
            result.Sip.Should().Be(source.Sip);
            result.Pin.Should().Be(source.Pin);
            result.DefenceAdvocateId.Should().Be(participant[0].Id);
            result.InterpreterLanguageCode.Should().Be(source.InterpreterLanguage.Code);
            result.OtherLanguage.Should().BeEmpty();
        }

        [Test]
        public void Should_map_endpoint_with_other_language_to_endpoint_response()
        {
            var participant = new ParticipantBuilder().Build();

            var source = new Endpoint("displayName", "sip", "pin", participant[0]);
            source.UpdateLanguagePreferences(null, "other language");
            
            var result = EndpointToResponseMapper.MapEndpointToResponse(source);

            result.InterpreterLanguageCode.Should().BeNull();
            result.OtherLanguage.Should().Be(source.OtherLanguage);
        }
    }
}
