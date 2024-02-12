using BookingsApi.Contract.V1.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V1
{
    public class HearingRequestValidation : AbstractValidator<HearingRequest>
    {
        public HearingRequestValidation()
        {
            RuleFor(x => x.Participants)
                .SetValidator(new UpdateHearingParticipantsRequestValidation());
            
            RuleFor(x => x.Endpoints)
                .SetValidator(new UpdateHearingEndpointsRequestValidation());
        }
    }
}
