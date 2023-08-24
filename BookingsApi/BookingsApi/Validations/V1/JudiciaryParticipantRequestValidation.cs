using BookingsApi.Contract.V1.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V1
{
    public class JudiciaryParticipantRequestValidation : AbstractValidator<JudiciaryParticipantRequest>
    {
        public const string NoPersonalCodeErrorMessage = "Personal code is required";
        public const string NoDisplayNameErrorMessage = "Display name is required";

        public JudiciaryParticipantRequestValidation()
        {
            RuleFor(x => x.PersonalCode).NotEmpty().WithMessage(NoPersonalCodeErrorMessage);
            RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameErrorMessage);
        }
    }
}
