using System;
using System.Threading.Tasks;
using Bookings.API;
using Bookings.Common.Configuration;
using Bookings.Common.Security;
using Bookings.DAL;
using Bookings.IntegrationTests.Helper;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TechTalk.SpecFlow;
using Testing.Common.Configuration;


namespace Bookings.IntegrationTests.Hooks
{
    [Binding]
    public static class ApiHooks
    {
        [BeforeFeature]
        public static void BeforeApiFeature(Contexts.TestContext apiTestContext)
        {
            var webHostBuilder = WebHost.CreateDefaultBuilder()
                .UseKestrel(c => c.AddServerHeader = false)
                .UseEnvironment("Development")
                .UseDefaultServiceProvider(options => options.ValidateScopes = false)
                .UseStartup<Startup>();
            apiTestContext.Server = new TestServer(webHostBuilder);
            GetClientAccessTokenForApi(apiTestContext);

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddUserSecrets<Startup>().Build();

            apiTestContext.DbString = configuration.GetConnectionString("VhBookings");

            var dbContextOptionsBuilder = new DbContextOptionsBuilder<BookingsDbContext>();
            dbContextOptionsBuilder.EnableSensitiveDataLogging();
            dbContextOptionsBuilder.UseSqlServer(apiTestContext.DbString);
            apiTestContext.BookingsDbContextOptions = dbContextOptionsBuilder.Options;
            apiTestContext.TestDataManager = new TestDataManager(apiTestContext.BookingsDbContextOptions);
        }

        private static void GetClientAccessTokenForApi(Contexts.TestContext apiTestContext)
        {
            var configRootBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddUserSecrets<Startup>();

            var configRoot = configRootBuilder.Build();

            var azureAdConfigurationOptions =
                Options.Create(configRoot.GetSection("AzureAd").Get<AzureAdConfiguration>());
            var testSettingsOptions = Options.Create(configRoot.GetSection("Testing").Get<TestSettings>());
            var azureAdConfiguration = azureAdConfigurationOptions.Value;
            var testSettings = testSettingsOptions.Value;

            apiTestContext.BearerToken = new AzureTokenProvider(azureAdConfigurationOptions).GetClientAccessToken(
                testSettings.TestClientId, testSettings.TestClientSecret,
                azureAdConfiguration.VhBookingsApiResourceId);
        }

        [BeforeScenario]
        public static void BeforeApiScenario(Contexts.TestContext apiTestContext)
        {
            apiTestContext.NewHearingId = Guid.Empty;
        }

        [AfterScenario]
        public static async Task AfterApiScenario(Contexts.TestContext apiTestContext)
        {
            if (apiTestContext.NewHearingId != Guid.Empty)
            {
                NUnit.Framework.TestContext.WriteLine($"Removing test hearing {apiTestContext.NewHearingId}");
                await apiTestContext.TestDataManager.RemoveVideoHearing(apiTestContext.NewHearingId);
            }
        }

        [AfterFeature]
        public static void AfterApiFeature(Contexts.TestContext apiTestContext)
        {
            apiTestContext.Server.Dispose();
        }
    }
}