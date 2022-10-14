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
                if (request.EndTime < request.StartTime)
                {
                    errors.Add(new ValidationFailure($"{request.Username}", $"End time {request.EndTime} is before start time {request.StartTime}."));
                }
            }

            return new ValidationResult(errors);
        }
    }
}