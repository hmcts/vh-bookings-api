using System;

namespace BookingsApi.DAL.Exceptions
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly

    public class NonWorkingHoursNotFoundException : Exception
    {
        public NonWorkingHoursNotFoundException(long id) : base($"NonWorkingHours {id} does not exist") { }
    }
}
