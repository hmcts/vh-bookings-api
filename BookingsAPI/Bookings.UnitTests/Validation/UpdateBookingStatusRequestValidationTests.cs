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

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new UpdateBookingStatusRequestValidation();
        }

        [Test]
        public async Task should_pass_validation_when_request_is_not_empty()
        {
            var request = new UpdateBookingStatusRequest
            {
                Status = UpdateBookingStatus.Cancelled,
                UpdatedBy = "TestUser"
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task should_fail_validation_when_request_has_empty_booking_status()
        {
            object emptyStatus = null;
            var request = new UpdateBookingStatusRequest
            {
                Status = (UpdateBookingStatus)emptyStatus,
                UpdatedBy = "TestUser"
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
        }
    }
}
