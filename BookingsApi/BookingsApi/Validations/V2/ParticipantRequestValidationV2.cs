using BookingsApi.Contract.V2.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V2
{
    public class ParticipantRequestValidationV2 : AbstractValidator<ParticipantRequestV2>
    {
        public static readonly string NoDisplayNameErrorMessage = "Display name is required";
        public static readonly string InvalidDisplayNameErrorMessage = "Display name will accept upto 255 alphanumeric characters, spaces, and the following special characters: ',._-";
        public static readonly string NoHearingRoleCodeErrorMessage = "Hearing role code is required";
        public static readonly string InvalidContactEmailErrorMessage = "Contact Email is Invalid";

        public ParticipantRequestValidationV2()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(ParticipantValidationV2.NoFirstNameErrorMessage);
            RuleFor(x => x.FirstName).Matches(ParticipantValidationV2.NameRegex).WithMessage(ParticipantValidationV2.FirstNameDoesntMatchRegex);
            RuleFor(x => x.LastName).NotEmpty().WithMessage(ParticipantValidationV2.NoLastNameErrorMessage);
            RuleFor(x => x.LastName).Matches(ParticipantValidationV2.NameRegex).WithMessage(ParticipantValidationV2.LastNameDoesntMatchRegex);
            RuleFor(x => x.ContactEmail).Must(x => x.IsValidEmail())
                .When(x => !string.IsNullOrWhiteSpace(x.ContactEmail)).WithMessage(InvalidContactEmailErrorMessage);

            var regex = @"^[\p{L}\p{N}\s',._-]+${1,255}$";
            RuleFor(x => x.DisplayName)
                .NotEmpty().WithMessage(NoDisplayNameErrorMessage)
                .Matches(regex).WithMessage(InvalidDisplayNameErrorMessage);
            RuleFor(x => x.HearingRoleCode).NotEmpty().WithMessage(NoHearingRoleCodeErrorMessage);

            When(x => x.Screening != null, () =>
            {
                RuleFor(x => x.Screening).SetValidator(new ScreeningRequestValidation());
            });
        }
    }
}