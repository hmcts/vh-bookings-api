using System;
using System.Runtime.Serialization;

namespace BookingsApi.DAL.Exceptions
{
    [Serializable]
    public class JusticeUserNotFoundException : BookingsDalException
    {
        public JusticeUserNotFoundException(Guid id) : base($"Justice user with id {id} not found")
        {
        }

        protected JusticeUserNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
