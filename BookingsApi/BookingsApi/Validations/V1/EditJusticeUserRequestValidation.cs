using BookingsApi.Contract.V1.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V1;

public class EditJusticeUserRequestValidation : AbstractValidator<EditJusticeUserRequest>
{
    public const string NoUsernameErrorMessage = "Username is required";
    public const string NoIdErrorMessage = "Id is required";
    public const string NoRoleErrorMessage = "Role is required";
    
    public EditJusticeUserRequestValidation()
    {
        RuleFor(x => x.Id).NotNull().WithMessage(NoIdErrorMessage);
        RuleFor(x => x.Username).NotEmpty().WithMessage(NoUsernameErrorMessage);
        RuleForEach(x => x.Roles).IsInEnum().WithMessage(NoRoleErrorMessage);
    }
}