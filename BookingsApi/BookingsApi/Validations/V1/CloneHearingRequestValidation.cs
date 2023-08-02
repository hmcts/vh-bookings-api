using System.Collections.Generic;
using System.Linq;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Domain;
using FluentValidation.Results;

namespace BookingsApi.Validations.V1
{
    public class CloneHearingRequestValidation 
    {
        private readonly Hearing _originalHearing;
        public const string InvalidDateRangeErrorMessage = "Dates cannot be before original hearing";
        public const string DuplicateDateErrorMessage = "Dates must be unique";

        
        public CloneHearingRequestValidation(Hearing originalHearing)
        {
            _originalHearing = originalHearing;
        }

        public ValidationResult ValidateDates(CloneHearingRequest request)
        {
            var errors = new List<ValidationFailure>();
            if (request.Dates.Any(x => x <= _originalHearing.ScheduledDateTime))
            {
                errors.Add(new ValidationFailure(nameof(request.Dates), InvalidDateRangeErrorMessage));
            }
            var allDays = request.Dates.Select(x => x.DayOfYear).ToList();
            if (allDays.Count != allDays.Distinct().Count())
            {
                errors.Add(new ValidationFailure(nameof(request.Dates), DuplicateDateErrorMessage));
            }

            return new ValidationResult(errors);
        }
        
    }
}