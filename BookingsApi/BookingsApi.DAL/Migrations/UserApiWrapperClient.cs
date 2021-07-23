using BookingsApi.Common.Configuration;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UserApi.Client;

namespace BookingsApi.DAL.Migrations
{
    public static class UserApiWrapperClient
    {
        public static async Task<List<string>> GetJudgesFromAdAsync()
        {
            List<string> judges = new List<string>();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddUserSecrets("D76B6EB8-F1A2-4A51-9B8F-21E1B6B81E4F");

            var config = builder.Build();
     
            var instrumentationKey = config["ApplicationInsights:InstrumentationKey"];

            var telemetryClient = new TelemetryClient() { InstrumentationKey = instrumentationKey };
            

            var azureAd = config.GetSection("AzureAd").Get<AzureAdConfiguration>();
            var services = config.GetSection("Services").Get<ServicesConfiguration>();

            var authContext = new AuthenticationContext($"{azureAd.Authority}{azureAd.TenantId}");
            var credential = new ClientCredential(azureAd.ClientId, azureAd.ClientSecret);

            var result = await authContext.AcquireTokenAsync(services.UserApiUrl, credential);
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {result.AccessToken}");

            var userApiClient = new UserApiClient(client);
            try
            {
                judges =  (await userApiClient.GetJudgesAsync()).Select(x => x.Email).ToList();
            }catch(Exception ex)
            {
                telemetryClient.TrackException(ex);
            }

            return judges;
        }
    }
}
