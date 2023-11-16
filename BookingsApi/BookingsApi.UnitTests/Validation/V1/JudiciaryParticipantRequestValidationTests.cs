using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Requests.Enums;
using BookingsApi.Validations.V1;
using FizzWare.NBuilder;

namespace BookingsApi.UnitTests.Validation.V1
{
    public class JudiciaryParticipantRequestValidationTests
    {
        private JudiciaryParticipantRequestValidation _validator;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new JudiciaryParticipantRequestValidation();
        }

        [Test]
        public async Task should_pass_validation_when_all_properties_are_valid()
        {
            // Arrange
            var request = Builder<JudiciaryParticipantRequest>.CreateNew()
                .With(x=> x.PersonalCode, "PersonalCode")
                .With(x => x.DisplayName, "DisplayName")
                .With(x => x.HearingRoleCode, JudiciaryParticipantHearingRoleCode.Judge)
                .With(x => x.OptionalContactTelephone, "01234567890")
                .With(x => x.OptionalContactEmail, "email@email.com")
                .Build();
            
            // Act
            var result = await _validator.ValidateAsync(request);
            
            // Assert
            result.IsValid.Should().BeTrue();
        }
        
        [Test]
        public async Task should_pass_validation_when_all_required_properties_are_valid_and_optional_properties_are_not_specified()
        {
            // Arrange
            var request = Builder<JudiciaryParticipantRequest>.CreateNew()
                .With(x=> x.PersonalCode, "PersonalCode")
                .With(x => x.DisplayName, "DisplayName")
                .With(x => x.HearingRoleCode, JudiciaryParticipantHearingRoleCode.Judge)
                .Build();

            request.OptionalContactTelephone = null;
            request.OptionalContactEmail = null;
            
            // Act
            var result = await _validator.ValidateAsync(request);
            
            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task should_fail_validation_when_required_properties_are_not_specified()
        {
            // Arrange
            var request = Builder<JudiciaryParticipantRequest>.CreateNew()
                .With(x=> x.PersonalCode, null)
                .With(x => x.DisplayName, null)
                .Build();
            
            // Act
            var result = await _validator.ValidateAsync(request);
            
            // Assert
            result.Errors.Exists(x => x.ErrorMessage == JudiciaryParticipantRequestValidation.NoPersonalCodeErrorMessage)
                .Should().BeTrue();
            result.Errors.Exists(x => x.ErrorMessage == JudiciaryParticipantRequestValidation.NoDisplayNameErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task should_fail_validation_when_optional_contact_email_is_invalid()
        {
            // Arrange
            var request = Builder<JudiciaryParticipantRequest>.CreateNew()
                .With(x=> x.OptionalContactEmail, "abcd")
                .Build();

            // Act
            var result = await _validator.ValidateAsync(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Exists(x => x.ErrorMessage == JudiciaryParticipantRequestValidation.InvalidOptionalContactEmailErrorMessage)
                .Should().BeTrue();
        }
    }
}
