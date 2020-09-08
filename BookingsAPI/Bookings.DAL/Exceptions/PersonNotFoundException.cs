using System;
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
namespace Bookings.DAL.Exceptions
{
    public abstract class PersonException : Exception
    {
        protected PersonException(string message) : base(message)
        {
        }

        protected static string GetObfuscatedUsernameAsync(string username)
        {
            var obfuscatedUsername = System.Text.RegularExpressions.Regex.Replace(username, @"(?!\b)\w", "*");
            return obfuscatedUsername;
        }
    }

    public class PersonNotFoundException : PersonException
    {
        public PersonNotFoundException(string username) : base(
            $"Person with username {GetObfuscatedUsernameAsync(username)} does not exist")
        {
        }
    }

    public class PersonIsAJudgeException : PersonException
    {
        public PersonIsAJudgeException(string username) : base(
            $"Person with username {GetObfuscatedUsernameAsync(username)} is a judge")
        {
        }
    }
}