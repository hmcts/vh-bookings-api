namespace Bookings.Infrastructure.Services.IntegrationEvents
{
    public interface IIntegrationEvent
    {
        IntegrationEventType EventType { get; }
    }
}