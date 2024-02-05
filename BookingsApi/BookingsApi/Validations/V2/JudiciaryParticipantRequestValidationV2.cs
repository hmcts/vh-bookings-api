using BookingsApi.Contract.V2.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V2
{
    public class JudiciaryParticipantRequestValidationV2 : AbstractValidator<JudiciaryParticipantRequestV2>
    {
        public const string NoPersonalCodeErrorMessage = "Personal code is required";
        public const string NoDisplayNameErrorMessage = "Display name is required";

        public JudiciaryParticipantRequestValidationV2()
        {
            RuleFor(x => x.PersonalCode).NotEmpty().WithMessage(NoPersonalCodeErrorMessage);
            RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameErrorMessage);
            RuleFor(x => x.HearingRoleCode).NotEmpty().IsInEnum();
        }
    }
}
