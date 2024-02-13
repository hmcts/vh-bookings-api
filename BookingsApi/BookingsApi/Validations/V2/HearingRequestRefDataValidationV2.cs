using BookingsApi.Contract.V2.Requests;
using BookingsApi.Validations.Common;

namespace BookingsApi.Validations.V2
{
    public class HearingRequestRefDataValidationV2 : RefDataInputValidatorValidator<HearingRequestV2>
    {
        public HearingRequestRefDataValidationV2(List<HearingRole> hearingRoles)
        {
            RuleFor(x => x.Participants)
                .SetValidator(new UpdateHearingParticipantsRequestRefDataValidationV2(hearingRoles));
        }
    }
}
