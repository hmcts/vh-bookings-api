using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BookingsApi.Client
{
    public partial class BookingsApiClient
    {
        public static BookingsApiClient GetClient(HttpClient httpClient)
        {
            var apiClient = new BookingsApiClient(httpClient)
            {
                ReadResponseAsString = true
            };
            apiClient.JsonSerializerSettings.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
            apiClient.JsonSerializerSettings.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            
            return apiClient;
        }
        
        public static BookingsApiClient GetClient(string baseUrl, HttpClient httpClient)
        {
            var apiClient = GetClient(httpClient);
            apiClient.BaseUrl = baseUrl;
            return apiClient;
        }

        static partial void UpdateJsonSerializerSettings(JsonSerializerOptions settings)
        {
            settings.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
            settings.WriteIndented = true;
            settings.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            settings.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        }
    }
}