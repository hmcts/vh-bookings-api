namespace BookingsApi.DAL.Exceptions
{
    public abstract class BookingsDalException : Exception
    {
        protected BookingsDalException(string message) : base(message)
        {
        }
        
        protected BookingsDalException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
    
    public abstract class EntityNotFoundException : BookingsDalException
    {
        protected EntityNotFoundException(string message) : base(message)
        {
        }
        
        protected EntityNotFoundException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }

    public abstract class ObfuscatedEntityNotFoundException : EntityNotFoundException
    {

        protected ObfuscatedEntityNotFoundException(string message) : base(message)
        {
        }

        protected ObfuscatedEntityNotFoundException(SerializationInfo info, StreamingContext context) : base(info,
            context)
        {
        }

        protected static string GetObfuscatedUsernameAsync(string username)
        {
            var obfuscatedUsername = System.Text.RegularExpressions.Regex.Replace(username, @"(?!\b)\w", "*");
            return obfuscatedUsername;
        }
    }
}