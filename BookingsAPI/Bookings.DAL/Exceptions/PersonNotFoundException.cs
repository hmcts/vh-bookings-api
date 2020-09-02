using System;
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
namespace Bookings.DAL.Exceptions
{
    public class PersonNotFoundException : Exception
    {
        public PersonNotFoundException(string username) : base(
            $"Person with username {GetObfuscatedUsernameAsync(username)} does not exist")
        {
        }

        private static string GetObfuscatedUsernameAsync(string username)
        {
            var obfuscatedUsername = System.Text.RegularExpressions.Regex.Replace(username, @"(?!\b)\w", "*");
            return obfuscatedUsername;
           
        }
    }
}