using System.Collections.Generic;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api;
using AcceptanceTests.Common.Configuration;
using AcceptanceTests.Common.Configuration.Users;
using Bookings.AcceptanceTests.Contexts;
using Bookings.Api.Contract.Responses;
using Bookings.Common.Configuration;
using Bookings.Domain;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TechTalk.SpecFlow;
using Testing.Common.Configuration;

namespace Bookings.AcceptanceTests.Hooks
{
    [Binding]
    public class ConfigHooks
    {
        private readonly IConfigurationRoot _configRoot;

        public ConfigHooks(TestContext context)
        {
            _configRoot = ConfigurationManager.BuildConfig("D76B6EB8-F1A2-4A51-9B8F-21E1B6B81E4F", GetTargetEnvironment());
            context.Config = new Config();
        }

        private static string GetTargetEnvironment()
        {
            return NUnit.Framework.TestContext.Parameters["TargetEnvironment"] ?? "";
        }

        [BeforeScenario(Order = (int)HooksSequence.ConfigHooks)]
        public async Task RegisterSecrets(TestContext context)
        {
            RegisterAzureSecrets(context);
            RegisterHearingServices(context);
            RegisterTestSettings(context);
            RegisterTestUsers(context);
            RegisterDefaultData(context);
            await GenerateBearerTokens(context);
        }

        private void RegisterAzureSecrets(TestContext context)
        {
            context.Config.AzureAdConfiguration = Options.Create(_configRoot.GetSection("AzureAd").Get<AzureAdConfiguration>()).Value;
            context.Config.AzureAdConfiguration.Authority += context.Config.AzureAdConfiguration.TenantId;
            ConfigurationManager.VerifyConfigValuesSet(context.Config.AzureAdConfiguration);
        }

        private void RegisterHearingServices(TestContext context)
        {
            context.Config.ServicesConfiguration = Options.Create(_configRoot.GetSection("Services").Get<ServicesConfiguration>()).Value;
            ConfigurationManager.VerifyConfigValuesSet(context.Config.ServicesConfiguration);
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
                CaseName = "Bookings Api Automated Test",
                ParticipantsResponses = new List<ParticipantResponse>(),
                EndPointResponses = new List<EndpointResponse>(),
                TestContextData = new Dictionary<string, dynamic>()
            };
            context.TestData.CaseName.Should().NotBeNullOrEmpty();
        }

        private static async Task GenerateBearerTokens(TestContext context)
        {
            var azureConfig = new AzureAdConfig()
            {
                Authority = context.Config.AzureAdConfiguration.Authority,
                ClientId = context.Config.AzureAdConfiguration.ClientId,
                ClientSecret = context.Config.AzureAdConfiguration.ClientSecret,
                TenantId = context.Config.AzureAdConfiguration.TenantId
            };

            context.BearerToken = await ConfigurationManager.GetBearerToken(
                azureConfig, context.Config.ServicesConfiguration.BookingsApiResourceId);
            context.BearerToken.Should().NotBeNullOrEmpty();

            Zap.SetAuthToken(context.BearerToken);
        }
    }

    internal class AzureAdConfig : IAzureAdConfig
    {
        public string Authority { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string TenantId { get; set; }
    }
}
