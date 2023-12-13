using BookingsApi.Contract.V1.Requests;
using BookingsApi.Validations.V1;

namespace BookingsApi.UnitTests.Validation.V1
{
    public class ReassignJudiciaryJudgeRequestValidationTests
    {
        private ReassignJudiciaryJudgeRequestValidation _validator;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new ReassignJudiciaryJudgeRequestValidation();
        }
        
        [Test]
        public async Task Should_pass_validation()
        {
            var request = new ReassignJudiciaryJudgeRequest
            {
                DisplayName = "DisplayName",
                PersonalCode = "PersonalCode"
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_return_missing_display_name_error()
        {
            var request = new ReassignJudiciaryJudgeRequest
            {
                PersonalCode = "PersonalCode"
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Exists(x => x.ErrorMessage == JudiciaryParticipantRequestValidation.NoDisplayNameErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_missing_personal_code_error()
        {
            var request = new ReassignJudiciaryJudgeRequest
            {
                DisplayName = "DisplayName"
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Exists(x => x.ErrorMessage == JudiciaryParticipantRequestValidation.NoPersonalCodeErrorMessage)
                .Should().BeTrue();
        }
    }
}
