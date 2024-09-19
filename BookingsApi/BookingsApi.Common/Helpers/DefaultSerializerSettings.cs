using System.Text.Json;
using System.Text.Json.Serialization;

namespace BookingsApi.Common.Helpers
{
    public static class DefaultSerializerSettings
    {
        public static JsonSerializerOptions DefaultSystemTextJsonSerializerSettings()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };

            return options;
        }
    }
}