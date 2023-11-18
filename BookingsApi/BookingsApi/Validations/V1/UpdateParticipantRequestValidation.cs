using BookingsApi.Contract.V1.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V1;

public class UpdateParticipantRequestValidation : AbstractValidator<UpdateParticipantRequest>
{
    public static readonly string NoDisplayNameErrorMessage = "Display name is required";

    public UpdateParticipantRequestValidation()
    {
      RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameErrorMessage);
      RuleFor(x => x.FirstName).Matches(ParticipantRequestValidation.NameRegex).WithMessage(ParticipantRequestValidation.FirstNameDoesntMatchRegex);
      RuleFor(x => x.LastName).Matches(ParticipantRequestValidation.NameRegex).WithMessage(ParticipantRequestValidation.LastNameDoesntMatchRegex);
    }
}
