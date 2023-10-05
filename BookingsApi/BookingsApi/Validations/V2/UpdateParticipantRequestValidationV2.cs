using BookingsApi.Contract.V2.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V2
{
    public class UpdateParticipantRequestValidationV2 : AbstractValidator<UpdateParticipantRequestV2>
    {
        private const string NameRegex = "^(\\w+(?:\\w|[\\s'._-](?![\\s'._-]))*\\w+)$";
        public static readonly string FirstNameDoesntMatchRegex = "First name must match regular expression";
        public static readonly string LastNameDoesntMatchRegex = "Last name must match regular expression";
        public static readonly string NoDisplayNameErrorMessage = "Display name is required";
        public static readonly string NoParticipantIdErrorMessage = "ParticipantId is required";
        public static readonly string NoFirstNameErrorMessage = "First name is required";
        public static readonly string NoLastNameErrorMessage = "Last name is required";

        public UpdateParticipantRequestValidationV2()
        {
            RuleFor(x => x.ParticipantId).NotEmpty().WithMessage(NoParticipantIdErrorMessage);
            RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameErrorMessage);
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(NoFirstNameErrorMessage);
            RuleFor(x => x.FirstName).Matches(NameRegex).WithMessage(FirstNameDoesntMatchRegex);
            RuleFor(x => x.LastName).NotEmpty().WithMessage(NoLastNameErrorMessage);
            RuleFor(x => x.LastName).Matches(NameRegex).WithMessage(LastNameDoesntMatchRegex);
        }
    }
}