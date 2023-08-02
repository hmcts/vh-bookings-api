using System.Text.RegularExpressions;

namespace BookingsApi.Validations.V1
{
    /// <summary>Simple validator to check email formats</summary>
    public static class EmailValidator
    {
        private const string RegexPattern = @"^([!#-'*/-9=?A-Z^-~-]+(\.[!#-'*/-9=?A-Z^-~-]+)*)@([!#-'*/-9=?A-Z^-~-]+(\.[!#-'*/-9=?A-Z^-~-]+)+)$";
        
        /// <summary>
        /// Test if the given string is specified and a valid email address
        /// </summary>
        /// <remarks>
        /// This was recommended one of the simplest way to manage email validation.
        /// Use of the MailAddress class did not validate all email formats e.g. 'email.@email.com' was considered valid
        /// Updated to use the above regex during validation - VIH-9428
        /// </remarks>
        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            var match = Regex.Match(email, RegexPattern);
            return match.Success;
        }
    }
}
