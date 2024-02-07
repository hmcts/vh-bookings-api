using BookingsApi.Contract.V2.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V2
{
    public class UpdateJudiciaryParticipantsRequestValidationV2 : AbstractValidator<UpdateJudiciaryParticipantsRequestV2>
    {
        public UpdateJudiciaryParticipantsRequestValidationV2()
        {
            RuleForEach(x => x.NewJudiciaryParticipants)
                .SetValidator(new JudiciaryParticipantRequestValidationV2());
            
            RuleForEach(x => x.ExistingJudiciaryParticipants)
                .SetValidator(new UpdateJudiciaryParticipantRequestValidationV2());
        }
    }
}
