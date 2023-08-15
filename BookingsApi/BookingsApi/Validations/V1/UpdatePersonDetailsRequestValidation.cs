using BookingsApi.Contract.V1.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V1
{
    public class UpdatePersonDetailsRequestValidation : AbstractValidator<UpdatePersonDetailsRequest>
    {
        public static readonly string NoFirstNameErrorMessage = "First name is required";
        public static readonly string NoLastNameErrorMessage = "Last name is required";
        public static readonly string NoUsernameErrorMessage = "Username is required";
        
        public UpdatePersonDetailsRequestValidation()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(NoFirstNameErrorMessage);
            RuleFor(x => x.LastName).NotEmpty().WithMessage(NoLastNameErrorMessage);
            RuleFor(x => x.Username).NotEmpty().WithMessage(NoUsernameErrorMessage);
        }
    }
}