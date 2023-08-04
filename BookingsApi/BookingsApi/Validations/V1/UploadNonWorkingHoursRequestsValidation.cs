using BookingsApi.Contract.V1.Requests;
using FluentValidation;
using FluentValidation.Results;
using ValidationFailure = FluentValidation.Results.ValidationFailure;

namespace BookingsApi.Validations.V1
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