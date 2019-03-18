using System.Net.Http;
using System.Threading.Tasks;
using Bookings.API;
using Bookings.Common.Configuration;
using Bookings.Common.Security;
using Bookings.DAL;
using Bookings.IntegrationTests.Contexts;
using Bookings.IntegrationTests.Helper;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Testing.Common.Configuration;

namespace Bookings.IntegrationTests.Steps
{
    public abstract class StepsBase
    {
        protected DbContextOptions<BookingsDbContext> BookingsDbContextOptions;
        protected TestDataManager Hooks { get; set; }
        private TestServer _server;
        private string _dbString;
        private string _bearerToken;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var webHostBuilder = WebHost.CreateDefaultBuilder()
                .UseKestrel(c => c.AddServerHeader = false)
                .UseEnvironment("Development")
                .UseStartup<Startup>();
            _server = new TestServer(webHostBuilder);
            GetClientAccessTokenForApi();

            var dbContextOptionsBuilder = new DbContextOptionsBuilder<BookingsDbContext>();
            dbContextOptionsBuilder.EnableSensitiveDataLogging();
            dbContextOptionsBuilder.UseSqlServer(_dbString);
            BookingsDbContextOptions = dbContextOptionsBuilder.Options;
            Hooks = new TestDataManager(BookingsDbContextOptions);
        }

        private void GetClientAccessTokenForApi()
        {
            var configRootBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddUserSecrets<Startup>();

            var configRoot = configRootBuilder.Build();

            _dbString = configRoot.GetConnectionString("VhBookings");

            var azureAdConfigurationOptions = Options.Create(configRoot.GetSection("AzureAd").Get<AzureAdConfiguration>());
            var testSettingsOptions = Options.Create(configRoot.GetSection("Testing").Get<TestSettings>());

            var azureAdConfiguration = azureAdConfigurationOptions.Value;
            var testSettings = testSettingsOptions.Value;

            _bearerToken = new AzureTokenProvider(azureAdConfigurationOptions).GetClientAccessToken(
                testSettings.TestClientId, testSettings.TestClientSecret,
                azureAdConfiguration.VhBookingsApiResourceId);
        }

        protected async Task<HttpResponseMessage> SendGetRequestAsync(Contexts.TestContext apiTestContext)
        {
            using (var client = apiTestContext.Server.CreateClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiTestContext.BearerToken}");
                return await client.GetAsync(apiTestContext.Uri);
            }
        }

        protected async Task<HttpResponseMessage> SendPatchRequestAsync(Contexts.TestContext apiTestContext)
        {
            using (var client = apiTestContext.Server.CreateClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiTestContext.BearerToken}");
                return await client.PatchAsync(apiTestContext.Uri, apiTestContext.HttpContent);
            }
        }

        protected async Task<HttpResponseMessage> SendPostRequestAsync(Contexts.TestContext apiTestContext)
        {
            using (var client = apiTestContext.Server.CreateClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiTestContext.BearerToken}");
                return await client.PostAsync(apiTestContext.Uri, apiTestContext.HttpContent);
            }
        }

        protected async Task<HttpResponseMessage> SendPutRequestAsync(Contexts.TestContext apiTestContext)
        {
            using (var client = apiTestContext.Server.CreateClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiTestContext.BearerToken}");
                return await client.PutAsync(apiTestContext.Uri, apiTestContext.HttpContent);
            }
        }

        protected async Task<HttpResponseMessage> SendDeleteRequestAsync(Contexts.TestContext apiTestContext)
        {
            using (var client = apiTestContext.Server.CreateClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiTestContext.BearerToken}");
                return await client.DeleteAsync(apiTestContext.Uri);
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _server.Dispose();
        }
    }
}
