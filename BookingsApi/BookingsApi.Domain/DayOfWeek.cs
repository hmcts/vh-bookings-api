using BookingsApi.Domain.Ddd;
namespace BookingsApi.Domain
{
    public class DayOfWeek : Entity<int>
    {
        public string Day { get; set; }
    }
}
