using BookingsApi.Contract.V1.Requests;
using BookingsApi.Validations;
using FluentAssertions;
using NUnit.Framework;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Requests.Enums;

namespace BookingsApi.UnitTests.Validation
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
            request.CancelReason = "settled";

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

        [Test]
        public async Task Should_pass_validation_when_status_is_cancelled_and_cancel_reason_is_empty()
        {
            request = new UpdateBookingStatusRequest();
            request.Status = UpdateBookingStatus.Cancelled;
            request.UpdatedBy = "TestUser";

            var result = await _validator.ValidateAsync(request);
            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be("Cancel reason is required when a hearing is cancelled");
        }

        [Test]
        public async Task Should_pass_validation_when_status_is_cancelled_and_cancel_reason_is_not_empty()
        {
            request.Status = UpdateBookingStatus.Cancelled;
            request.UpdatedBy = "TestUser";
            request.CancelReason = "some other information";

            var result = await _validator.ValidateAsync(request);
            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_pass_validation_when_status_is_created_and_cancel_reason_is_not_empty()
        {
            request.Status = UpdateBookingStatus.Created;
            request.UpdatedBy = "TestUser";

            var result = await _validator.ValidateAsync(request);
            result.IsValid.Should().BeTrue();
        }
    }
}
