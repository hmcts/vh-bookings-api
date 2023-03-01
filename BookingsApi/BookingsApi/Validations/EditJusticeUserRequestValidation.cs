using BookingsApi.Contract.Requests;
using FluentValidation;

namespace BookingsApi.Validations;

public class EditJusticeUserRequestValidation : AbstractValidator<EditJusticeUserRequest>
{
    public static readonly string NoUsernameErrorMessage = "Username is required";
    public EditJusticeUserRequestValidation()
    {
        RuleFor(x => x.Id).NotNull();
        RuleFor(x => x.Username).NotEmpty().WithMessage(NoUsernameErrorMessage);
        RuleFor(x => x.Role).IsInEnum();
    }
}