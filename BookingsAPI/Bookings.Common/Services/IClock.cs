using System;

namespace Bookings.Common.Services
{
    public interface IClock
    {
        DateTime UtcNow { get; }
    }
}