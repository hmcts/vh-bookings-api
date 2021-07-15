using BookingsApi.Contract.Requests;
using FluentValidation;

namespace BookingsApi.Validations
{
    public class JudiciaryLeaverRequestValidation : AbstractValidator<JudiciaryLeaverRequest>
    {
        private const string IdErrorMessage = "Id can not be null or empty";
        private const string LeaverErrorMessage = "Leaver flag cannot be null or empty";

        public JudiciaryLeaverRequestValidation()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(IdErrorMessage);
            RuleFor(x => x.Leaver).NotEmpty().WithMessage(LeaverErrorMessage);
        }
    }
}