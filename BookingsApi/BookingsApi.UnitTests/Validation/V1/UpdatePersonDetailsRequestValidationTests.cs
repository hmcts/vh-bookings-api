using BookingsApi.Contract.V1.Requests;
using BookingsApi.Validations.V1;
using FizzWare.NBuilder;

namespace BookingsApi.UnitTests.Validation.V1
{
    public class UpdatePersonDetailsRequestValidationTests
    {
        private UpdatePersonDetailsRequestValidation _validator;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new UpdatePersonDetailsRequestValidation();
        }
        
        [Test]
        public async Task Should_pass_validation()
        {
            var request = BuildRequest();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
        
        [Test]
        public async Task Should_fail_validation_when_first_name_is_missing()
        {
            var request = BuildRequest();
            request.FirstName = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == UpdatePersonDetailsRequestValidation.NoFirstNameErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_fail_validation_when_last_name_is_missing()
        {
            var request = BuildRequest();
            request.LastName = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == UpdatePersonDetailsRequestValidation.NoLastNameErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_fail_validation_when_username_is_missing()
        {
            var request = BuildRequest();
            request.Username = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == UpdatePersonDetailsRequestValidation.NoUsernameErrorMessage)
                .Should().BeTrue();
        }
        
        private UpdatePersonDetailsRequest BuildRequest()
        {
            return Builder<UpdatePersonDetailsRequest>.CreateNew()
                .Build();
        }
    }
}