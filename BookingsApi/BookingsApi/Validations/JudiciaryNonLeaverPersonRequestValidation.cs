using BookingsApi.Contract.V1.Requests;
using FluentValidation;

namespace BookingsApi.Validations
{
    public class JudiciaryNonLeaverPersonRequestValidation : AbstractValidator<JudiciaryPersonRequest>
    {
        public static readonly string NoPersonalCodeErrorMessage = "Personal code can not be null or empty";
        public static readonly string NoFirstNameErrorMessage = "First name / known as is required";
        public static readonly string NoSurnameNameErrorMessage = "Surname is required";
        public static readonly string NoEmailErrorMessage = "Email is required";
        
        public JudiciaryNonLeaverPersonRequestValidation()
        {
            RuleFor(x => x.PersonalCode).NotEmpty().WithMessage(NoPersonalCodeErrorMessage);
            RuleFor(x => x.KnownAs).NotEmpty().WithMessage(NoFirstNameErrorMessage);
            RuleFor(x => x.Surname).NotEmpty().WithMessage(NoSurnameNameErrorMessage);
            RuleFor(x => x.Email).NotEmpty().WithMessage(NoEmailErrorMessage);
        }
    }
}