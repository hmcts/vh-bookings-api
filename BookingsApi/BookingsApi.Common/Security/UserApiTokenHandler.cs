using BookingsApi.Common.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

namespace BookingsApi.Common.Security
{
    public class UserApiTokenHandler : DelegatingHandler
    {
        private readonly ServicesConfiguration _servicesConfiguration;

        public UserApiTokenHandler(IOptions<ServicesConfiguration> servicesConfiguration)
        {
            _servicesConfiguration = servicesConfiguration.Value;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("Authorization", $"Bearer {_servicesConfiguration.UserApiToken}");
            return await base.SendAsync(request, cancellationToken);
        }

    }
}