using System;

namespace BookingsApi.Common.Services
{
    public class RandomNumberGenerator : IRandomNumberGenerator
    {
        public int Generate(int min, int max)
        {
            return new Random().Next(min, max);
        }
    }
}
