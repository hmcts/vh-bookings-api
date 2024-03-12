using BookingsApi.Contract.V2.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V2
{
    public class HearingRequestInputValidationV2 : AbstractValidator<HearingRequestV2>
    {
        public HearingRequestInputValidationV2()
        {
            RuleFor(x => x.Participants)
                .SetValidator(x => new UpdateHearingParticipantsRequestInputValidationV2(checkParticipantCount: GetJudiciaryParticipantCount(x) == 0));

            RuleFor(x => x.Endpoints)
                .SetValidator(new UpdateHearingEndpointsRequestValidationV2());

            RuleFor(x => x.JudiciaryParticipants)
                .SetValidator(new UpdateJudiciaryParticipantsRequestValidationV2());
        }
        
        private static int GetJudiciaryParticipantCount(HearingRequestV2 request)
        {
            if (request.JudiciaryParticipants == null)
            {
                return 0;
            }
            
            return request.JudiciaryParticipants.NewJudiciaryParticipants.Count + 
                   request.JudiciaryParticipants.ExistingJudiciaryParticipants.Count + 
                   request.JudiciaryParticipants.RemovedJudiciaryParticipantPersonalCodes.Count;
        }
    }
}
