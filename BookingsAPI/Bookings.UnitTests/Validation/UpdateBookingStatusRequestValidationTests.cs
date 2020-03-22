using Bookings.Api.Contract.Requests;
using Bookings.API.Validations;
using FluentAssertions;
using NUnit.Framework;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests.Enums;

namespace Bookings.UnitTests.Validation
{
    public class UpdateBookingStatusRequestValidationTests
    {
        private UpdateBookingStatusRequestValidation _validator;
        private UpdateBookingStatusRequest request;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new UpdateBookingStatusRequestValidation();
            request = new UpdateBookingStatusRequest();
        }

        [Test]
        public async Task Should_pass_validation_when_request_is_not_empty()
        {
            request.Status = UpdateBookingStatus.Cancelled;
            request.UpdatedBy = "TestUser";

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_pass_validation_when_request_is_empty()
        {
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be("UpdatedBy is required");
            result.Errors[1].ErrorMessage.Should().Be("The booking status is not recognised");
        }
    }
}
