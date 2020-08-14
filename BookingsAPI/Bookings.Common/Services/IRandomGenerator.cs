using System;

namespace Bookings.Common.Services
{
    public interface IRandomGenerator
    {
        // string GetRandomSip();
        // string GetRandomPin();
        string GetRandomFromTicks(uint skip, uint take);
        string GetRandomFromTicks(DateTime dateTime, uint skip, uint take);
    }
}