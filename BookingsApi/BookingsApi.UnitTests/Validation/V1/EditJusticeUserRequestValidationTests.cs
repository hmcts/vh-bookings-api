using BookingsApi.Contract.V1.Requests;
using BookingsApi.Validations.V1;
using FizzWare.NBuilder;

namespace BookingsApi.UnitTests.Validation.V1
{
    public class EditJusticeUserRequestValidationTests
    {
        private EditJusticeUserRequestValidation _validator;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new EditJusticeUserRequestValidation();
        }

        [Test]
        public async Task should_pass_edit_justice_user_validation_when_all_required_properties_are_set()
        {
            var request = Builder<EditJusticeUserRequest>.CreateNew().Build();
            request.FirstName = "William";
            request.LastName = "Craig";
            request.ContactTelephone = "01234546789";
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
        
        [Test]
        public async Task should_fail_edit_justice_user_validation_when_required_properties_are_missing()
        {
            var request = Builder<EditJusticeUserRequest>.CreateNew().Build();
            request.Username = null;
            request.FirstName = "Wil'liam";
            request.LastName = "Cra ig";
            request.ContactTelephone = "01234546789101112131415";
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Any(x => x.ErrorMessage == EditJusticeUserRequestValidation.NoUsernameErrorMessage)
                .Should().BeTrue();
        }
    }
}