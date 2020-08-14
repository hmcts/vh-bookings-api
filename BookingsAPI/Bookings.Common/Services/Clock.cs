using System;

namespace Bookings.Common.Services
{
    public class Clock : IClock
    {
        public DateTime UtcNow { get; } = DateTime.UtcNow;
    }
}