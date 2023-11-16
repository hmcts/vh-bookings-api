using BookingsApi.Contract.V1.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V1
{
    public class JudiciaryParticipantRequestValidation : AbstractValidator<JudiciaryParticipantRequest>
    {
        public const string NoPersonalCodeErrorMessage = "Personal code is required";
        public const string NoDisplayNameErrorMessage = "Display name is required";
        public const string InvalidOptionalContactEmailErrorMessage = "Optional contact email is invalid";

        public JudiciaryParticipantRequestValidation()
        {
            RuleFor(x => x.PersonalCode).NotEmpty().WithMessage(NoPersonalCodeErrorMessage);
            RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameErrorMessage);
            RuleFor(x => x.HearingRoleCode).NotEmpty().IsInEnum();
            RuleFor(x => x.OptionalContactEmail)
                .Must(x => x.IsValidEmail())
                .When(x => !string.IsNullOrEmpty(x.OptionalContactEmail))
                .WithMessage(InvalidOptionalContactEmailErrorMessage);
        }
    }
}
