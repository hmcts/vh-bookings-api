 using System.Collections.Generic;
using System.Net.Http;
using AcceptanceTests.Common.Configuration;
using AcceptanceTests.Common.Configuration.Users;
using Bookings.API;
using Bookings.Api.Contract.Requests;
using Bookings.Common.Configuration;
using Bookings.Common.Security;
using Bookings.DAL;
using Bookings.IntegrationTests.Contexts;
using Bookings.IntegrationTests.Helper;
using FluentAssertions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TechTalk.SpecFlow;
using Testing.Common.Configuration;
using TestData = Testing.Common.Configuration.TestData;

namespace Bookings.IntegrationTests.Hooks
{
    [Binding]
    public class ConfigHooks
    {
        private readonly IConfigurationRoot _configRoot;

        public ConfigHooks(TestContext context)
        {
            _configRoot = ConfigurationManager.BuildConfig("D76B6EB8-F1A2-4A51-9B8F-21E1B6B81E4F");
            context.Config = new Config();
            context.UserAccounts = new List<UserAccount>();
        }

        [BeforeScenario(Order = (int)HooksSequence.ConfigHooks)]
        public void RegisterSecrets(TestContext context)
        {
            var azureOptions = RegisterAzureSecrets(context);
            RegisterHearingServices(context);
            RegisterTestSettings(context);
            RegisterTestUsers(context);
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

        private void RegisterTestUsers(TestContext context)
        {
            context.UserAccounts = Options.Create(_configRoot.GetSection("UserAccounts").Get<List<UserAccount>>()).Value;
            context.UserAccounts.Should().NotBeNullOrEmpty();
            foreach (var user in context.UserAccounts)
            {
                user.Key = user.Lastname;
                user.Username = $"{user.DisplayName.Replace(" ", "")}@{context.Config.TestSettings.UsernameStem}";
            }
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
        }
    }
}
