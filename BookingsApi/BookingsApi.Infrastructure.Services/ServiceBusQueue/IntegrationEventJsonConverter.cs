using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using BookingsApi.Infrastructure.Services.IntegrationEvents;

namespace BookingsApi.Infrastructure.Services.ServiceBusQueue;

public class IntegrationEventJsonConverter : JsonConverter<IIntegrationEvent>
{
    public override IIntegrationEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Implement this if you need deserialization support.
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, IIntegrationEvent value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}