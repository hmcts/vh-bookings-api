using System;
using System.Threading.Tasks;
using BookingsApi.Common.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace BookingsApi.Common.Security
{
    public interface ITokenProvider
    {
        Task<string> GetClientAccessToken(string clientId, string clientSecret, string clientResource);
        Task<AuthenticationResult> GetAuthorisationResult(string clientId, string clientSecret, string clientResource);
    }

    public class AzureTokenProvider : ITokenProvider
    {
        private readonly AzureAdConfiguration _securitySettings;

        public AzureTokenProvider(IOptions<AzureAdConfiguration> environmentConfiguration)
        {
            _securitySettings = environmentConfiguration.Value;
        }

        public async Task<string> GetClientAccessToken(string clientId, string clientSecret, string clientResource)
        {
            var result = await GetAuthorisationResult(clientId, clientSecret, clientResource);
            return result.AccessToken;
        }

        public async Task<AuthenticationResult> GetAuthorisationResult(string clientId, string clientSecret,
            string clientResource)
        {
            
            AuthenticationResult result;
            var authority = $"{_securitySettings.Authority}{_securitySettings.TenantId}";
            var app = ConfidentialClientApplicationBuilder.Create(clientId).WithClientSecret(clientSecret)
                .WithAuthority(authority).Build();

            try
            {
                result = await app.AcquireTokenForClient(new[] {$"{clientResource}/.default"}).ExecuteAsync();
            }
            catch (MsalServiceException)
            {
                throw new UnauthorizedAccessException();
            }

            return result;
        }
    }
}