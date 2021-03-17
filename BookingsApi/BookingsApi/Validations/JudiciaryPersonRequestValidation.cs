using BookingsApi.Contract.Requests;
using FluentValidation;

namespace BookingsApi.Validations
{
    public class JudiciaryPersonRequestValidation : AbstractValidator<JudiciaryPersonRequest>
    {
        public static readonly string NoIdErrorMessage = "Id can not be null or empty";
        public static readonly string NoFirstNameErrorMessage = "First name / known as is required";
        public static readonly string NoSurnameNameErrorMessage = "Surname is required";
        public static readonly string NoEmailErrorMessage = "Email is required";
        
        public JudiciaryPersonRequestValidation()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(NoIdErrorMessage);
            RuleFor(x => x.KnownAs).NotEmpty().WithMessage(NoFirstNameErrorMessage);
            RuleFor(x => x.Surname).NotEmpty().WithMessage(NoSurnameNameErrorMessage);
            RuleFor(x => x.Email).NotEmpty().WithMessage(NoEmailErrorMessage);
        }
    }
}