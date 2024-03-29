using System.Text;
using AcceptanceTests.Common.Api.Helpers;

namespace BookingsApi.IntegrationTests.Helper
{
    public static class RequestBody
    {
        public static HttpContent Set<T>(T request)
        {
            return new StringContent(RequestHelper.Serialise(request), Encoding.UTF8, "application/json");
        }
    }
}
