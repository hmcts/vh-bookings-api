using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using BookingsApi.Mappings.Common;
using BookingsApi.Mappings.V2;

namespace BookingsApi.UnitTests.Mappings.Common
{
    public class JudiciaryParticipantToResponseMapperTests
    {
        [Test]
        public void Should_map_judiciary_participant_to_response()
        {
            // Arrange
            var judiciaryParticipant = CreateJudiciaryParticipant();
            var interpreterLanguage = new InterpreterLanguage(1, "spa", "Spanish", "WelshValue", InterpreterType.Verbal, true);
            judiciaryParticipant.UpdateLanguagePreferences(interpreterLanguage, null);
  
            // Act
            var mapper = new JudiciaryParticipantToResponseMapper();
            var result = mapper.MapJudiciaryParticipantToResponse(judiciaryParticipant);

            // Assert
            result.PersonalCode.Should().Be(judiciaryParticipant.JudiciaryPerson.PersonalCode);
            result.DisplayName.Should().Be(judiciaryParticipant.DisplayName);
            result.HearingRoleCode.Should().Be(Contract.V2.Enums.JudiciaryParticipantHearingRoleCode.Judge);
            result.Email.Should().Be(judiciaryParticipant.JudiciaryPerson.Email);
            result.Title.Should().Be(judiciaryParticipant.JudiciaryPerson.Title);
            result.FirstName.Should().Be(judiciaryParticipant.JudiciaryPerson.KnownAs);
            result.LastName.Should().Be(judiciaryParticipant.JudiciaryPerson.Surname);
            result.FullName.Should().Be(judiciaryParticipant.JudiciaryPerson.Fullname);
            result.WorkPhone.Should().Be(judiciaryParticipant.JudiciaryPerson.WorkPhone);
            result.IsGeneric.Should().Be(judiciaryParticipant.JudiciaryPerson.IsGeneric);
            result.OptionalContactEmail.Should().Be(judiciaryParticipant.JudiciaryPerson.Email);
            result.OptionalContactTelephone.Should().Be(judiciaryParticipant.JudiciaryPerson.WorkPhone);
            result.InterpreterLanguage.Should().NotBeNull();
            result.InterpreterLanguage.Should().BeEquivalentTo(InterpreterLanguageToResponseMapperV2.MapInterpreterLanguageToResponse(interpreterLanguage));
        }

        [Test]
        public void Should_map_judiciary_participant_without_interpreter_language_to_response()
        {
            // Arrange
            var judiciaryParticipant = CreateJudiciaryParticipant();
            judiciaryParticipant.UpdateLanguagePreferences(null, "OtherLanguage");
  
            // Act
            var mapper = new JudiciaryParticipantToResponseMapper();
            var result = mapper.MapJudiciaryParticipantToResponse(judiciaryParticipant);
            
            // Assert
            result.InterpreterLanguage.Should().BeNull();
            result.OtherLanguage.Should().Be(judiciaryParticipant.OtherLanguage);
        }

        private static JudiciaryParticipant CreateJudiciaryParticipant()
        {
            const string personalCode = "PersonalCode";
            var judiciaryPerson = new JudiciaryPersonBuilder(personalCode).Build();
            var judiciaryParticipant = new JudiciaryParticipant("DisplayName", judiciaryPerson, 
                JudiciaryParticipantHearingRoleCode.Judge, "optionalEmail@email.com", 
                "1234");
            return judiciaryParticipant;
        }
    }
}
