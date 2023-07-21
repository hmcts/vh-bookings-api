
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Validations;
using FluentAssertions;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace BookingsApi.UnitTests.Validation
{
    public class AddEndpointRequestValidationTests
    {
        private AddEndpointRequestValidation _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new AddEndpointRequestValidation();
        }

        [Test]
        public async Task Should_pass_validation()
        {
            var request = new AddEndpointRequest
            {
                DisplayName = "Display name"
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_fail_validation_when_display_name_is_empty()
        {
            var request = new AddEndpointRequest
            {
                DisplayName = string.Empty,
            };

            var result = await _validator.ValidateAsync(request);
            result.Errors.Any(x => x.ErrorMessage == AddEndpointRequestValidation.NoDisplayNameError)
                .Should().BeTrue();
        }
    }
}
