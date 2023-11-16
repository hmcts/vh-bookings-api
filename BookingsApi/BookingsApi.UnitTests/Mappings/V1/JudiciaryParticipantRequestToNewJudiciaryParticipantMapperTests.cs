using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Requests.Enums;
using BookingsApi.Extensions;
using BookingsApi.Mappings.V1;

namespace BookingsApi.UnitTests.Mappings.V1
{
    public class JudiciaryParticipantRequestToNewJudiciaryParticipantMapperTests
    {
        [TestCase(JudiciaryParticipantHearingRoleCode.Judge)]
        [TestCase(JudiciaryParticipantHearingRoleCode.PanelMember)]
        public void should_map_request(JudiciaryParticipantHearingRoleCode hearingRoleCode)
        {
            // Arrange
            var request = new JudiciaryParticipantRequest
            {
                DisplayName = "DisplayName",
                PersonalCode = "PersonalCode",
                HearingRoleCode = hearingRoleCode,
                OptionalContactTelephone = "01234567890",
                OptionalContactEmail = "generic-email@email.com"
            };

            // Act
            var result = JudiciaryParticipantRequestToNewJudiciaryParticipantMapper.Map(request);

            // Assert
            result.DisplayName.Should().Be(request.DisplayName);
            result.PersonalCode.Should().Be(request.PersonalCode);
            result.HearingRoleCode.Should().Be(request.HearingRoleCode.MapToDomainEnum());
            result.OptionalContactTelephone.Should().Be(request.OptionalContactTelephone);
            result.OptionalContactEmail.Should().Be(request.OptionalContactEmail);
        }
    }
}
