using System;

namespace Bookings.DAL.Exceptions
{
    // ReSharper disable once S3925
    public class HearingNotFoundException : Exception
    {
        public HearingNotFoundException(Guid hearingId) : base($"Hearing {hearingId} does not exist")
        {
        }
        
    }
}