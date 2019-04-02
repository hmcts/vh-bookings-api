using System;
using System.Linq;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using FluentValidation;

namespace Bookings.API.Validations
{
    public class UpdateParticipantRequestValidation : AbstractValidator<UpdateParticipantRequest>
    {
        public static readonly string NoDisplayNameErrorMessage = "Display name is required";
        public static readonly string NoHouseNumberErrorMessage = "HouseNumber is required";
        public static readonly string NoStreetErrorMessage = "Street is required";
        public static readonly string NoCityErrorMessage = "City is required";
        public static readonly string NoCountyErrorMessage = "County is required";
        public static readonly string NoPostcodeErrorMessage = "Postcode is required";


        public UpdateParticipantRequestValidation()
        {
            var individualRoles = new[] { "Claimant LIP", "Defendant LIP", "Applicant LIP", "Respondent LIP" };

            RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameErrorMessage);
            RuleFor(x => x.HouseNumber).NotEmpty().When(x => individualRoles.Contains(x.HearingRoleName)).WithMessage(NoHouseNumberErrorMessage);
            RuleFor(x => x.Street).NotEmpty().When(x => individualRoles.Contains(x.HearingRoleName)).WithMessage(NoStreetErrorMessage);
            RuleFor(x => x.City).NotEmpty().When(x => individualRoles.Contains(x.HearingRoleName)).WithMessage(NoCityErrorMessage);
            RuleFor(x => x.County).NotEmpty().When(x => individualRoles.Contains(x.HearingRoleName)).WithMessage(NoCountyErrorMessage);
            RuleFor(x => x.Postcode).NotEmpty().When(x => individualRoles.Contains(x.HearingRoleName)).WithMessage(NoPostcodeErrorMessage);
        }

        
    }
}