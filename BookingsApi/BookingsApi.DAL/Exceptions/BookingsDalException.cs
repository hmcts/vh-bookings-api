using System;
using System.Runtime.Serialization;

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

    [Serializable]
    public class JusticeUserAlreadyExistsException : BookingsDalException
    {
        public JusticeUserAlreadyExistsException(string username) : base($"A justice user with the username {username} already exists")
        {
        }

        protected JusticeUserAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}