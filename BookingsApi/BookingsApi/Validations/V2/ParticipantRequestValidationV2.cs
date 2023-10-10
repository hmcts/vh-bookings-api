using BookingsApi.Contract.V2.Requests;
using BookingsApi.Domain.Constants;
using FluentValidation;

namespace BookingsApi.Validations.V2
{
    public class ParticipantRequestValidationV2 : AbstractValidator<ParticipantRequestV2>
    {
        public static readonly string NoDisplayNameErrorMessage = "Display name is required";
        public static readonly string InvalidDisplayNameErrorMessage = "Display name will accept upto 255 alphanumeric characters, spaces, and the following special characters: ',._-";
        public static readonly string NoHearingRoleCodeErrorMessage = "Hearing role code is required";
        public static readonly string NoTelephoneNumberErrorMessage = "Telephone Number is required";
        public static readonly string NoContactEmailErrorMessage = "Contact Email is required";
        public static readonly string InvalidContactEmailErrorMessage = "Contact Email is Invalid";

        public ParticipantRequestValidationV2()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(ParticipantValidationV2.NoFirstNameErrorMessage);
            RuleFor(x => x.FirstName).Matches(ParticipantValidationV2.NameRegex).WithMessage(ParticipantValidationV2.FirstNameDoesntMatchRegex);
            RuleFor(x => x.LastName).NotEmpty().WithMessage(ParticipantValidationV2.NoLastNameErrorMessage);
            RuleFor(x => x.LastName).Matches(ParticipantValidationV2.NameRegex).WithMessage(ParticipantValidationV2.LastNameDoesntMatchRegex);
            RuleFor(x => x.ContactEmail).NotEmpty().WithMessage(NoContactEmailErrorMessage).Must(x => x.IsValidEmail()).WithMessage(InvalidContactEmailErrorMessage);

            var regex = "^([-A-Za-z0-9 ',._]){1,255}$";
            RuleFor(x => x.DisplayName)
                .NotEmpty().WithMessage(NoDisplayNameErrorMessage)
                .Matches(regex).WithMessage(InvalidDisplayNameErrorMessage);
            RuleFor(x => x.HearingRoleCode).NotEmpty().WithMessage(NoHearingRoleCodeErrorMessage);
            RuleFor(x => x.TelephoneNumber).NotEmpty().When(x => x.HearingRoleCode != HearingRoleCodes.Judge).WithMessage(NoTelephoneNumberErrorMessage);
        }
    }
}