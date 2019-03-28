using System;
using System.Collections.Generic;
using System.Text;

namespace Bookings.DAL.Exceptions
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly

    public class ParticipantNotFoundException : Exception
    {
        public ParticipantNotFoundException(Guid participantId) : base($"Participant {participantId} does not exist")
        {
        }
    }
}
