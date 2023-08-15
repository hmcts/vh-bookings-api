﻿using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Validations.V1;

namespace BookingsApi.UnitTests.Validation.V1
{
    public class UpdateEndpointRequestValidationTests
    {
        private UpdateEndpointRequestValidation _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new UpdateEndpointRequestValidation();
        }

        [Test]
        public async Task Should_pass_validation()
        {
            var request = new UpdateEndpointRequest
            {
                DisplayName = "Display name"
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_fail_validation_when_display_name_is_empty()
        {
            var request = new UpdateEndpointRequest
            {
                DisplayName = string.Empty,
            };

            var result = await _validator.ValidateAsync(request);
            result.Errors.Any(x => x.ErrorMessage == UpdateEndpointRequestValidation.NoDisplayNameError)
                .Should().BeTrue();
        }
    }
}
