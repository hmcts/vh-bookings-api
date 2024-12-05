using System.Text;

namespace BookingsApi.IntegrationTests.Helper
{
    public static class RequestBody
    {
        public static HttpContent Set<T>(T request)
        {
            return new StringContent(ApiClientResponse.Serialize(request), Encoding.UTF8, "application/json");
        }
    }
}
