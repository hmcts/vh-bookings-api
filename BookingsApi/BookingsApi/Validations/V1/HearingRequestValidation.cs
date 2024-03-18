using BookingsApi.Contract.V1.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V1
{
    public class HearingRequestValidation : AbstractValidator<HearingRequest>
    {
        public HearingRequestValidation()
        {
            RuleFor(x => x.HearingVenueName)
                .NotEmpty().WithMessage(UpdateHearingRequestValidation.NoHearingVenueNameErrorMessage);

            RuleFor(x => x.ScheduledDateTime).Custom((dateTime, context) =>
            {
                if (dateTime < DateTime.UtcNow)
                {
                    context.AddFailure(UpdateHearingRequestValidation.ScheduleDateTimeInPastErrorMessage);
                }
            });

            RuleFor(x => x.ScheduledDuration)
                .GreaterThan(0).WithMessage(UpdateHearingRequestValidation.NoScheduleDurationErrorMessage);

            RuleFor(x => x.CaseNumber)
                .NotEmpty().WithMessage(CaseRequestValidation.CaseNumberMessage);
            
            RuleFor(x => x.Participants)
                .SetValidator(new UpdateHearingParticipantsRequestValidation());
            
            RuleFor(x => x.Endpoints)
                .SetValidator(new UpdateHearingEndpointsRequestValidation());
        }
    }
}
