using BookingsApi.Domain.Ddd;
using System.Diagnostics.CodeAnalysis;

namespace BookingsApi.Domain
{
    [ExcludeFromCodeCoverage]
    public class DayOfWeek : Entity<int>
    {
        public string Day { get; set; }
    }
}
