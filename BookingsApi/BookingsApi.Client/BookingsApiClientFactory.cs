using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

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
            apiClient.JsonSerializerSettings.ContractResolver = new DefaultContractResolver {NamingStrategy = new SnakeCaseNamingStrategy()};
            apiClient.JsonSerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            apiClient.JsonSerializerSettings.Converters.Add(new StringEnumConverter());
            return apiClient;
        }
        
        public static BookingsApiClient GetClient(string baseUrl, HttpClient httpClient)
        {
            var apiClient = GetClient(httpClient);
            apiClient.BaseUrl = baseUrl;
            return apiClient;
        }
    }
}