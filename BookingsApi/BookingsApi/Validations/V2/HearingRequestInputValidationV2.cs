using BookingsApi.Contract.V2.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V2
{
    public class HearingRequestInputValidationV2 : AbstractValidator<HearingRequestV2>
    {
        public HearingRequestInputValidationV2()
        {
            RuleFor(x => x.Participants)
                .SetValidator(new UpdateHearingParticipantsRequestInputValidationV2());

            RuleFor(x => x.Endpoints)
                .SetValidator(new UpdateHearingEndpointsRequestValidationV2());

            RuleFor(x => x.JudiciaryParticipants)
                .SetValidator(new UpdateJudiciaryParticipantsRequestValidationV2());
        }
    }
}
