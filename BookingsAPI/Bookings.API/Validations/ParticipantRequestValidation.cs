using Bookings.Api.Contract.Requests;
using FluentValidation;
using System.Linq;

namespace Bookings.API.Validations
{
    public class ParticipantRequestValidation : AbstractValidator<ParticipantRequest>
    {
        public static readonly string NoDisplayNameErrorMessage = "Display name is required";
        public static readonly string NoCaseRoleNameErrorMessage = "Display name is required";
        public static readonly string NoHearingRoleNameErrorMessage = "Display name is required";
        public static readonly string NoTitleErrorMessage = "Title is required";
        public static readonly string NoFirstNameErrorMessage = "First name is required";
        public static readonly string NoLastNameErrorMessage = "Last name is required";
        public static readonly string NoUsernameErrorMessage = "Username is required";
        public static readonly string NoContactEmailErrorMessage = "Contact Email is required";
        public static readonly string NoHousenumberErrorMessage = "Housenumber is required";
        public static readonly string NoStreetErrorMessage = "Street is required";
        public static readonly string NoCityErrorMessage = "City is required";
        public static readonly string NoCountyErrorMessage = "County is required";
        public static readonly string NoPostcodeErrorMessage = "Postcode is required";



        public ParticipantRequestValidation()
        {
            var individualRoles = new[] { "Claimant LIP", "Defendant LIP", "Applicant LIP", "Respondent LIP" };
            RuleFor(x => x.Title).NotEmpty().WithMessage(NoTitleErrorMessage);
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(NoFirstNameErrorMessage);
            RuleFor(x => x.LastName).NotEmpty().WithMessage(NoLastNameErrorMessage);
            RuleFor(x => x.ContactEmail).NotEmpty().WithMessage(NoContactEmailErrorMessage);
            RuleFor(x => x.Username).NotEmpty().WithMessage(NoUsernameErrorMessage);
            
            RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameErrorMessage);
            RuleFor(x => x.CaseRoleName).NotEmpty().WithMessage(NoCaseRoleNameErrorMessage);
            RuleFor(x => x.HearingRoleName).NotEmpty().WithMessage(NoHearingRoleNameErrorMessage);
            RuleFor(x => x.HouseNumber).NotEmpty().When(x => individualRoles.Contains(x.HearingRoleName)).WithMessage(NoHousenumberErrorMessage);
            RuleFor(x => x.Street).NotEmpty().When(x => individualRoles.Contains(x.HearingRoleName)).WithMessage(NoStreetErrorMessage);
            RuleFor(x => x.City).NotEmpty().When(x => individualRoles.Contains(x.HearingRoleName)).WithMessage(NoCityErrorMessage);
            RuleFor(x => x.County).NotEmpty().When(x => individualRoles.Contains(x.HearingRoleName)).WithMessage(NoCountyErrorMessage);
            RuleFor(x => x.Postcode).NotEmpty().When(x => individualRoles.Contains(x.HearingRoleName)).WithMessage(NoPostcodeErrorMessage);



        }


    }
}