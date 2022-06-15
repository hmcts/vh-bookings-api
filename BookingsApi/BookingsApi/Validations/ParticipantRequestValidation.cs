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
        // TODO: apply email validation on username and contact email
        public ParticipantRequestValidation()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(NoFirstNameErrorMessage);
            RuleFor(x => x.LastName).NotEmpty().WithMessage(NoLastNameErrorMessage);
            RuleFor(x => x.ContactEmail).NotEmpty().WithMessage(NoContactEmailErrorMessage);
            RuleFor(x => x.Username).NotEmpty().When(x => x.CaseRoleName == "Judge").WithMessage(NoUsernameErrorMessage);

            RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameErrorMessage);
            RuleFor(x => x.CaseRoleName).NotEmpty().WithMessage(NoCaseRoleNameErrorMessage);
            RuleFor(x => x.HearingRoleName).NotEmpty().WithMessage(NoHearingRoleNameErrorMessage);
        }
    }
}