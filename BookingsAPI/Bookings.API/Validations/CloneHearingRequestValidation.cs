using System.Collections.Generic;
using System.Linq;
using Bookings.Api.Contract.Requests;
using Bookings.Domain;
using FluentValidation.Results;

namespace Bookings.API.Validations
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
            if (request.Dates.Any(x => x.DayOfYear <= _originalHearing.ScheduledDateTime.DayOfYear))
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