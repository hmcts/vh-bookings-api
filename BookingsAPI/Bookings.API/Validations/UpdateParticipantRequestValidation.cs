using Bookings.Api.Contract.Requests;
using FluentValidation;

namespace Bookings.API.Validations
{
    public class UpdarteParticipantRequestValidation : AbstractValidator<ParticipantRequest>
    {
        public static readonly string NoDisplayNameErrorMessage = "Display name is required";
        public static readonly string NoContactEmailErrorMessage = "Contact Email is required";

        public UpdarteParticipantRequestValidation()
        {
            RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameErrorMessage);
        }
    }
}