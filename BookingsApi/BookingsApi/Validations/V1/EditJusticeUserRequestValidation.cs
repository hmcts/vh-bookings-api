using BookingsApi.Contract.V1.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V1;

public class EditJusticeUserRequestValidation : AbstractValidator<EditJusticeUserRequest>
{
    private static readonly string DefaultRegion = "GB";
    private static readonly string _nameRegex = $"^(\\w+(?:\\w|[\\s'._-](?![\\s'._-]))*\\w+)$";
    public static readonly string NoFirstNameErrorMessage = "First name is required";
    public static readonly string NoLastNameErrorMessage = "Last name is required";

    public const string NoUsernameErrorMessage = "Username is required";
    public const string NoIdErrorMessage = "Id is required";
    public const string NoRoleErrorMessage = "Role is required";
    
    public static readonly string FirstNameDoesntMatchRegex = "First name must match regular expression";
    public static readonly string LastNameDoesntMatchRegex = "Last name must match regular expression";
        
     public EditJusticeUserRequestValidation()
    {
        RuleFor(x => x.FirstName).Matches(_nameRegex).WithMessage(FirstNameDoesntMatchRegex);
        RuleFor(x => x.LastName).Matches(_nameRegex).WithMessage(LastNameDoesntMatchRegex);

        RuleFor(x => x.FirstName).NotEmpty().WithMessage(NoFirstNameErrorMessage);
        RuleFor(x => x.LastName).NotEmpty().WithMessage(NoLastNameErrorMessage);
        RuleFor(x => x.ContactTelephone).Must((_, telephoneNumber) => IsPhone(telephoneNumber))
            .When(x => !string.IsNullOrWhiteSpace(x.ContactTelephone));
        RuleFor(x => x.Id).NotNull().WithMessage(NoIdErrorMessage);
        RuleFor(x => x.Username).NotEmpty().WithMessage(NoUsernameErrorMessage);
        RuleForEach(x => x.Roles).IsInEnum().WithMessage(NoRoleErrorMessage);
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