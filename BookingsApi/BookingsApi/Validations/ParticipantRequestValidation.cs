using BookingsApi.Contract.Requests;
using FluentValidation;

namespace BookingsApi.Validations
{
    public class ParticipantRequestValidation : AbstractValidator<ParticipantRequest>
    {
        public static readonly string NoDisplayNameErrorMessage = "Display name is required";
        public static readonly string NoCaseRoleNameErrorMessage = "Case role is required";
        public static readonly string NoHearingRoleNameErrorMessage = "Hearing role is required";
        public static readonly string NoFirstNameErrorMessage = "First name is required";
        public static readonly string NoLastNameErrorMessage = "Last name is required";
        public static readonly string NoUsernameErrorMessage = "Username is required";
        public static readonly string NoContactEmailErrorMessage = "Contact Email is required";
        public static readonly string NoTelephoneNumberErrorMessage = "Telephone Number is required";
        public static readonly string InvalidContactEmailErrorMessage = "Contact Email is Invalid";
        public static readonly string InvalidJudgeUsernameErrorMessage = "Judge username is Invalid";
        public static readonly string InvalidTelephoneNumberErrorMessage = "Telephone Number is Invalid";

        public ParticipantRequestValidation()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(NoFirstNameErrorMessage);
            RuleFor(x => x.LastName).NotEmpty().WithMessage(NoLastNameErrorMessage);
            RuleFor(x => x.ContactEmail).NotEmpty().WithMessage(NoContactEmailErrorMessage).Must(x => x.IsValidEmail()).WithMessage(InvalidContactEmailErrorMessage);

            RuleFor(x => x.Username).NotEmpty().When(x => x.HearingRoleName == "Judge").WithMessage(NoUsernameErrorMessage);
            RuleFor(x => x.Username).Must(x => x.IsValidEmail()).When(x => x.HearingRoleName == "Judge" && !string.IsNullOrEmpty(x.Username))
                .WithMessage(InvalidJudgeUsernameErrorMessage);

            RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameErrorMessage);
            RuleFor(x => x.CaseRoleName).NotEmpty().WithMessage(NoCaseRoleNameErrorMessage);
            RuleFor(x => x.HearingRoleName).NotEmpty().WithMessage(NoHearingRoleNameErrorMessage);
            RuleFor(x => x.TelephoneNumber).NotEmpty().WithMessage(NoTelephoneNumberErrorMessage).Must(x => x.IsValidPhoneNumber()).WithMessage(InvalidTelephoneNumberErrorMessage);
        }
    }
}