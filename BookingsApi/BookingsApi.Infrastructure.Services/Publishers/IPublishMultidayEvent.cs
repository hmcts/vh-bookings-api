namespace BookingsApi.Infrastructure.Services.Publishers
{
    public interface IPublishMultidayEvent : IPublishEvent
    {
        int TotalDays { get;  set; }
    }

}
