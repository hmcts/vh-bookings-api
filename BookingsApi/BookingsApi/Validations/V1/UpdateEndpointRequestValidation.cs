using BookingsApi.Contract.V1.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V1
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
