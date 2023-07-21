using BookingsApi.Contract.V1.Requests;
using FluentValidation;

namespace BookingsApi.Validations
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
