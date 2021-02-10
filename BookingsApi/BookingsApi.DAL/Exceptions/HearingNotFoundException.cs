using System;

namespace BookingsApi.DAL.Exceptions
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class HearingNotFoundException : Exception
    {
        
        
        public HearingNotFoundException(Guid hearingId) : base($"Hearing {hearingId} does not exist")
        {
        }
        
    }
}