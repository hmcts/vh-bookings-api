using BookingsApi.Contract.Requests;
using FluentValidation;

namespace BookingsApi.Validations;

public class AddJusticeUserRequestValidation : AbstractValidator<AddJusticeUserRequest>
{
    private static readonly string DefaultRegion = "GB";
    private static readonly string _nameRegex = $"^(\\w+(?:\\w|[\\s'._-](?![\\s'._-]))*\\w+)$";
    public static readonly string FirstNameDoesntMatchRegex = "First name must match regular expression";
    public static readonly string LastNameDoesntMatchRegex = "Last name must match regular expression";
        
    public static readonly string NoFirstNameErrorMessage = "First name is required";
    public static readonly string NoLastNameErrorMessage = "Last name is required";
    public static readonly string NoUsernameErrorMessage = "Username is required";
    public static readonly string NoCreatedByErrorMessage = "CreatedBy is required";
    public static readonly string NoContactEmailErrorMessage = "Contact Email is required";
    public static readonly string InvalidContactEmailErrorMessage = "Contact Email is Invalid";

    public AddJusticeUserRequestValidation()
    {
        RuleFor(x => x.FirstName).Matches(_nameRegex).WithMessage(FirstNameDoesntMatchRegex);
        RuleFor(x => x.LastName).Matches(_nameRegex).WithMessage(LastNameDoesntMatchRegex);

        RuleFor(x => x.FirstName).NotEmpty().WithMessage(NoFirstNameErrorMessage);
        RuleFor(x => x.LastName).NotEmpty().WithMessage(NoLastNameErrorMessage);
        RuleFor(x => x.Username).NotEmpty().WithMessage(NoUsernameErrorMessage);
        RuleFor(x => x.CreatedBy).NotEmpty().WithMessage(NoCreatedByErrorMessage);
        RuleFor(x => x.ContactEmail).NotEmpty().WithMessage(NoContactEmailErrorMessage).Must(x => x.IsValidEmail())
            .WithMessage(InvalidContactEmailErrorMessage);

        RuleFor(x => x.ContactTelephone).Must((_, telephoneNumber) => IsPhone(telephoneNumber))
            .When(x => !string.IsNullOrWhiteSpace(x.ContactTelephone));
        RuleForEach(x => x.Roles).IsInEnum();
    }

    private static bool IsPhone(string telephoneNumber)
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