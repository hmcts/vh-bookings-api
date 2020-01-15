using Bookings.Api.Contract.Requests;
using FluentValidation;

namespace Bookings.API.Validations
{
    public class ParticipantRequestValidation : AbstractValidator<ParticipantRequest>
    {
        public static readonly string NoDisplayNameErrorMessage = "Display name is required";
        public static readonly string NoCaseRoleNameErrorMessage = "Display name is required";
        public static readonly string NoHearingRoleNameErrorMessage = "Display name is required";
        public static readonly string NoFirstNameErrorMessage = "First name is required";
        public static readonly string NoLastNameErrorMessage = "Last name is required";
        public static readonly string NoUsernameErrorMessage = "Username is required";
        public static readonly string NoContactEmailErrorMessage = "Contact Email is required";

        public ParticipantRequestValidation()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(NoFirstNameErrorMessage);
            RuleFor(x => x.LastName).NotEmpty().WithMessage(NoLastNameErrorMessage);
            RuleFor(x => x.ContactEmail).NotEmpty().WithMessage(NoContactEmailErrorMessage);
            RuleFor(x => x.Username).NotEmpty().WithMessage(NoUsernameErrorMessage);
            RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameErrorMessage);
            RuleFor(x => x.CaseRoleName).NotEmpty().WithMessage(NoCaseRoleNameErrorMessage);
            RuleFor(x => x.HearingRoleName).NotEmpty().WithMessage(NoHearingRoleNameErrorMessage);
        }
    }
}