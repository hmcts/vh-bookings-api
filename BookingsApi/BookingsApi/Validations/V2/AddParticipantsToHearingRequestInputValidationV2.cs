using BookingsApi.Contract.V2.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V2
{
    public class AddParticipantsToHearingRequestInputValidationV2 : AbstractValidator<AddParticipantsToHearingRequestV2>
    {
        public static readonly string NoParticipantsErrorMessage = "Please provide at least one participant";

        public AddParticipantsToHearingRequestInputValidationV2()
        {
            RuleFor(x => x.Participants)
                .NotEmpty().WithMessage(NoParticipantsErrorMessage);

            RuleForEach(x => x.Participants)
                .SetValidator(new ParticipantRequestValidationV2());
        }
    }
}