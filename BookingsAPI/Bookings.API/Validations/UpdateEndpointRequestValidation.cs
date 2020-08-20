using Bookings.Api.Contract.Requests;
using FluentValidation;

namespace Bookings.API.Validations
{
    public class UpdateEndpointRequestValidation : AbstractValidator<UpdateEndpointRequest>
    {
        public const string NoDisplayNameError = "DisplayName is required";

        public UpdateEndpointRequestValidation()
        {
            RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameError);
        }
    }
}
