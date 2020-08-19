using Bookings.Api.Contract.Requests;
using FluentValidation;

namespace Bookings.API.Validations
{
    public class AddEndpointRequestValidation : AbstractValidator<AddEndpointRequest>
    {
        public const string NoDisplayNameError = "DisplayName is required";
        
        public AddEndpointRequestValidation()
        {
            RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameError);
        }
    }
}
