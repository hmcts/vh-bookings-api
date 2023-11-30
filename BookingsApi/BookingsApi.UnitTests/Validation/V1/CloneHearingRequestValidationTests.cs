using System.Collections.Generic;
using BookingsApi.Common;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Validations.V1;

namespace BookingsApi.UnitTests.Validation.V1
{
    public class CloneHearingRequestValidationTests
    {
        [Test]
        public void should_pass_validation_when_dates_are_ahead_of_original_hearing()
        {
            var originalHearing = new VideoHearingBuilder().Build();
            var date1 = originalHearing.ScheduledDateTime.GetNextWorkingDay();
            var date2 = date1.GetNextWorkingDay();
            var date3 = date2.GetNextWorkingDay();
            var dates = new List<DateTime>
            {
                date1,
                date2,
                date3,
            };
            var request = new CloneHearingRequest {Dates = dates};

            var validator = new CloneHearingRequestValidation(originalHearing);
            var result = validator.ValidateDates(request);

            result.IsValid.Should().BeTrue();
        }
        
        [Test]
        public void should_fail_validation_when_dates_are_before_original_hearing()
        {
            var originalHearing = new VideoHearingBuilder().Build();
            var request = new CloneHearingRequest
            {
                Dates = new List<DateTime>
                {
                    originalHearing.ScheduledDateTime.GetNextWorkingDay(),
                    originalHearing.ScheduledDateTime.AddDays(-3).GetNextWorkingDay(),
                    originalHearing.ScheduledDateTime.AddDays(3).GetNextWorkingDay()
                }
            };

            var validator = new CloneHearingRequestValidation(originalHearing);
            var result = validator.ValidateDates(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == CloneHearingRequestValidation.InvalidDateRangeErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public void should_fail_validation_when_dates_are_same_as_original_hearing()
        {
            var originalHearing = new VideoHearingBuilder().Build();
            var request = new CloneHearingRequest
            {
                Dates = new List<DateTime>
                {
                    originalHearing.ScheduledDateTime
                }
            };

            var validator = new CloneHearingRequestValidation(originalHearing);
            var result = validator.ValidateDates(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == CloneHearingRequestValidation.InvalidDateRangeErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public void should_fail_validation_when_list_contains_duplicate_dates()
        {
            var originalHearing = new VideoHearingBuilder().Build();
            var date = originalHearing.ScheduledDateTime.GetNextWorkingDay();
            var request = new CloneHearingRequest
            {
                Dates = new List<DateTime>
                {
                    date,date
                }
            };

            var validator = new CloneHearingRequestValidation(originalHearing);
            var result = validator.ValidateDates(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == CloneHearingRequestValidation.DuplicateDateErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public void should_pass_validation_when_scheduled_duration_is_valid()
        {
            var originalHearing = new VideoHearingBuilder().Build();
            var date = originalHearing.ScheduledDateTime.GetNextWorkingDay();
            var request = new CloneHearingRequest
            {
                Dates = new List<DateTime> { date },
                ScheduledDuration = 480
            };
            
            var validator = new CloneHearingRequestValidation();
            var result = validator.Validate(request);

            result.IsValid.Should().BeTrue();
        }
        
        [TestCase(0)]
        [TestCase(-1)]
        public void should_fail_validation_when_scheduled_duration_is_invalid(int scheduledDuration)
        {
            var originalHearing = new VideoHearingBuilder().Build();
            var date = originalHearing.ScheduledDateTime.GetNextWorkingDay();
            var request = new CloneHearingRequest
            {
                Dates = new List<DateTime> { date },
                ScheduledDuration = scheduledDuration
            };
            
            var validator = new CloneHearingRequestValidation();
            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == CloneHearingRequestValidation.InvalidScheduledDuration)
                .Should().BeTrue();
        }
    }
}