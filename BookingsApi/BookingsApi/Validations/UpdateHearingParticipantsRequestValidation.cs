using BookingsApi.Contract.V1.Requests;
using FluentValidation;

namespace BookingsApi.Validations
{
    public class UpdateHearingParticipantsRequestValidation : AbstractValidator<UpdateHearingParticipantsRequest>
    {
        public static readonly string NoParticipantsErrorMessage = "Please provide at least one participant";

        public UpdateHearingParticipantsRequestValidation()
        {
            RuleFor(x => x.NewParticipants.Count + x.ExistingParticipants.Count + x.RemovedParticipantIds.Count)
                .NotEmpty().WithMessage(NoParticipantsErrorMessage);

            RuleForEach(x => x.NewParticipants)
                .SetValidator(new ParticipantRequestValidation());

            RuleForEach(x => x.ExistingParticipants)
                .SetValidator(new UpdateParticipantRequestValidation());
        }
    }
}