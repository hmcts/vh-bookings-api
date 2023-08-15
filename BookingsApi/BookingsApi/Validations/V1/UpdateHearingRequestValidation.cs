using BookingsApi.Contract.V1.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V1
{
    public class UpdateHearingRequestValidation : AbstractValidator<UpdateHearingRequest>
    {
        public static readonly string NoHearingVenueNameErrorMessage = "Hearing name cannot not be blank";
        public static readonly string ScheduleDateTimeInPastErrorMessage = "ScheduledDateTime cannot be in the past";
        public static readonly string NoScheduleDurationErrorMessage = "Schedule duration must be greater than 0";
        public static readonly string NoUpdatedByErrorMessage = "UpdatedBy is missing";

        public UpdateHearingRequestValidation()
        {
            RuleFor(x => x.HearingVenueName)
                .NotEmpty().WithMessage(NoHearingVenueNameErrorMessage);

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