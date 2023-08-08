using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace BookingsApi.Common.Helpers
{
    public static class DefaultSerializerSettings
    {
        public static JsonSerializerSettings DefaultNewtonsoftSerializerSettings()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver {NamingStrategy = new SnakeCaseNamingStrategy()},
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.All
            };

            settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            settings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));

            return settings;
        }
    }
}