using System;

namespace Bookings.Domain.Exceptions
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class ParticipantHasNotHearingException : Exception
    {
        public ParticipantHasNotHearingException(string message) : base(message)
        {
        }
    }
}
