using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Requests.Consts;
using FluentValidation;

namespace BookingsApi.Validations
{
    public class ParticipantRequestValidation : AbstractValidator<ParticipantRequest>
    {
        public static readonly string NoDisplayNameErrorMessage = "Display name is required";
        public static readonly string NoCaseRoleNameErrorMessage = "Case Role name is required";
        public static readonly string NoHearingRoleNameErrorMessage = "Hearing Role name is required";
        public static readonly string NoFirstNameErrorMessage = "First name is required";
        public static readonly string NoLastNameErrorMessage = "Last name is required";
        public static readonly string NoUsernameErrorMessage = "Username is required";
        public static readonly string NoContactEmailErrorMessage = "Contact Email is required";
        public static readonly string NoTelephoneErrorMessage = "Telephone is required";

        public ParticipantRequestValidation()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(NoFirstNameErrorMessage);
            RuleFor(x => x.LastName).NotEmpty().WithMessage(NoLastNameErrorMessage);
            RuleFor(x => x.ContactEmail).NotEmpty().WithMessage(NoContactEmailErrorMessage);
            RuleFor(x => x.Username).NotEmpty().WithMessage(NoUsernameErrorMessage);
            RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameErrorMessage);
            RuleFor(x => x.CaseRoleName).NotEmpty().WithMessage(NoCaseRoleNameErrorMessage);
            RuleFor(x => x.HearingRoleName).NotEmpty().WithMessage(NoHearingRoleNameErrorMessage);
            RuleFor(x => x.TelephoneNumber).NotEmpty().WithMessage(NoTelephoneErrorMessage).When(x => x.HearingRoleName == HearingRoleName.StaffMember);
        }
    }
}