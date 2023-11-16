using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;

namespace BookingsApi.UnitTests.Domain.JudiciaryParticipants
{
    public class GetEmailTests
    {
        [Test]
        public void Should_return_judiciary_participant_contact_email_when_judiciary_person_is_generic()
        {
            // Arrange
            var judiciaryPerson = new JudiciaryPersonBuilder(isGeneric: true).Build();
            const string displayName = "DisplayName";
            const JudiciaryParticipantHearingRoleCode hearingRoleCode = JudiciaryParticipantHearingRoleCode.Judge;
            const string contactEmail = "generic-email@email.com";
            var judiciaryParticipant = new JudiciaryParticipant(displayName, judiciaryPerson, hearingRoleCode, contactEmail: contactEmail);

            // Act & Assert
            var email = judiciaryParticipant.GetEmail();
            email.Should().Be(judiciaryParticipant.ContactEmail);
        }
        
        [Test]
        public void Should_return_judiciary_person_email_when_judiciary_person_is_not_generic()
        {
            // Arrange
            var judiciaryPerson = new JudiciaryPersonBuilder(isGeneric: false).Build();
            const string displayName = "DisplayName";
            const JudiciaryParticipantHearingRoleCode hearingRoleCode = JudiciaryParticipantHearingRoleCode.Judge;
            var judiciaryParticipant = new JudiciaryParticipant(displayName, judiciaryPerson, hearingRoleCode);
            
            // Act & Assert
            var email = judiciaryParticipant.GetEmail();
            email.Should().Be(judiciaryPerson.Email);
        }
    }
}
