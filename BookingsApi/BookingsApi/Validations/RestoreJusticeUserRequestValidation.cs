using BookingsApi.Contract.V1.Requests;
using FluentValidation;

namespace BookingsApi.Validations;

public class RestoreJusticeUserRequestValidation : AbstractValidator<RestoreJusticeUserRequest>
{
    public const string NoUsernameErrorMessage = "Username is required";
    public const string NoIdErrorMessage = "Id is required";
    
    public RestoreJusticeUserRequestValidation()
    {
        RuleFor(x => x.Id).NotNull().WithMessage(NoIdErrorMessage);
        RuleFor(x => x.Username).NotEmpty().WithMessage(NoUsernameErrorMessage);
    }
}