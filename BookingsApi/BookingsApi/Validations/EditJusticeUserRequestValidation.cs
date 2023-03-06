using BookingsApi.Contract.Requests;
using FluentValidation;

namespace BookingsApi.Validations;

public class EditJusticeUserRequestValidation : AbstractValidator<EditJusticeUserRequest>
{
    public const string NoUsernameErrorMessage = "Username is required";
    public const string NoIdErrorMessage = "Id is required";
    public const string NoRoleErrorMessage = "Role is required";
    
    public EditJusticeUserRequestValidation()
    {
        RuleFor(x => x.Id).NotNull().WithMessage(NoIdErrorMessage);
        RuleFor(x => x.Username).NotEmpty().WithMessage(NoUsernameErrorMessage);
        RuleFor(x => x.Role).IsInEnum().WithMessage(NoRoleErrorMessage);
    }
}