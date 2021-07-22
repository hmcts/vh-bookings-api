using BookingsApi.Common.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace BookingsApi.Common.Security
{
    public class UserApiTokenHandler : DelegatingHandler
    {
        private readonly ServicesConfiguration _servicesConfiguration;
        private readonly AzureAdConfiguration _azureConfiguration;

        public UserApiTokenHandler(IOptions<ServicesConfiguration> servicesConfiguration, IOptions<AzureAdConfiguration> azureConfig)
        {
            _servicesConfiguration = servicesConfiguration.Value;
            _azureConfiguration = azureConfig.Value;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authContext = new AuthenticationContext($"{_azureConfiguration.Authority}{_azureConfiguration.TenantId}");
            var credential = new ClientCredential(_azureConfiguration.ClientId, _azureConfiguration.ClientSecret);

            var result = await authContext.AcquireTokenAsync(_servicesConfiguration.UserApiUrl, credential);

            request.Headers.Add("Authorization", $"Bearer {result.AccessToken}");
            return await base.SendAsync(request, cancellationToken);
        }

    }
}