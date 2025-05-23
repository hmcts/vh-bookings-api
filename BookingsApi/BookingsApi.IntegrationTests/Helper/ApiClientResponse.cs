using System.Text.Json;
using BookingsApi.Common.Helpers;

namespace BookingsApi.IntegrationTests.Helper
{
    public static class ApiClientResponse
    {
        private static JsonSerializerOptions Options => new()
        {
            PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
            PropertyNameCaseInsensitive = true,
            Converters = { new PascalCaseEnumConverterFactory() }
        };
        
        public static async Task<T> GetResponses<T>(HttpContent content)
        {
            var json = await content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, Options);
        }
        
        public static string Serialize(object request)
        {
            return JsonSerializer.Serialize(request, Options);
        }
    }
}
