using BookingsApi.Contract.Requests;
using FluentValidation;

namespace BookingsApi.Validations
{
    public class JudiciaryLeaverPersonRequestValidation : AbstractValidator<JudiciaryPersonRequest>
    {
        public static readonly string NoPersonalCodeErrorMessage = "Personal code can not be null or empty";

        public JudiciaryLeaverPersonRequestValidation()
        {
            RuleFor(x => x.PersonalCode).NotEmpty().WithMessage(NoPersonalCodeErrorMessage);
        }
    }
}
