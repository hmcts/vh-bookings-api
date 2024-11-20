using System.Text.Json;
using BookingsApi.Common.DotNet6.Helpers;
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
                Converters = { new PascalCaseEnumConverterFactory() }
            });
        }
        
        public static string Serialize(object request)
        {
            return JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
                PropertyNameCaseInsensitive = true,
                Converters = { new PascalCaseEnumConverterFactory() }
            });
        }
    }
}
