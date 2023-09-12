using BookingsApi.Contract.V2.Requests;
using BookingsApi.Validations.Common;
using FluentValidation;

namespace BookingsApi.Validations.V2
{
    public class UpdateHearingParticipantsRequestInputValidationV2 : AbstractValidator<UpdateHearingParticipantsRequestV2>
    {
        public static readonly string NoParticipantsErrorMessage = "Please provide at least one participant";

        public UpdateHearingParticipantsRequestInputValidationV2()
        {
            RuleFor(x => x.NewParticipants.Count + x.ExistingParticipants.Count + x.RemovedParticipantIds.Count)
                .GreaterThan(0).WithMessage(NoParticipantsErrorMessage);

            RuleForEach(x => x.NewParticipants)
                .SetValidator(new ParticipantRequestValidationV2());

            RuleForEach(x => x.ExistingParticipants)
                .SetValidator(new UpdateParticipantRequestValidationV2());
        }
    }
}