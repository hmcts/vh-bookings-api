using BookingsApi.Contract.V1.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V1
{
    public class UpdateJudiciaryParticipantRequestValidation : AbstractValidator<UpdateJudiciaryParticipantRequest>
    {
        public const string NoDisplayNameErrorMessage = "Display name is required";

        public UpdateJudiciaryParticipantRequestValidation()
        {
            RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameErrorMessage);
            RuleFor(x => x.HearingRoleCode).IsInEnum();
        }
    }
}
