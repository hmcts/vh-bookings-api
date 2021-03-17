using System;

#pragma warning disable S3925 // "ISerializable" should be implemented correctly
namespace BookingsApi.DAL.Exceptions
{
    public abstract class JudiciaryPersonException : Exception
    {
        protected JudiciaryPersonException(string message) : base(message)
        {
        }

        protected static string GetObfuscatedUsernameAsync(string username)
        {
            return System.Text.RegularExpressions.Regex.Replace(username, @"(?!\b)\w", "*");
        }
    }

    public class JudiciaryPersonNotFoundException : JudiciaryPersonException
    {
        public JudiciaryPersonNotFoundException(string username) : 
            base($"Judiciary Person with username {GetObfuscatedUsernameAsync(username)} does not exist")
        {
        }
        
        public JudiciaryPersonNotFoundException(Guid id) : 
            base($"Judiciary Person with External ref id: {id} does not exist")
        {
        }
    }
    
    public class JudiciaryPersonAlreadyExistsException : JudiciaryPersonException
    {
        public JudiciaryPersonAlreadyExistsException(string username) : 
            base($"Judiciary Person with username {GetObfuscatedUsernameAsync(username)} already exists")
        {
        }
        
        public JudiciaryPersonAlreadyExistsException(Guid externalRefId) : 
            base($"Judiciary Person with ExternalRefId {externalRefId} already exists")
        {
        }
    }
}