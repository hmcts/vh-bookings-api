using BookingsApi.Contract.Requests;
using FluentValidation;

namespace BookingsApi.Validations
{
    public class JudiciaryLeaverPersonRequestValidation : AbstractValidator<JudiciaryPersonRequest>
    {
        public static readonly string NoIdErrorMessage = "Id can not be null or empty";

        public JudiciaryLeaverPersonRequestValidation()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(NoIdErrorMessage);
        }
    }
}
