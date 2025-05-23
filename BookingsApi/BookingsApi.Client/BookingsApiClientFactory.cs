using System.Net.Http;
using System.Text.Json;
using BookingsApi.Common.Helpers;

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
            settings.Converters.Add(new PascalCaseEnumConverterFactory());
        }
    }
}