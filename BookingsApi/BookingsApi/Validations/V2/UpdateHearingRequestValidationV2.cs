using BookingsApi.Contract.V2.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V2
{
    public class UpdateHearingRequestValidationV2 : AbstractValidator<UpdateHearingRequestV2>
    {
        public static readonly string NoHearingVenueCodeErrorMessage = "Hearing code cannot not be blank";
        public static readonly string ScheduleDateTimeInPastErrorMessage = "ScheduledDateTime cannot be in the past";
        public static readonly string NoScheduleDurationErrorMessage = "Schedule duration must be greater than 0";
        public static readonly string NoUpdatedByErrorMessage = "UpdatedBy is missing";

        public UpdateHearingRequestValidationV2()
        {
            RuleFor(x => x.HearingVenueCode)
                .NotEmpty().WithMessage(NoHearingVenueCodeErrorMessage);

            RuleFor(x => x.ScheduledDateTime).Custom((dateTime, context) =>
            {
                if (dateTime < DateTime.UtcNow)
                {
                    context.AddFailure(ScheduleDateTimeInPastErrorMessage);
                }
            });
            
            RuleFor(x => x.ScheduledDuration)
                .GreaterThan(0).WithMessage(NoScheduleDurationErrorMessage);

            RuleFor(x => x.UpdatedBy)
                .NotEmpty().WithMessage(NoUpdatedByErrorMessage);
        }
    }
}