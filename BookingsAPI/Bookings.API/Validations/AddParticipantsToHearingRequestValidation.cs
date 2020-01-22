using Bookings.Api.Contract.Requests;
using FluentValidation;

namespace Bookings.API.Validations
{
    public class AddParticipantsToHearingRequestValidation : AbstractValidator<AddParticipantsToHearingRequest>
    {
        public static readonly string NoParticipantsErrorMessage = "Please provide at least one participant";

        public AddParticipantsToHearingRequestValidation()
        {
            RuleFor(x => x.Participants)
                .NotEmpty().WithMessage(NoParticipantsErrorMessage);

            RuleForEach(x => x.Participants)
                .SetValidator(new ParticipantRequestValidation());
        }
    }
}