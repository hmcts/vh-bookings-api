using BookingsApi.Contract.V2.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V2
{
    public class HearingRequestInputValidationV2 : AbstractValidator<HearingRequestV2>
    {
        public const string NoHearingVenueCodeErrorMessage = "Hearing code cannot not be blank";
        public const string ScheduleDateTimeInPastErrorMessage = "ScheduledDateTime cannot be in the past";
        public const string NoScheduleDurationErrorMessage = "Schedule duration must be greater than 0";

        public HearingRequestInputValidationV2()
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
            
            RuleFor(x => x.Participants)
                .SetValidator(new UpdateHearingParticipantsRequestInputValidationV2());

            RuleFor(x => x.Endpoints)
                .SetValidator(new UpdateHearingEndpointsRequestValidationV2());

            RuleFor(x => x.JudiciaryParticipants)
                .SetValidator(new UpdateJudiciaryParticipantsRequestValidationV2());
        }
    }
}
