using System;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace BookingsApi.Validations
{
    /// <summary>Simple validator to check email formats</summary>
    public static class EmailValidator
    {
        private static TimeSpan RegexTimeOut = TimeSpan.FromSeconds(4);
        private const string RegexPattern = @"^([!#-'*+/-9=?A-Z^-~-]+(\.[!#-'*+/-9=?A-Z^-~-]+)*)@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])(?:[a-zA-Z0-9](?:\.[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";

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

            try
            {
                var r = Regex.Match(email, RegexPattern, RegexOptions.None, RegexTimeOut);
                return r.Success;
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}
