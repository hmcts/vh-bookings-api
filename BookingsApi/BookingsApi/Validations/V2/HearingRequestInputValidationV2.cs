using BookingsApi.Contract.V2.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V2
{
    public class HearingRequestInputValidationV2 : AbstractValidator<HearingRequestV2>
    {
        public HearingRequestInputValidationV2()
        {
            RuleFor(x => x.HearingVenueCode)
                .NotEmpty().WithMessage(UpdateHearingRequestValidationV2.NoHearingVenueCodeErrorMessage);

            RuleFor(x => x.ScheduledDuration)
                .GreaterThan(0).WithMessage(UpdateHearingRequestValidationV2.NoScheduleDurationErrorMessage);
            
            RuleFor(x => x.CaseNumber)
                .NotEmpty().WithMessage(CaseRequestValidationV2.CaseNumberMessage);
            
            RuleFor(x => x.Participants)
                .SetValidator(new UpdateHearingParticipantsRequestInputValidationV2());

            RuleFor(x => x.Endpoints)
                .SetValidator(new UpdateHearingEndpointsRequestValidationV2());

            RuleFor(x => x.JudiciaryParticipants)
                .SetValidator(new UpdateJudiciaryParticipantsRequestValidationV2());
        }
    }
}
