using System;
using System.Linq;

namespace BookingsApi.Domain
{
    public static class RandomStringGenerator
    {
        private static readonly Random Random = new Random();
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string GenerateRandomString(int length)
        {
            return new string(Enumerable.Repeat(Chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}