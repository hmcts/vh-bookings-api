using BookingsApi.Contract.V2.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V2
{
    public class UpdateHearingParticipantsRequestInputValidationV2 : AbstractValidator<UpdateHearingParticipantsRequestV2>
    {
        public const string NoParticipantsErrorMessage = "Please provide at least one participant";

        public UpdateHearingParticipantsRequestInputValidationV2(bool checkParticipantCount = true)
        {
            if (checkParticipantCount)
            {
                RuleFor(x => x.NewParticipants.Count + x.ExistingParticipants.Count + x.RemovedParticipantIds.Count)
                    .GreaterThan(0).WithMessage(NoParticipantsErrorMessage);
            }

            RuleForEach(x => x.NewParticipants)
                .SetValidator(new ParticipantRequestValidationV2());

            RuleForEach(x => x.ExistingParticipants)
                .SetValidator(new UpdateParticipantRequestValidationV2());
        }
    }
}