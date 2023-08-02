using System;
using System.Collections.Generic;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Validations.V1;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Validation.V1
{
    public class UploadNonWorkingHoursRequestsValidationTests
    {
        private string _username;

        [SetUp]
        public void SetUp()
        {
            _username = "username@email.com";
        }

        [Test]
        public void Should_fail_validation_when_end_time_is_before_start_time()
        {
            // Arrange
            var startTime = new DateTime(2022, 1, 7);
            var endTime = new DateTime(2022, 1, 1);

            var requests = new List<UploadNonWorkingHoursRequest>
            {
                new UploadNonWorkingHoursRequest(_username, startTime, endTime)
            };

            var validator = new UploadNonWorkingHoursRequestsValidation();

            // Act
            var result = validator.ValidateRequests(requests);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors[0].PropertyName.Should().Be(_username);
            result.Errors[0].ErrorMessage.Should().Be($"End time {endTime} is before start time {startTime}.");
        }

        [Test]
        public void Should_pass_validation_when_all_times_are_valid()
        {
            // Arrange
            var startTime = new DateTime(2022, 1, 1);
            var endTime = new DateTime(2022, 1, 7);

            var requests = new List<UploadNonWorkingHoursRequest>
            {
                new UploadNonWorkingHoursRequest(_username, startTime, endTime)
            };

            var validator = new UploadNonWorkingHoursRequestsValidation();

            // Act
            var result = validator.ValidateRequests(requests);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}