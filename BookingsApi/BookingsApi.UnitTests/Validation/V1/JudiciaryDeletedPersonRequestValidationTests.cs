using BookingsApi.Contract.V1.Requests;
using BookingsApi.Validations.V1;
using FluentValidation;

namespace BookingsApi.UnitTests.Validation.V1
{
    public class JudiciaryDeletedPersonRequestValidationTests
    {
        private JudiciaryDeletedPersonRequestValidation _validator;

        [OneTimeSetUp]
        public void SetUp()
        {
            _validator = new JudiciaryDeletedPersonRequestValidation();
        }
        
        [Test]
        public async Task Should_pass_validation_when_all_properties_are_valid()
        {
            var request = new JudiciaryPersonRequest
            {
                PersonalCode = "123",
                Deleted = true,
                DeletedOn = "2023-01-01"
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
        
        [TestCase(null)]
        [TestCase("")]
        public async Task Should_fail_validation_when_personal_code_is_missing(string personalCode)
        {
            var request = new JudiciaryPersonRequest
            {
                PersonalCode = personalCode,
                Deleted = true,
                DeletedOn = "2023-01-01"
            };

            var result = await _validator.ValidateAsync(request);
            
            result.IsValid.Should().BeFalse();
            result.Errors.Exists(x => x.ErrorMessage == JudiciaryDeletedPersonRequestValidation.NoPersonalCodeErrorMessage)
                .Should().BeTrue();
        }
    }
}
