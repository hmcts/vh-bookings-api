using BookingsApi.Contract.Requests;
using FluentValidation;

namespace BookingsApi.Validations
{
    public class JudiciaryLeaverRequestValidation : AbstractValidator<JudiciaryLeaverRequest>
    {
        private const string PersonalCodeErrorMessage = "Personal code can not be null or empty";
        private const string LeaverErrorMessage = "Leaver flag cannot be null or empty";

        public JudiciaryLeaverRequestValidation()
        {
            RuleFor(x => x.PersonalCode).NotEmpty().WithMessage(PersonalCodeErrorMessage);
            RuleFor(x => x.Leaver).NotEmpty().WithMessage(LeaverErrorMessage);
        }
    }
}