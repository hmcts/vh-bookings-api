using BookingsApi.Contract.V2.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V2
{
    public class UpdateJudiciaryParticipantRequestValidationV2 : AbstractValidator<UpdateJudiciaryParticipantRequestV2>
    {
        public const string NoDisplayNameErrorMessage = "Display name is required";

        public UpdateJudiciaryParticipantRequestValidationV2()
        {
            RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameErrorMessage);
            RuleFor(x => x.HearingRoleCode).IsInEnum();
        }
    }
}
