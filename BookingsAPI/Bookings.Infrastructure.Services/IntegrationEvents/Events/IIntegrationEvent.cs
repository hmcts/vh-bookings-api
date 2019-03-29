namespace Bookings.Infrastructure.Services.IntegrationEvents.Events
{
    public interface IIntegrationEvent
    {
        IntegrationEventType EventType { get; }
    }
}