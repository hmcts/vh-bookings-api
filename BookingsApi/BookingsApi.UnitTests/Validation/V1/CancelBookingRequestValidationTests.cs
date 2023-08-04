using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Validations.V1;

namespace BookingsApi.UnitTests.Validation.V1
{
    public class CancelBookingRequestValidationTests
    {
        private CancelBookingRequestValidation _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new CancelBookingRequestValidation();
        }

        [Test]
        public async Task should_pass_validation()
        {
            var request = new CancelBookingRequest
            {
                CancelReason = "Not needed anymore",
                UpdatedBy = "Test",
            };
            
            var result = await _validator.ValidateAsync(request);
            
            result.IsValid.Should().BeTrue();
        }
        
        [Test]
        public async Task should_return_missing_cancel_reason_error()
        {
            var request = new CancelBookingRequest { UpdatedBy = "Test" };
            
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == CancelBookingRequestValidation.CancelReasonMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task should_return_missing_updateby_error()
        {
            var request = new CancelBookingRequest { CancelReason = "Cancelled" };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == CancelBookingRequestValidation.UpdatedByMessage)
                .Should().BeTrue();
        }
    }
}