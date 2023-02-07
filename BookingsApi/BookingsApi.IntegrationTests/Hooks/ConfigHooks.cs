using System.Collections.Generic;
using System.Net.Http;
using AcceptanceTests.Common.Api;
using AcceptanceTests.Common.Configuration;
using AcceptanceTests.Common.Configuration.Users;
using BookingsApi.Common.Configuration;
using BookingsApi.Common.Security;
using BookingsApi.Contract.Configuration;
using BookingsApi.Contract.Requests;
using BookingsApi.DAL;
using BookingsApi.IntegrationTests.Contexts;
using BookingsApi.IntegrationTests.Helper;
using FluentAssertions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TechTalk.SpecFlow;
using Testing.Common.Configuration;
using ConfigurationManager = AcceptanceTests.Common.Configuration.ConfigurationManager;
using TestData = Testing.Common.Configuration.TestData;

namespace BookingsApi.IntegrationTests.Hooks
{
    [Binding]
    public class ConfigHooks
    {
        private readonly IConfigurationRoot _configRoot;

        public ConfigHooks(TestContext context)
        {
            var userSecretsId = "D76B6EB8-F1A2-4A51-9B8F-21E1B6B81E4F";
            _configRoot = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddJsonFile("appsettings.Development.json", true)
                .AddJsonFile("appsettings.Production.json", true).AddJsonFile("useraccounts.json", true)
                .AddUserSecrets(userSecretsId).Build();
            context.Config = new Config();
            context.UserAccounts = new List<UserAccount>();
        }

        [BeforeScenario(Order = (int)HooksSequence.ConfigHooks)]
        public void RegisterSecrets(TestContext context)
        {
            var azureOptions = RegisterAzureSecrets(context);
            RegisterHearingServices(context);
            RegisterTestSettings(context);
            RegisterDefaultData(context);
            RegisterDatabaseSettings(context);
            RegisterServer(context);
            RegisterApiSettings(context);
            GenerateBearerTokens(context, azureOptions);
        }

        private IOptions<AzureAdConfiguration> RegisterAzureSecrets(TestContext context)
        {
            var azureOptions = Options.Create(_configRoot.GetSection("AzureAd").Get<AzureAdConfiguration>());
            context.Config.AzureAdConfiguration = azureOptions.Value;
            ConfigurationManager.VerifyConfigValuesSet(context.Config.AzureAdConfiguration);
            return azureOptions;
        }

        private void RegisterHearingServices(TestContext context)
        {
            context.Config.ServicesConfiguration = Options.Create(_configRoot.GetSection("Services").Get<ServicesConfiguration>()).Value;
            context.Config.ServicesConfiguration.BookingsApiResourceId.Should().NotBeNullOrEmpty();
        }

        private void RegisterTestSettings(TestContext context)
        {
            context.Config.TestSettings = Options.Create(_configRoot.GetSection("Testing").Get<TestSettings>()).Value;
            ConfigurationManager.VerifyConfigValuesSet(context.Config.TestSettings);
        }

        private static void RegisterDefaultData(TestContext context)
        {
            context.TestData = new TestData()
            {
                CaseName = "Bookings Api Integration Test",
                Participants = new List<ParticipantRequest>(),
                RemovedPersons = new List<string>()
            };
            context.TestData.CaseName.Should().NotBeNullOrEmpty();
        }

        private void RegisterDatabaseSettings(TestContext context)
        {
            context.Config.ConnectionStrings = Options.Create(_configRoot.GetSection("ConnectionStrings").Get<ConnectionStrings>()).Value;
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<BookingsDbContext>();
            dbContextOptionsBuilder.EnableSensitiveDataLogging();
            dbContextOptionsBuilder.UseSqlServer(context.Config.ConnectionStrings.VhBookings);
            context.BookingsDbContextOptions = dbContextOptionsBuilder.Options;
            context.TestDataManager = new TestDataManager(context.BookingsDbContextOptions, context.TestData.CaseName);
        }

        private static void RegisterServer(TestContext context)
        {
            var webHostBuilder = WebHost.CreateDefaultBuilder()
                    .UseKestrel(c => c.AddServerHeader = false)
                    .UseEnvironment("Development")
                    .UseStartup<Startup>();
            context.Server = new TestServer(webHostBuilder);
        }

        private static void RegisterApiSettings(TestContext context)
        {
            context.Response = new HttpResponseMessage();
        }

        private static void GenerateBearerTokens(TestContext context, IOptions<AzureAdConfiguration> azureOptions)
        {
            context.BearerToken = new AzureTokenProvider(azureOptions).GetClientAccessToken(
                azureOptions.Value.ClientId, azureOptions.Value.ClientSecret,
                context.Config.ServicesConfiguration.BookingsApiResourceId);
            context.BearerToken.Should().NotBeNullOrEmpty();

            Zap.SetAuthToken(context.BearerToken);
        }
    }
}
