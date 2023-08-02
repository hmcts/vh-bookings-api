using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Requests.Enums;
using BookingsApi.Validations.V1;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Validation.V1
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
                .With(x => x.ContactTelephone, null)
                .With(x=>x.Roles, new List<JusticeUserRole>() { JusticeUserRole.Vho })
                .Build();
            
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
        
        [TestCase("+44 123456 123", false)]
        [TestCase("(44) 123456 123", false)]
        [TestCase("(44) 123456 1234", true)]
        [TestCase("+44 123456 1234", true)]
        [TestCase("+441234567890", true)]
        [TestCase("01234567890", true)]
        [TestCase("01234567890", true)]
        [TestCase("0123 456 7890", true)]
        [TestCase("0 123 456 7890", true)]
        [TestCase("0123456789999999", false)]
        [TestCase("Hel1£ffhf", false)]
        public async Task should_validate_uk_phone_number(string telephone, bool expected)
        {
            var request = Builder<AddJusticeUserRequest>.CreateNew()
                .With(x=> x.Username, Faker.Internet.UserName())
                .With(x=> x.ContactEmail, Faker.Internet.Email())
                .With(x => x.ContactTelephone, telephone)
                .With(x=>x.Roles, new List<JusticeUserRole>() { JusticeUserRole.Vho })
                .Build();
            
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().Be(expected);
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
                .With(x=>x.Roles, new List<JusticeUserRole>() { JusticeUserRole.Vho })
                .Build();
            
            var result = await _validator.ValidateAsync(request);
            
            result.IsValid.Should().BeFalse();
            result.Errors.Any(x => x.ErrorMessage == AddJusticeUserRequestValidation.FirstNameDoesntMatchRegex)
                .Should().BeTrue();
            result.Errors.Any(x => x.ErrorMessage == AddJusticeUserRequestValidation.LastNameDoesntMatchRegex)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task should_fail_validation_when_roles_does_not_have_any_values()
        {
            var request = Builder<AddJusticeUserRequest>.CreateNew()
                .With(x=> x.Username, Faker.Internet.UserName())
                .With(x=> x.ContactEmail, Faker.Internet.Email())
                .With(x => x.ContactTelephone, null)
                .With(x=>x.Roles, new List<JusticeUserRole>())
                .Build();
            
            var result = await _validator.ValidateAsync(request);
            
            result.IsValid.Should().BeFalse();
            result.Errors.Any(x => x.ErrorMessage == AddJusticeUserRequestValidation.NoRoleErrorMessage)
                .Should().BeTrue();
        }
    }
}