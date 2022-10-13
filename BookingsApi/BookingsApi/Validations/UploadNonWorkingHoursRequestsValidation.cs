using BookingsApi.Contract.Requests;
using FluentValidation;
using FluentValidation.Results;
using System.Collections.Generic;

namespace BookingsApi.Validations
{
    public class UploadNonWorkingHoursRequestsValidation : AbstractValidator<List<UploadNonWorkingHoursRequest>>
    {
        public UploadNonWorkingHoursRequestsValidation()
        {
        }

        public ValidationResult ValidateRequests(List<UploadNonWorkingHoursRequest> requests)
        {
            var errors = new List<ValidationFailure>();

            foreach (var request in requests)
            {
                foreach (var nonWorkingHours in request.NonWorkingHours)
                {
                    if (nonWorkingHours.EndTime < nonWorkingHours.StartTime)
                    {
                        errors.Add(new ValidationFailure($"{request.Username}", $"End time {nonWorkingHours.EndTime} is before start time {nonWorkingHours.StartTime}."));
                    }
                }
            }

            return new ValidationResult(errors);
        }
    }
}