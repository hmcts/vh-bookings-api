using System.Text.Json;
using System.Text.Json.Serialization;
using BookingsApi.Common.Helpers;

namespace BookingsApi.IntegrationTests.Helper
{
    public static class ApiClientResponse
    {
        public static async Task<T> GetResponses<T>(HttpContent content)
        {
            var json = await content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
                PropertyNameCaseInsensitive = true, // To make sure it matches the properties case-insensitively
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            });
        }
    }
}
