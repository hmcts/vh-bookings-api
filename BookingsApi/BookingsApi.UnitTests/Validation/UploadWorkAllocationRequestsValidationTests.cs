using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Contract.Requests;
using BookingsApi.Validations;
using BookingsApi.Common;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace BookingsApi.UnitTests.Validation
{
    public class UploadWorkAllocationRequestsValidationTests
    {
        [TestCase(null, 1, 1, 1)]
        [TestCase(null, null, 1, 1)]
        [TestCase(1, 1, null, null)]
        [TestCase(1, 1, 1, null)]
        public void Should_fail_validation_when_some_times_are_populated_and_others_are_not(int? endTimeHour, int? endTimeMinutes, int? startTimeHour, int? startTimeMinutes)
        {
            // Arrange
            var requests = new List<UploadWorkAllocationRequest>
            {
                new UploadWorkAllocationRequest
                {
                    DayWorkHours = new List<DayWorkHours> {
                        new DayWorkHours(1, endTimeHour, endTimeMinutes, startTimeHour, startTimeMinutes)
                    }
                }
            };

            var validator = new UploadWorkAllocationRequestsValidation();

            // Act
            var result = validator.ValidateRequests(requests);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void Should_pass_validation_when_all_times_are_null()
        {
            // Arrange
            var requests = new List<UploadWorkAllocationRequest>
            {
                new UploadWorkAllocationRequest
                {
                    DayWorkHours = new List<DayWorkHours> {
                        new DayWorkHours(1, null, null, null, null)
                    }
                }
            };

            var validator = new UploadWorkAllocationRequestsValidation();

            // Act
            var result = validator.ValidateRequests(requests);

            // Assert
            result.IsValid.Should().BeTrue();
        }


        [Test]
        public void Should_fail_validation_when_end_time_is_before_start_time()
        {
            // Arrange
            var requests = new List<UploadWorkAllocationRequest>
            {
                new UploadWorkAllocationRequest
                {
                    DayWorkHours = new List<DayWorkHours> {
                        new DayWorkHours(1, 17, 0, 9, 0)
                    }
                }
            };

            var validator = new UploadWorkAllocationRequestsValidation();

            // Act
            var result = validator.ValidateRequests(requests);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void Should_pass_validation_when_all_times_are_valid()
        {
            // Arrange
            var requests = new List<UploadWorkAllocationRequest>
            {
                new UploadWorkAllocationRequest
                {
                    DayWorkHours = new List<DayWorkHours> {
                        new DayWorkHours(1, 9, 0, 17, 0)
                    }
                }
            };

            var validator = new UploadWorkAllocationRequestsValidation();

            // Act
            var result = validator.ValidateRequests(requests);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}