using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;

namespace BookingsApi.UnitTests.Domain.JudiciaryParticipants
{
    public class GetTelephoneTests
    {
        [Test]
        public void Should_return_judiciary_participant_contact_telephone_when_judiciary_person_is_generic()
        {
            // Arrange
            var judiciaryPerson = new JudiciaryPersonBuilder(isGeneric: true).Build();
            const string displayName = "DisplayName";
            const JudiciaryParticipantHearingRoleCode hearingRoleCode = JudiciaryParticipantHearingRoleCode.Judge;
            const string contactTelephone = "0123456789";
            var judiciaryParticipant = new JudiciaryParticipant(displayName, judiciaryPerson, hearingRoleCode, contactTelephone: contactTelephone);

            // Act & Assert
            var telephone = judiciaryParticipant.GetTelephone();
            telephone.Should().Be(judiciaryParticipant.ContactTelephone);
        }
        
        [Test]
        public void Should_return_judiciary_person_work_phone_when_judiciary_person_is_not_generic()
        {
            // Arrange
            var judiciaryPerson = new JudiciaryPersonBuilder(isGeneric: false).Build();
            const string displayName = "DisplayName";
            const JudiciaryParticipantHearingRoleCode hearingRoleCode = JudiciaryParticipantHearingRoleCode.Judge;
            var judiciaryParticipant = new JudiciaryParticipant(displayName, judiciaryPerson, hearingRoleCode);
            
            // Act & Assert
            var telephone = judiciaryParticipant.GetTelephone();
            telephone.Should().Be(judiciaryPerson.WorkPhone);
        }
    }
}
