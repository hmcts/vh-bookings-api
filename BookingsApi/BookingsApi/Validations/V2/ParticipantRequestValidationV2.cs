using BookingsApi.Contract.V2.Requests;
using BookingsApi.Domain.Constants;
using FluentValidation;

namespace BookingsApi.Validations.V2
{
    public class ParticipantRequestValidationV2 : AbstractValidator<ParticipantRequestV2>
    {
        private const string NameRegex = "^(\\w+(?:\\w|[\\s'._-](?![\\s'._-]))*\\w+)$";
        public static readonly string FirstNameDoesntMatchRegex = "First name must match regular expression";
        public static readonly string LastNameDoesntMatchRegex = "Last name must match regular expression";
        public static readonly string NoDisplayNameErrorMessage = "Display name is required";
        public static readonly string InvalidDisplayNameErrorMessage = "Display name will accept upto 255 alphanumeric characters, spaces, and the following special characters: ',._-";
        public static readonly string NoHearingRoleCodeErrorMessage = "Hearing role is required";
        public static readonly string NoFirstNameErrorMessage = "First name is required";
        public static readonly string NoLastNameErrorMessage = "Last name is required";
        public static readonly string NoUsernameErrorMessage = "Username is required";
        public static readonly string NoTelephoneNumberErrorMessage = "Telephone Number is required";
        public static readonly string NoContactEmailErrorMessage = "Contact Email is required";
        public static readonly string InvalidContactEmailErrorMessage = "Contact Email is Invalid";
        public static readonly string InvalidJudgeUsernameErrorMessage = "Judge username is Invalid";

        public ParticipantRequestValidationV2()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(NoFirstNameErrorMessage);
            RuleFor(x => x.FirstName).Matches(NameRegex).WithMessage(FirstNameDoesntMatchRegex);
            RuleFor(x => x.LastName).NotEmpty().WithMessage(NoLastNameErrorMessage);
            RuleFor(x => x.LastName).Matches(NameRegex).WithMessage(LastNameDoesntMatchRegex);
            RuleFor(x => x.ContactEmail).NotEmpty().WithMessage(NoContactEmailErrorMessage).Must(x => x.IsValidEmail()).WithMessage(InvalidContactEmailErrorMessage);

            RuleFor(x => x.Username).NotEmpty().When(x => x.HearingRoleCode == HearingRoleCodes.Judge).WithMessage(NoUsernameErrorMessage);
            RuleFor(x => x.Username).Must(x => x.IsValidEmail()).When(x => x.HearingRoleCode == HearingRoleCodes.Judge && !string.IsNullOrEmpty(x.Username))
                .WithMessage(InvalidJudgeUsernameErrorMessage);

            var regex = "^([-A-Za-z0-9 ',._]){1,255}$";
            RuleFor(x => x.DisplayName)
                .NotEmpty().WithMessage(NoDisplayNameErrorMessage)
                .Matches(regex).WithMessage(InvalidDisplayNameErrorMessage);
            RuleFor(x => x.HearingRoleCode).NotEmpty().WithMessage(NoHearingRoleCodeErrorMessage);
            RuleFor(x => x.TelephoneNumber).NotEmpty().When(x => x.HearingRoleCode != HearingRoleCodes.Judge).WithMessage(NoTelephoneNumberErrorMessage);
        }
    }
}