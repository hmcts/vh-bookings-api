using BookingsApi.Contract.Requests;
using FluentValidation;

namespace BookingsApi.Validations
{
    public class ParticipantRequestValidation : AbstractValidator<ParticipantRequest>
    {
        private static readonly string DefaultRegion = "GB";
        private static readonly string _nameRegex = "^(\\w+(?:\\w|[\\s'._-](?![\\s'._-]))*\\w+)$";
        public static readonly string FirstNameDoesntMatchRegex = "First name must match regular expression";
        public static readonly string LastNameDoesntMatchRegex = "Last name must match regular expression";
        public static readonly string NoDisplayNameErrorMessage = "Display name is required";
        public static readonly string NoCaseRoleNameErrorMessage = "Case role is required";
        public static readonly string NoHearingRoleNameErrorMessage = "Hearing role is required";
        public static readonly string NoFirstNameErrorMessage = "First name is required";
        public static readonly string NoLastNameErrorMessage = "Last name is required";
        public static readonly string NoUsernameErrorMessage = "Username is required";
        public static readonly string NoTelephoneNumberErrorMessage = "Telephone Number is required";
        public static readonly string NoContactEmailErrorMessage = "Contact Email is required";
        public static readonly string InvalidContactEmailErrorMessage = "Contact Email is Invalid";
        public static readonly string InvalidJudgeUsernameErrorMessage = "Judge username is Invalid";

        public ParticipantRequestValidation()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(NoFirstNameErrorMessage);
            RuleFor(x => x.FirstName).Matches(_nameRegex).WithMessage(FirstNameDoesntMatchRegex);
            RuleFor(x => x.LastName).NotEmpty().WithMessage(NoLastNameErrorMessage);
            RuleFor(x => x.LastName).Matches(_nameRegex).WithMessage(LastNameDoesntMatchRegex);
            RuleFor(x => x.ContactEmail).NotEmpty().WithMessage(NoContactEmailErrorMessage).Must(x => x.IsValidEmail()).WithMessage(InvalidContactEmailErrorMessage);

            RuleFor(x => x.Username).NotEmpty().When(x => x.HearingRoleName == "Judge").WithMessage(NoUsernameErrorMessage);
            RuleFor(x => x.Username).Must(x => x.IsValidEmail()).When(x => x.HearingRoleName == "Judge" && !string.IsNullOrEmpty(x.Username))
                .WithMessage(InvalidJudgeUsernameErrorMessage);

            RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameErrorMessage);
            RuleFor(x => x.CaseRoleName).NotEmpty().WithMessage(NoCaseRoleNameErrorMessage);
            RuleFor(x => x.HearingRoleName).NotEmpty().WithMessage(NoHearingRoleNameErrorMessage);
            RuleFor(x => x.TelephoneNumber).Must((_, telephoneNumber) => IsPhone(telephoneNumber)).When(x => x.HearingRoleName != "Judge").WithMessage(NoTelephoneNumberErrorMessage);
        }
        
        private bool IsPhone(string telephoneNumber)
        {
            var phoneNumberUtil = PhoneNumbers.PhoneNumberUtil.GetInstance();
            try
            {
                return phoneNumberUtil.IsValidNumberForRegion(phoneNumberUtil.Parse(telephoneNumber, DefaultRegion), DefaultRegion);
            }
            catch (PhoneNumbers.NumberParseException)
            {
                return false;
            }
        }
    }
}