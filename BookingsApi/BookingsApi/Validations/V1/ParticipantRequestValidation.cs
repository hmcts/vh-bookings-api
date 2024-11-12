using BookingsApi.Contract.V1.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V1
{
    public class ParticipantRequestValidation : AbstractValidator<ParticipantRequest>
    {
        public const string NameRegex = "^(\\w+(?:\\w|[\\s'._-](?![\\s'._-]))*\\w+)$";
        public const string FirstNameDoesntMatchRegex = "First name must match regular expression";
        public const string LastNameDoesntMatchRegex = "Last name must match regular expression";
        public const string NoDisplayNameErrorMessage = "Display name is required";
        public const string NoCaseRoleNameErrorMessage = "Case role is required";
        public const string NoHearingRoleNameErrorMessage = "Hearing role is required";
        public const string NoFirstNameErrorMessage = "First name is required";
        public const string NoLastNameErrorMessage = "Last name is required";
        public const string NoUsernameErrorMessage = "Username is required";
        public const string NoTelephoneNumberErrorMessage = "Telephone Number is required";
        public const string NoContactEmailErrorMessage = "Contact Email is required";
        public const string InvalidContactEmailErrorMessage = "Contact Email is Invalid";
        public const string InvalidJudgeUsernameErrorMessage = "Judge username is Invalid";

        public ParticipantRequestValidation()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(NoFirstNameErrorMessage);
            RuleFor(x => x.FirstName).Matches(NameRegex).WithMessage(FirstNameDoesntMatchRegex);
            RuleFor(x => x.LastName).NotEmpty().WithMessage(NoLastNameErrorMessage);
            RuleFor(x => x.LastName).Matches(NameRegex).WithMessage(LastNameDoesntMatchRegex);
            RuleFor(x => x.ContactEmail).NotEmpty().WithMessage(NoContactEmailErrorMessage).Must(x => x.IsValidEmail()).WithMessage(InvalidContactEmailErrorMessage);

            RuleFor(x => x.Username).NotEmpty().When(x => x.HearingRoleName.ToLowerInvariant() == "Judge".ToLowerInvariant()).WithMessage(NoUsernameErrorMessage);
            RuleFor(x => x.Username).Must(x => x.IsValidEmail()).When(x => x.HearingRoleName.ToLowerInvariant() == "Judge".ToLowerInvariant() && !string.IsNullOrEmpty(x.Username))
                .WithMessage(InvalidJudgeUsernameErrorMessage);

            RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameErrorMessage);
            RuleFor(x => x.CaseRoleName).NotEmpty().WithMessage(NoCaseRoleNameErrorMessage);
            RuleFor(x => x.HearingRoleName).NotEmpty().WithMessage(NoHearingRoleNameErrorMessage);
            RuleFor(x => x.TelephoneNumber).NotEmpty().When(x => x.HearingRoleName.ToLowerInvariant() != "Judge".ToLowerInvariant() && 
                x.HearingRoleName.ToLowerInvariant() != "Panel Member".ToLowerInvariant()).WithMessage(NoTelephoneNumberErrorMessage);
        }
    }
}