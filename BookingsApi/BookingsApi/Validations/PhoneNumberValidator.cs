using System.Text.RegularExpressions;

namespace BookingsApi.Validations
{
    public static class PhoneNumberValidation
    {
        public static bool IsValidPhoneNumber(this string phoneNumber)
        {
            return Regex.Match(phoneNumber, "^([0-9() +-.])*$").Success;
        }
    }
}
