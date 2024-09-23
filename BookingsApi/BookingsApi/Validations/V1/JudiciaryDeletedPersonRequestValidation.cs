using BookingsApi.Contract.V1.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V1
{
    public class JudiciaryDeletedPersonRequestValidation : AbstractValidator<JudiciaryPersonRequest>
    {
        public const string NoPersonalCodeErrorMessage = "Personal code can not be null or empty";

        public JudiciaryDeletedPersonRequestValidation()
        {
            RuleFor(x => x.PersonalCode).NotEmpty().WithMessage(NoPersonalCodeErrorMessage);
        }
    }
}
