using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.Validations;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Validation
{
    public class AddJusticeUserRequestValidationTests
    {
        private AddJusticeUserRequestValidation _validator;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new AddJusticeUserRequestValidation();
        }

        [Test]
        public async Task should_pass_add_justice_user_validation_when_all_required_properties_are_set()
        {
            var request = Builder<AddJusticeUserRequest>.CreateNew()
                .With(x=> x.Username, Faker.Internet.UserName())
                .With(x=> x.ContactEmail, Faker.Internet.Email())
                .Build();
            
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
        
        [Test]
        public async Task should_fail_add_justice_user_validation_when_required_properties_are_missing()
        {
            var request = Builder<AddJusticeUserRequest>.CreateNew().Build();
            request.FirstName = null;
            request.LastName = null;
            request.Username = null;
            request.CreatedBy = null;
            request.ContactEmail = null;
            
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Any(x => x.ErrorMessage == AddJusticeUserRequestValidation.NoFirstNameErrorMessage)
                .Should().BeTrue();
            result.Errors.Any(x => x.ErrorMessage == AddJusticeUserRequestValidation.NoLastNameErrorMessage)
                .Should().BeTrue();
            result.Errors.Any(x => x.ErrorMessage == AddJusticeUserRequestValidation.NoUsernameErrorMessage)
                .Should().BeTrue();
            result.Errors.Any(x => x.ErrorMessage == AddJusticeUserRequestValidation.NoCreatedByErrorMessage)
                .Should().BeTrue();
            result.Errors.Any(x => x.ErrorMessage == AddJusticeUserRequestValidation.NoContactEmailErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task should_fail_validation_when_first_or_last_name_do_not_pass_regex_for_acceptable_values()
        {
            var request = Builder<AddJusticeUserRequest>.CreateNew()
                .With(x=> x.FirstName, "John Doe  DoubleSpace")
                .With(x=> x.LastName, "    Another   Space")
                .Build();
            
            var result = await _validator.ValidateAsync(request);
            
            result.IsValid.Should().BeFalse();
            result.Errors.Any(x => x.ErrorMessage == AddJusticeUserRequestValidation.FirstNameDoesntMatchRegex)
                .Should().BeTrue();
            result.Errors.Any(x => x.ErrorMessage == AddJusticeUserRequestValidation.LastNameDoesntMatchRegex)
                .Should().BeTrue();
        }
    }
}