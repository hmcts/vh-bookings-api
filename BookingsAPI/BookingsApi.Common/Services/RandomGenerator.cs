using System;
using System.Linq;

namespace BookingsApi.Common.Services
{
    public class RandomGenerator : IRandomGenerator
    {
        public string GetWeakDeterministic(long ticks, uint skip, uint take)
        {
            var ticksString = ticks.ToString();

            if (skip >= ticksString.Length || take > ticksString.Length - skip)
            {
                throw new ArgumentOutOfRangeException($"skip and take values are wrong: {skip}:{take}");
            }

            return string.Concat(ticksString.Reverse().Skip((int) skip).Take((int) take));
        }
    }
}