using System;
using Microsoft.Identity.Client;

namespace Bookings.Common.Security
{
    public interface ITokenProvider
    {
        string GetClientAccessToken(string tenantId, string clientId, string clientSecret, string[] scopes);
    }

    public class TokenProvider : ITokenProvider
    {
        public string GetClientAccessToken(string tenantId, string clientId, string clientSecret, string[] scopes)
        {
            var app = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithAuthority(AzureCloudInstance.AzurePublic, tenantId)
                .WithClientSecret(clientSecret)
                .Build();

            try
            {
                return app.AcquireTokenForClient(scopes).ExecuteAsync().Result.AccessToken;
            }
            catch (MsalServiceException ex)
            {
                throw new UnauthorizedAccessException("Error getting token", ex);
            }
        }
    }
}