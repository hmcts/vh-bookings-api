using System;
using System.Text.Json;
using JsonException = System.Text.Json.JsonException;

namespace BookingsApi.Common.Helpers;

public class PascalCaseEnumConverter<T> : System.Text.Json.Serialization.JsonConverter<T> where T : struct, Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var enumValue = reader.GetString();
        if (Enum.TryParse(enumValue, ignoreCase: true, out T result))
        {
            return result;
        }
        throw new JsonException($"Unable to convert \"{enumValue}\" to Enum \"{typeof(T)}\".");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var enumValue = value.ToString(); // Gets the PascalCase name of the enum
        writer.WriteStringValue(enumValue);
    }
}