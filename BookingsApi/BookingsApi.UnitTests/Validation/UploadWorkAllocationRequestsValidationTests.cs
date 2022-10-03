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
        private string _username;

        [SetUp]
        public void SetUp()
        {
            _username = "username@email.com";
        }

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
                    Username = _username,
                    WorkingHours = new List<WorkingHours> {
                        new WorkingHours(1, endTimeHour, endTimeMinutes, startTimeHour, startTimeMinutes)
                    }
                }
            };

            var validator = new UploadWorkAllocationRequestsValidation();

            // Act
            var result = validator.ValidateRequests(requests);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors[0].PropertyName.Should().Be($"{_username}, Day Number 1");
            result.Errors[0].ErrorMessage.Should().Be("Day contains a blank start/end time along with a populated start/end time.");
        }

        [Test]
        public void Should_pass_validation_when_all_times_are_null()
        {
            // Arrange
            var requests = new List<UploadWorkAllocationRequest>
            {
                new UploadWorkAllocationRequest
                {
                    Username = _username,
                    WorkingHours = new List<WorkingHours> {
                        new WorkingHours(1, null, null, null, null)
                    }
                }
            };

            var validator = new UploadWorkAllocationRequestsValidation();

            // Act
            var result = validator.ValidateRequests(requests);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [TestCase(-1)]
        [TestCase(60)]
        public void Should_fail_validation_when_start_time_minutes_is_not_valid(int startTimeMinutes)
        {
            // Arrange
            var requests = new List<UploadWorkAllocationRequest>
            {
                new UploadWorkAllocationRequest
                {
                    Username = _username,
                    WorkingHours = new List<WorkingHours> {
                        new WorkingHours(1, 9, startTimeMinutes, 17, 0)
                    }
                }
            };

            var validator = new UploadWorkAllocationRequestsValidation();

            // Act
            var result = validator.ValidateRequests(requests);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors[0].PropertyName.Should().Be($"{_username}, Day Number 1");
            result.Errors[0].ErrorMessage.Should().Be("Start time minutes is not within 0-59.");
        }

        [TestCase(-1)]
        [TestCase(60)]
        public void Should_fail_validation_when_end_time_minutes_is_not_valid(int endTimeMinutes)
        {
            // Arrange
            var requests = new List<UploadWorkAllocationRequest>
            {
                new UploadWorkAllocationRequest
                {
                    Username = _username,
                    WorkingHours = new List<WorkingHours> {
                        new WorkingHours(1, 09, 0, 17, endTimeMinutes)
                    }
                }
            };

            var validator = new UploadWorkAllocationRequestsValidation();

            // Act
            var result = validator.ValidateRequests(requests);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors[0].PropertyName.Should().Be($"{_username}, Day Number 1");
            result.Errors[0].ErrorMessage.Should().Be("End time minutes is not within 0-59.");
        }

        [Test]
        public void Should_fail_validation_when_end_time_is_before_start_time()
        {
            // Arrange
            var requests = new List<UploadWorkAllocationRequest>
            {
                new UploadWorkAllocationRequest
                {
                    Username = _username,
                    WorkingHours = new List<WorkingHours> {
                        new WorkingHours(1, 17, 0, 9, 0)
                    }
                }
            };

            var validator = new UploadWorkAllocationRequestsValidation();

            // Act
            var result = validator.ValidateRequests(requests);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors[0].PropertyName.Should().Be($"{_username}, Day Number 1");
            result.Errors[0].ErrorMessage.Should().Be("End time 09:00:00 is before start time 17:00:00.");
        }

        [Test]
        public void Should_pass_validation_when_all_times_are_valid()
        {
            // Arrange
            var requests = new List<UploadWorkAllocationRequest>
            {
                new UploadWorkAllocationRequest
                {
                    Username = _username,
                    WorkingHours = new List<WorkingHours> {
                        new WorkingHours(1, 9, 0, 17, 0)
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