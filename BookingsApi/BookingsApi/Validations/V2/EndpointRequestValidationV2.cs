using BookingsApi.Contract.V2.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V2;

public class EndpointRequestValidationV2 : AbstractValidator<EndpointRequestV2>
{
    public static readonly string InvalidDisplayNameErrorMessage = "Display name will accept upto 255 alphanumeric characters, spaces, and the following special characters: ',._-";
    public static readonly string InvalidDefenceAdvocateContactEmailError = "Defence Advocate Contact Email is Invalid";

    public EndpointRequestValidationV2()
    {
        // regex where only alphanumeric and underscore are allowed and maximum 255 characters
        var regex = @"^[\p{L}\p{N}\s',._-]+${1,255}$";
            
        RuleFor(x => x.DisplayName).NotEmpty().Matches(regex).WithMessage(InvalidDisplayNameErrorMessage);
        RuleFor(x => x.DefenceAdvocateContactEmail).Must(x => x.IsValidEmail()).When(x => x.DefenceAdvocateContactEmail != null)
            .WithMessage(InvalidDefenceAdvocateContactEmailError);
    }
}