using BookingsApi.Contract.V1.Requests;
using BookingsApi.Domain;
using BookingsApi.Validations.V1;

namespace BookingsApi.UnitTests.Validation.V1
{
    public class UpdateHearingRequestValidationTests
    {
        private UpdateHearingRequestValidation _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new UpdateHearingRequestValidation();
        }

        [Test]
        public async Task Should_pass_validation()
        {
            var request = BuildRequest();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_return_missing_hearing_venue_name_error()
        {
            // arrange
            // venue name and code are both empty
            var request = BuildRequest();
            request.HearingVenueName = string.Empty;

            // act
            var result = await _validator.ValidateAsync(request);

            // assert
            result.Errors.Exists(x => x.ErrorMessage == UpdateHearingRequestValidation.NoHearingVenueNameErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_hearing_schedule_date_time_in_past_error()
        {
            var request = BuildRequest();
            request.ScheduledDateTime = DateTime.MinValue;
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Exists(x => x.ErrorMessage == UpdateHearingRequestValidation.ScheduleDateTimeInPastErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_hearing_schedule_duration_error()
        {
            var request = BuildRequest();
            request.ScheduledDuration = 0;
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Exists(x => x.ErrorMessage == UpdateHearingRequestValidation.NoScheduleDurationErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_missing_updated_by_error()
        {
            var request = BuildRequest();
            request.UpdatedBy = string.Empty;
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Exists(x => x.ErrorMessage == UpdateHearingRequestValidation.NoUpdatedByErrorMessage)
                .Should().BeTrue();
        }

        private static UpdateHearingRequest BuildRequest()
        {
            return new UpdateHearingRequest
            {
                HearingVenueName = "Test Venue",
                ScheduledDuration = 60,
                ScheduledDateTime = DateTime.Today.AddDays(5).AddHours(10).AddMinutes(30),
                HearingRoomName = "RoomUpdate",
                OtherInformation = "OtherInformationUpdate",
                UpdatedBy = "test@hmcts.net"
            };
        }
    }
}