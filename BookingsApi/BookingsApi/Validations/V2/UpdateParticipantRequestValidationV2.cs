using BookingsApi.Contract.V2.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V2
{
    public class UpdateParticipantRequestValidationV2 : AbstractValidator<UpdateParticipantRequestV2>
    {
        public static readonly string NoDisplayNameErrorMessage = "Display name is required";
        public static readonly string NoParticipantIdErrorMessage = "ParticipantId is required";

        public UpdateParticipantRequestValidationV2()
        {
            RuleFor(x => x.ParticipantId).NotEmpty().WithMessage(NoParticipantIdErrorMessage);
            RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameErrorMessage);
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(ParticipantValidationV2.NoFirstNameErrorMessage);
            RuleFor(x => x.FirstName).Matches(ParticipantValidationV2.NameRegex).WithMessage(ParticipantValidationV2.FirstNameDoesntMatchRegex);
            RuleFor(x => x.LastName).NotEmpty().WithMessage(ParticipantValidationV2.NoLastNameErrorMessage);
            RuleFor(x => x.LastName).Matches(ParticipantValidationV2.NameRegex).WithMessage(ParticipantValidationV2.LastNameDoesntMatchRegex);
            RuleFor(x => x.ContactEmail).Must(x => x.IsValidEmail()).WithMessage(ParticipantRequestValidationV2.InvalidContactEmailErrorMessage).When(x => !string.IsNullOrEmpty(x.ContactEmail));
        }
    }
}