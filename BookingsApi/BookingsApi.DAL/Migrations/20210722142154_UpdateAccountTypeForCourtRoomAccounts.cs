using BookingsApi.Common.Configuration;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using UserApi.Client;
using System.Linq;

namespace BookingsApi.DAL.Migrations
{
    public partial class UpdateAccountTypeForCourtRoomAccounts : Migration
    {
        private List<string> _judges;

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            GetJudgesFromAdAsync().GetAwaiter().GetResult();

            _judges.ForEach(judge =>
            {
                migrationBuilder.Sql($"UPDATE [dbo].[Person] SET [AccountType]='Courtroom' where [Username]='{judge}'");
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            GetJudgesFromAdAsync().GetAwaiter().GetResult();

            _judges.ForEach(judge =>
            {
                migrationBuilder.Sql($"UPDATE [dbo].[Person] SET [AccountType]=NULL where [Username]='{judge}'");
            });
        }

        private async Task GetJudgesFromAdAsync()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddUserSecrets("D76B6EB8-F1A2-4A51-9B8F-21E1B6B81E4F");

            var config = builder.Build();

            var instrumentationKey = config["ApplicationInsights:InstrumentationKey"];

            TelemetryConfiguration configuration = TelemetryConfiguration.CreateDefault();
            configuration.InstrumentationKey = instrumentationKey;
            var telemetryClient = new TelemetryClient(configuration);


            var azureAd = config.GetSection("AzureAd").Get<AzureAdConfiguration>();
            var services = config.GetSection("Services").Get<ServicesConfiguration>();

            var authContext = new AuthenticationContext($"{azureAd.Authority}{azureAd.TenantId}");
            var credential = new ClientCredential(azureAd.ClientId, azureAd.ClientSecret);

            var result = await authContext.AcquireTokenAsync(services.UserApiResourceId, credential);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {result.AccessToken}");

                var userApiClient = new UserApiClient(client);
                try
                {
                    var userApiResponse = await userApiClient.GetJudgesAsync();
                    _judges = userApiResponse.Select(x => x.Email).ToList();
                }
                catch (Exception ex)
                {
                    telemetryClient.TrackException(ex);
                }
            }
        }
    }
}
