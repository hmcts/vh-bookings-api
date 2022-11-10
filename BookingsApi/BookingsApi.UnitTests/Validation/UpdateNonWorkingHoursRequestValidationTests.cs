using System;
using System.Collections.Generic;
using BookingsApi.Contract.Requests;
using BookingsApi.Domain;
using BookingsApi.Validations;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Validation
{
    public class UpdateNonWorkingHoursRequestValidationTests
    {
        [Test]
        public void Should_pass_validation_with_valid_request()
        {
            // Arrange
            var newHours = new List<NonWorkingHours>
            {
                new()
                {
                    Id = 1,
                    StartTime = new DateTime(2022, 1, 1, 8, 0, 0, DateTimeKind.Utc),
                    EndTime = new DateTime(2022, 1, 1, 10, 0, 0, DateTimeKind.Utc)
                },
                new()
                {
                    Id = 0,
                    StartTime = new DateTime(2022, 2, 1, 8, 0, 0, DateTimeKind.Utc),
                    EndTime = new DateTime(2022, 2, 1, 10, 0, 0, DateTimeKind.Utc)
                }
            };

            var existingHours = new List<VhoNonAvailability>
            {
                new(1)
                {
                    StartTime = new DateTime(2022, 1, 1, 6, 0, 0, DateTimeKind.Utc),
                    EndTime = new DateTime(2022, 1, 1, 8, 0, 0, DateTimeKind.Utc)
                }
            };

            var request = new UpdateNonWorkingHoursRequest
            {
                Hours = newHours
            };

            // Act
            var result = new UpdateNonWorkingHoursRequestValidation().ValidateHours(request, existingHours);
            
            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void Should_fail_validation_when_work_hour_ids_not_found()
        {
            // Arrange
            var newHours = new List<NonWorkingHours>
            {
                new()
                {
                    Id = 1,
                    StartTime = new DateTime(2022, 1, 1, 8, 0, 0, DateTimeKind.Utc),
                    EndTime = new DateTime(2022, 1, 1, 10, 0, 0, DateTimeKind.Utc)
                }
            };
            
            var existingHours = new List<VhoNonAvailability>();
            
            var request = new UpdateNonWorkingHoursRequest
            {
                Hours = newHours
            };

            // Act
            var result = new UpdateNonWorkingHoursRequestValidation().ValidateHours(request, existingHours);
            
            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors[0].PropertyName.Should().Be("Hours");
            result.Errors[0].ErrorMessage.Should().Be(UpdateNonWorkingHoursRequestValidation.HourIdsNotFoundErrorMessage);
        }

        [Test]
        public void Should_fail_validation_when_work_hour_dates_overlap_with_existing_work_hour_not_in_request()
        {
            // Arrange
            var newHours = new List<NonWorkingHours>
            {
                new()
                {
                    Id = 1,
                    StartTime = new DateTime(2022, 1, 1, 10, 0, 0, DateTimeKind.Utc),
                    EndTime = new DateTime(2022, 1, 2, 10, 0, 0, DateTimeKind.Utc)
                }
            };

            var existingHours = new List<VhoNonAvailability>
            {
                new(1)
                {
                    StartTime = new DateTime(2022, 1, 1, 6, 0, 0, DateTimeKind.Utc),
                    EndTime = new DateTime(2022, 1, 1, 8, 0, 0, DateTimeKind.Utc)
                },
                new(2)
                {
                    StartTime = new DateTime(2022, 1, 2, 6, 0, 0, DateTimeKind.Utc),
                    EndTime = new DateTime(2022, 1, 2, 8, 0, 0, DateTimeKind.Utc)
                }
            };

            var request = new UpdateNonWorkingHoursRequest
            {
                Hours = newHours
            };
            
            // Act
            var result = new UpdateNonWorkingHoursRequestValidation().ValidateHours(request, existingHours);
            
            // Assert
            result.Errors[0].PropertyName.Should().Be("Hours");
            result.Errors[0].ErrorMessage.Should().Be(UpdateNonWorkingHoursRequestValidation.HoursOverlapErrorMessage);
        }
    }
}
