using System;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.Validations;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Validation
{
    public class UpdateHearingRequestValidationTests
    {
        private UpdateHearingRequestValidation _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
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
            var request = BuildRequest();
            request.HearingVenueName = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == UpdateHearingRequestValidation.NoHearingNameErrorMessage)
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
            result.Errors.Any(x => x.ErrorMessage == UpdateHearingRequestValidation.ScheduleDateTimeInPastErrorMessage)
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
            result.Errors.Any(x => x.ErrorMessage == UpdateHearingRequestValidation.NoScheduleDurationErrorMessage)
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
            result.Errors.Any(x => x.ErrorMessage == UpdateHearingRequestValidation.NoUpdatedByErrorMessage)
                .Should().BeTrue();
        }

        private UpdateHearingRequest BuildRequest()
        {
            return new UpdateHearingRequest
            {
                HearingVenueName = "Test Venue",
                ScheduledDuration = 60,
                ScheduledDateTime = DateTime.Today.AddDays(5).AddHours(10).AddMinutes(30),
                HearingRoomName = "RoomUpdate",
                OtherInformation = "OtherInformationUpdate",
                UpdatedBy = "test@auto.com"
            };
        }
    }
}