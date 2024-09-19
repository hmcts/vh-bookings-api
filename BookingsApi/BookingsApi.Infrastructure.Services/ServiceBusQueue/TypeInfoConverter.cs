using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BookingsApi.Infrastructure.Services.ServiceBusQueue;

public class TypeInfoConverter<T> : JsonConverter<T>
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Implement deserialization if needed
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        // Write the type info property
        writer.WriteStartObject();
        writer.WriteString("$type", typeof(T).FullName + ", " + typeof(T).Assembly.GetName().Name);

        // Serialize the rest of the properties
        foreach (var property in typeof(T).GetProperties())
        {
            var propertyValue = property.GetValue(value);
            JsonSerializer.Serialize(writer, propertyValue, property.PropertyType, options);
        }
        
        writer.WriteEndObject();
    }
}