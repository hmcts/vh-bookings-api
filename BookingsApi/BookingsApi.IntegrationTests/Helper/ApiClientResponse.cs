using BookingsApi.Common.Helpers;
using Newtonsoft.Json;

namespace BookingsApi.IntegrationTests.Helper
{
    public static class ApiClientResponse
    {
        public static async Task<T> GetResponses<T>(HttpContent content)
        {
            var json = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json,
                DefaultSerializerSettings.DefaultNewtonsoftSerializerSettings());
        }
    }
}
