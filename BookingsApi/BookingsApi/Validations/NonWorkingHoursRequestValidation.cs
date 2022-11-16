using BookingsApi.Contract.Requests;
using FluentValidation;
namespace BookingsApi.Validations
{
    public class NonWorkingHoursRequestValidation : AbstractValidator<NonWorkingHours>
    {
        public const string EndTimeErrorMessage = "EndTime must be after StartTime";
        
        public NonWorkingHoursRequestValidation()
        {
            RuleFor(x => x.EndTime)
                .GreaterThan(x => x.StartTime).WithMessage(EndTimeErrorMessage);
        }
    }
}
