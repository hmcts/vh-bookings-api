using BookingsApi.Contract.V2.Requests;
using FluentValidation;
using FluentValidation.Results;
using ValidationFailure = FluentValidation.Results.ValidationFailure;

namespace BookingsApi.Validations.V2
{
    public class CloneHearingRequestValidationV2 : AbstractValidator<CloneHearingRequestV2>
    {
        private readonly Hearing _originalHearing;
        public const string InvalidDateRangeErrorMessage = "Dates cannot be before original hearing";
        public const string DuplicateDateErrorMessage = "Dates must be unique";
        public const string InvalidScheduledDuration = "Scheduled duration must be greater than 0";

        public CloneHearingRequestValidationV2()
        {
            RuleFor(x => x.ScheduledDuration).GreaterThan(0).WithMessage(InvalidScheduledDuration);
        }
        
        public CloneHearingRequestValidationV2(Hearing originalHearing)
        {
            _originalHearing = originalHearing;
        }

        public ValidationResult ValidateDates(CloneHearingRequestV2 request)
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