using System;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Requests.Enums;
using BookingsApi.Domain.RefData;
using BookingsApi.Validations;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Validation
{
    public class RestoreJusticeUserRequestValidationTests
    {
        private RestoreJusticeUserRequestValidation _validator;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new RestoreJusticeUserRequestValidation();
        }

        [Test]
        public async Task should_pass_edit_justice_user_validation_when_all_required_properties_are_set()
        {
            var request = Builder<RestoreJusticeUserRequest>.CreateNew().Build();
            
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
        
        [Test]
        public async Task should_fail_edit_justice_user_validation_when_required_properties_are_missing()
        {
            var request = Builder<RestoreJusticeUserRequest>.CreateNew().Build();
            request.Username = null;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Any(x => x.ErrorMessage == RestoreJusticeUserRequestValidation.NoUsernameErrorMessage)
                .Should().BeTrue();
        }
    }
}