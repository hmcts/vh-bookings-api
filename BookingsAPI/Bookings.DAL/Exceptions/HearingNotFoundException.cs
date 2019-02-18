using System;
// ReSharper disable S3925

namespace Bookings.DAL.Exceptions
{
    public class HearingNotFoundException : Exception
    {
        public HearingNotFoundException(Guid hearingId) : base($"Hearing {hearingId} does not exist")
        {
        }
        
    }
}