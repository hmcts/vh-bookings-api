using System.Collections.Generic;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Validations.V1;

namespace BookingsApi.UnitTests.Validation.V1
{
    public class UploadWorkHoursRequestsValidationTests
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
            var requests = new List<UploadWorkHoursRequest>
            {
                new()
                {
                    Username = _username,
                    WorkingHours = new List<WorkingHours> {
                        new(1, endTimeHour, endTimeMinutes, startTimeHour, startTimeMinutes),
                        new(2, null, null, null, null),
                        new(3, null, null, null, null),
                        new(4, null, null, null, null),
                        new(5, null, null, null, null),
                        new(6, null, null, null, null),
                        new(7, null, null, null, null)
                    }
                }
            };

            var validator = new UploadWorkHoursRequestsValidation();

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
            var requests = new List<UploadWorkHoursRequest>
            {
                new()
                {
                    Username = _username,
                    WorkingHours = new List<WorkingHours> {
                        new(1, null, null, null, null),
                        new(2, null, null, null, null),
                        new(3, null, null, null, null),
                        new(4, null, null, null, null),
                        new(5, null, null, null, null),
                        new(6, null, null, null, null),
                        new(7, null, null, null, null)
                    }
                }
            };

            var validator = new UploadWorkHoursRequestsValidation();

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
            var requests = new List<UploadWorkHoursRequest>
            {
                new()
                {
                    Username = _username,
                    WorkingHours = new List<WorkingHours> {
                        new(1, 9, startTimeMinutes, 17, 0),
                        new(2, null, null, null, null),
                        new(3, null, null, null, null),
                        new(4, null, null, null, null),
                        new(5, null, null, null, null),
                        new(6, null, null, null, null),
                        new(7, null, null, null, null)
                    }
                }
            };

            var validator = new UploadWorkHoursRequestsValidation();

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
            var requests = new List<UploadWorkHoursRequest>
            {
                new()
                {
                    Username = _username,
                    WorkingHours = new List<WorkingHours> {
                        new(1, 09, 0, 17, endTimeMinutes),
                        new(2, null, null, null, null),
                        new(3, null, null, null, null),
                        new(4, null, null, null, null),
                        new(5, null, null, null, null),
                        new(6, null, null, null, null),
                        new(7, null, null, null, null)
                    }
                }
            };

            var validator = new UploadWorkHoursRequestsValidation();

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
            var requests = new List<UploadWorkHoursRequest>
            {
                new()
                {
                    Username = _username,
                    WorkingHours = new List<WorkingHours> {
                        new(1, 17, 0, 9, 0),
                        new(2, null, null, null, null),
                        new(3, null, null, null, null),
                        new(4, null, null, null, null),
                        new(5, null, null, null, null),
                        new(6, null, null, null, null),
                        new(7, null, null, null, null)
                    }
                }
            };

            var validator = new UploadWorkHoursRequestsValidation();

            // Act
            var result = validator.ValidateRequests(requests);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors[0].PropertyName.Should().Be($"{_username}, Day Number 1");
            result.Errors[0].ErrorMessage.Should().Be("End time 09:00:00 is before start time 17:00:00.");
        }
        [Test]
        public void Should_fail_validation_when_duplicate_users_in_upload()
        {
            // Arrange
            var requests = new List<UploadWorkHoursRequest>
            {
                new()
                {
                    Username = _username,
                    WorkingHours = new List<WorkingHours> {
                        new(1, 17, 0, 9, 0),
                        new(2, null, null, null, null),
                        new(3, null, null, null, null),
                        new(4, null, null, null, null),
                        new(5, null, null, null, null),
                        new(6, null, null, null, null),
                        new(7, null, null, null, null)
                    }
                },
                new()
                {
                    Username = _username,
                    WorkingHours = new List<WorkingHours> {
                        new(1, 17, 0, 9, 0),
                        new(2, null, null, null, null),
                        new(3, null, null, null, null),
                        new(4, null, null, null, null),
                        new(5, null, null, null, null),
                        new(6, null, null, null, null),
                        new(7, null, null, null, null)
                    }
                } 
            };

            var validator = new UploadWorkHoursRequestsValidation();

            // Act
            var result = validator.ValidateRequests(requests);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors[0].PropertyName.Should().Be($"{_username}");
            result.Errors[0].ErrorMessage.Should().Be("Multiple entries for user. Only one row per user required");
        }
        [Test]
        public void Should_pass_validation_when_all_times_are_valid()
        {
            // Arrange
            var requests = new List<UploadWorkHoursRequest>
            {
                new()
                {
                    Username = _username,
                    WorkingHours = new List<WorkingHours> {
                        new(1, 9, 0, 17, 0),
                        new(2, null, null, null, null),
                        new(3, null, null, null, null),
                        new(4, null, null, null, null),
                        new(5, null, null, null, null),
                        new(6, null, null, null, null),
                        new(7, null, null, null, null)
                    }
                }
            };

            var validator = new UploadWorkHoursRequestsValidation();

            // Act
            var result = validator.ValidateRequests(requests);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void Should_fail_validation_when_days_are_missing()
        {
            // Arrange
            var requests = new List<UploadWorkHoursRequest>
            {
                new()
                {
                    Username = _username,
                    WorkingHours = new List<WorkingHours>
                    {
                        new(1, 9, 0, 17, 0),
                        new(2, 9, 0, 17, 0),
                        new(3, 9, 0, 17, 0),
                        new(4, 9, 0, 17, 0),
                        new(5, 9, 0, 17, 0)
                    }
                }
            };
            
            var validator = new UploadWorkHoursRequestsValidation();
            
            // Act
            var result = validator.ValidateRequests(requests);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors[0].PropertyName.Should().Be($"{_username}, Day Numbers");
            result.Errors[0].ErrorMessage.Should().Be("Must specify one entry for each day of the week for days 1-7");
        }
        
        [Test]
        public void Should_fail_validation_when_duplicate_days_are_specified()
        {
            // Arrange
            var requests = new List<UploadWorkHoursRequest>
            {
                new()
                {
                    Username = _username,
                    WorkingHours = new List<WorkingHours>
                    {
                        new(1, 9, 0, 17, 0),
                        new(2, 9, 0, 17, 0),
                        new(3, 9, 0, 17, 0),
                        new(4, 9, 0, 17, 0),
                        new(4, 9, 0, 17, 0),
                        new(6, 9, 0, 17, 0),
                        new(7, 9, 0, 17, 0),
                    }
                }
            };
            
            var validator = new UploadWorkHoursRequestsValidation();
            
            // Act
            var result = validator.ValidateRequests(requests);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors[0].PropertyName.Should().Be($"{_username}, Day Numbers");
            result.Errors[0].ErrorMessage.Should().Be("Must specify one entry for each day of the week for days 1-7");
        }
        
        [Test]
        public void Should_pass_validation_when_work_hours_are_valid_and_days_are_specified_in_non_sequential_order()
        {
            // Arrange
            var requests = new List<UploadWorkHoursRequest>
            {
                new()
                {
                    Username = _username,
                    WorkingHours = new List<WorkingHours>
                    {
                        new(2, 9, 0, 17, 0),
                        new(1, 9, 0, 17, 0),
                        new(3, 9, 0, 17, 0),
                        new(5, 9, 0, 17, 0),
                        new(4, 9, 0, 17, 0),
                        new(6, 9, 0, 17, 0),
                        new(7, 9, 0, 17, 0),
                    }
                }
            };
            
            var validator = new UploadWorkHoursRequestsValidation();
            
            // Act
            var result = validator.ValidateRequests(requests);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}