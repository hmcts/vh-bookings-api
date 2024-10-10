using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace BookingsApi.Common.Helpers
{
    public static class DefaultSerializerSettings
    {
        /// <summary>
        /// The function `DefaultNewtonsoftSerializerSettings` returns default settings for
        /// Newtonsoft.Json serialization in C# with specific configurations.
        /// </summary>
        /// <returns>
        /// The method `DefaultNewtonsoftSerializerSettings` returns a `JsonSerializerSettings` object
        /// with specific configurations set for serialization using Newtonsoft.Json.
        /// </returns>
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