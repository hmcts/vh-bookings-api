namespace Bookings.Infrastructure.Services.ServiceBusQueue
{
    public class ServiceBusSettings
    {
        public string ConnectionString { get; set; }
        public string QueueName { get; set; }
    }
}