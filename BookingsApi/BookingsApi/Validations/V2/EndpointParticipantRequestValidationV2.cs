using BookingsApi.Contract.V2.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V2;

public class EndpointParticipantsRequestValidationV2 : AbstractValidator<EndpointParticipantsRequestV2>
{
    public static readonly string InvalidContactEmailError = "Contact Email is Invalid";

    public EndpointParticipantsRequestValidationV2()
    {
        // regex where only alphanumeric and underscore are allowed and maximum 255 characters
        RuleFor(x => x.ContactEmail)
            .Must(x => x.IsValidEmail())
            .When(x => x.ContactEmail != null)
            .WithMessage(InvalidContactEmailError);
    }
}