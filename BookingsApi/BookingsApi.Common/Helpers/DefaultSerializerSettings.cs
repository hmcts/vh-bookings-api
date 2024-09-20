using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

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
        
        public static JsonSerializerSettings DefaultNewtonsoftSerializerSettings()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver {NamingStrategy = new SnakeCaseNamingStrategy()},
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Objects
            };

            settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            settings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));

            return settings;
        }
    }
}