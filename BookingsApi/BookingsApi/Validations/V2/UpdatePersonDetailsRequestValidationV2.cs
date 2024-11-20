using BookingsApi.Contract.V2.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V2;

public class UpdatePersonDetailsRequestValidationV2 : AbstractValidator<UpdatePersonDetailsRequestV2>
{
    public const string NoFirstNameErrorMessage = "First name is required";
    public const string NoLastNameErrorMessage = "Last name is required";
    public const string NoUsernameErrorMessage = "Username is required";

    public UpdatePersonDetailsRequestValidationV2()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage(NoFirstNameErrorMessage);
        RuleFor(x => x.LastName).NotEmpty().WithMessage(NoLastNameErrorMessage);
        RuleFor(x => x.Username).NotEmpty().WithMessage(NoUsernameErrorMessage);
    }
}
