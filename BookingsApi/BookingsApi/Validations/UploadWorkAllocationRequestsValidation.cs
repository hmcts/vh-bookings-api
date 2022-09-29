using BookingsApi.Contract.Requests;
using FluentValidation;
using FluentValidation.Results;
using System.Collections.Generic;
using System.Linq;

namespace BookingsApi.Validations
{
    public class UploadWorkAllocationRequestsValidation : AbstractValidator<List<UploadWorkAllocationRequest>>
    {
        public static readonly string NoParticipantEmail = "Contact email for participant is required";
        public static readonly string NoLinkedParticipantEmail = "Contact email for linked participant is required";
        public static readonly string InvalidType = "A valid linked participant type is required";
        
        public UploadWorkAllocationRequestsValidation()
        {
        }

        public ValidationResult ValidateRequests(List<UploadWorkAllocationRequest> requests)
        {
            var errors = new List<ValidationFailure>();

            foreach (var request in requests)
            {
                foreach (var dayWorkHours in request.DayWorkHours)
                {
                    var containsNullTime = dayWorkHours.GetType().GetProperties()
                        .Where(pi => pi.PropertyType == typeof(int?))
                        .Select(pi => (int?)pi.GetValue(dayWorkHours))
                        .Any(value => value == null);

                    var containsPopulatedTime = dayWorkHours.GetType().GetProperties()
                        .Where(pi => pi.PropertyType == typeof(int?))
                        .Select(pi => (int?)pi.GetValue(dayWorkHours))
                        .Any(value => value != null);

                    if (containsNullTime && containsPopulatedTime)
                    {
                        errors.Add(new ValidationFailure($"{request.Username}, Day ${dayWorkHours.DayOfWeekId}", "Day contains a blank start/end time along with a populated start/end time."));
                        continue;
                    }

                    if (dayWorkHours.EndTime < dayWorkHours.StartTime)
                    {
                        errors.Add(new ValidationFailure($"{request.Username}, Day ${dayWorkHours.DayOfWeekId}", $"End time {dayWorkHours.EndTime} is before {dayWorkHours.StartTime}."));
                    }
                }
            }

            return new ValidationResult(errors);
        }
    }
}