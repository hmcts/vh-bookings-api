using BookingsApi.Contract.V1.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V1
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
