using System;
using System.Runtime.Serialization;

namespace BookingsApi.DAL.Exceptions
{
    public abstract class JudiciaryPersonException : Exception
    {
        protected JudiciaryPersonException(string message) : base(message)
        {
        }
        
        protected JudiciaryPersonException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }

        protected static string GetObfuscatedUsernameAsync(string username)
        {
            return System.Text.RegularExpressions.Regex.Replace(username, @"(?!\b)\w", "*");
        }
    }

    public abstract class JudiciaryLeaverException : Exception
    {
        protected JudiciaryLeaverException(string message) : base(message)
        {
        }

        protected JudiciaryLeaverException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        protected static string GetObfuscatedUsernameAsync(string username)
        {
            return System.Text.RegularExpressions.Regex.Replace(username, @"(?!\b)\w", "*");
        }
    }

    [Serializable]
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
        
        protected JudiciaryPersonNotFoundException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class JudiciaryLeaverNotFoundException : JudiciaryLeaverException
    {
        public JudiciaryLeaverNotFoundException(string username) :
            base($"Judiciary Person with username {GetObfuscatedUsernameAsync(username)} does not exist")
        {
        }

        public JudiciaryLeaverNotFoundException(Guid id) :
            base($"Judiciary Person with External ref id: {id} does not exist")
        {
        }

        protected JudiciaryLeaverNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
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
        
        protected JudiciaryPersonAlreadyExistsException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}