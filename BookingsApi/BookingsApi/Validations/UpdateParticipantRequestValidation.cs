using BookingsApi.Contract.Requests;
using FluentValidation;

namespace BookingsApi.Validations
{
    public class UpdateParticipantRequestValidation : AbstractValidator<UpdateParticipantRequest>
    {
        public static readonly string NoDisplayNameErrorMessage = "Display name is required";

        public UpdateParticipantRequestValidation()
        {
          RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameErrorMessage);
        }

        
    }
}