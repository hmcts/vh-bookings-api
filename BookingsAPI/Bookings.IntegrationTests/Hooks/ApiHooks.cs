using System;
using System.Linq;
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
        public static void BeforeApiFeature(Contexts.TestContext context)
        {
            var webHostBuilder = WebHost.CreateDefaultBuilder()
                .UseKestrel(c => c.AddServerHeader = false)
                .UseEnvironment("Development")
                .UseDefaultServiceProvider(options => options.ValidateScopes = false)
                .UseStartup<Startup>();
            context.Server = new TestServer(webHostBuilder);
            GetClientAccessTokenForApi(context);

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddUserSecrets<Startup>().Build();

            context.DbString = configuration.GetConnectionString("VhBookings");

            var dbContextOptionsBuilder = new DbContextOptionsBuilder<BookingsDbContext>();
            dbContextOptionsBuilder.EnableSensitiveDataLogging();
            dbContextOptionsBuilder.UseSqlServer(context.DbString);
            context.BookingsDbContextOptions = dbContextOptionsBuilder.Options;
            context.TestDataManager = new TestDataManager(context.BookingsDbContextOptions);
        }

        private static void GetClientAccessTokenForApi(Contexts.TestContext context)
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

            context.BearerToken = new AzureTokenProvider(azureAdConfigurationOptions).GetClientAccessToken(
                testSettings.TestClientId, testSettings.TestClientSecret,
                azureAdConfiguration.VhBookingsApiResourceId);
        }

        [BeforeScenario]
        public static void BeforeApiScenario(Contexts.TestContext context)
        {
            context.NewHearingId = Guid.Empty;
        }

        [AfterScenario]
        public static async Task AfterApiScenario(Contexts.TestContext context)
        {
            if (context.NewHearingId != Guid.Empty)
            {
                NUnit.Framework.TestContext.WriteLine($"Removing test hearing {context.NewHearingId}");
                await context.TestDataManager.RemoveVideoHearing(context.NewHearingId);
            }
            
            if (context.OldHearingId != Guid.Empty)
            {
                NUnit.Framework.TestContext.WriteLine($"Removing test hearing {context.OldHearingId}");
                await context.TestDataManager.RemoveVideoHearing(context.OldHearingId);
            }

            if (context.RemovedPersons != null && context.RemovedPersons.Any())
            {
                await context.TestDataManager.ClearUnattachedPersons(context.RemovedPersons);
            }
        }

        [AfterFeature]
        public static void AfterApiFeature(Contexts.TestContext context)
        {
            context.Server.Dispose();
        }
    }
}