using System;
using System.Linq;

namespace Bookings.Common.Services
{
    public class RandomGenerator : IRandomGenerator
    {
        private readonly IClock _clock;

        public RandomGenerator(IClock clock)
        {
            _clock = clock;
        }
        
        public string GetRandomFromTicks(uint skip, uint take)
        {
            return GetRandomFromTicks(_clock.UtcNow, skip, take);
        }

        public string GetRandomFromTicks(DateTime dateTime, uint skip, uint take)
        {
            var ticks = dateTime.Ticks.ToString();

            if (skip >= ticks.Length - 1 || take > ticks.Length - skip)
            {
                throw new ArgumentOutOfRangeException($"skip and take values are wrong: {skip}:{take}");
            }

            return string.Concat(ticks.Skip((int) skip).Take((int) take));
        }
    }
}