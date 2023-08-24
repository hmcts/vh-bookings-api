using BookingsApi.Contract.V1.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V1
{
    public class AddJudiciaryParticipantsToHearingRequestValidation : AbstractValidator<AddJudiciaryParticipantsRequest>
    {
        public const string NoParticipantsErrorMessage = "Please provide at least one participant";

        public AddJudiciaryParticipantsToHearingRequestValidation()
        {
            RuleFor(x => x.Participants)
                .NotEmpty().WithMessage(NoParticipantsErrorMessage);

            RuleForEach(x => x.Participants)
                .SetValidator(new JudiciaryParticipantRequestValidation());
        }
    }
}
