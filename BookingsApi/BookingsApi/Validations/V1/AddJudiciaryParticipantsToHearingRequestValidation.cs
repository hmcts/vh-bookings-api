using BookingsApi.Contract.V1.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V1
{
    public class AddJudiciaryParticipantsToHearingRequestValidation : AbstractValidator<List<JudiciaryParticipantRequest>>
    {
        public const string NoParticipantsErrorMessage = "Please provide at least one participant";

        public AddJudiciaryParticipantsToHearingRequestValidation()
        {
            RuleFor(x => x).NotEmpty().WithMessage(NoParticipantsErrorMessage);
            RuleForEach(judiciaryParticipants =>  judiciaryParticipants).SetValidator(new JudiciaryParticipantRequestValidation());
        }
    }
}
