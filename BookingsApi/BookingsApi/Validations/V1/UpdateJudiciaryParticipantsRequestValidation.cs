using BookingsApi.Contract.V1.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V1
{
    public class UpdateJudiciaryParticipantsRequestValidation : AbstractValidator<UpdateJudiciaryParticipantsRequest>
    {
        public UpdateJudiciaryParticipantsRequestValidation()
        {
            RuleForEach(x => x.NewJudiciaryParticipants)
                .SetValidator(new JudiciaryParticipantRequestValidation());
            
            RuleForEach(x => x.ExistingJudiciaryParticipants)
                .SetValidator(new UpdateJudiciaryParticipantRequestValidation());
        }
    }
}
