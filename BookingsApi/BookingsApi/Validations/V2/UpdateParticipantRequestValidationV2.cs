using BookingsApi.Contract.V2.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V2
{
    public class UpdateParticipantRequestValidationV2 : AbstractValidator<UpdateParticipantRequestV2>
    {
        public static readonly string NoDisplayNameErrorMessage = "Display name is required";
        public static readonly string NoParticipantIdErrorMessage = "ParticipantId is required";

        public UpdateParticipantRequestValidationV2()
        {
            RuleFor(x => x.ParticipantId).NotEmpty().WithMessage(NoParticipantIdErrorMessage);
            RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameErrorMessage);
        }
    }
}