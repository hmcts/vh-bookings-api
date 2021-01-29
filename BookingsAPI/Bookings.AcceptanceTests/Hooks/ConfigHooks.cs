﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api;
using AcceptanceTests.Common.Configuration;
using AcceptanceTests.Common.Configuration.Users;
using AcceptanceTests.Common.Exceptions;
using Bookings.AcceptanceTests.Contexts;
using Bookings.Api.Contract.Responses;
using Bookings.Common.Configuration;
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
            _configRoot = ConfigurationManager.BuildConfig("D76B6EB8-F1A2-4A51-9B8F-21E1B6B81E4F", "1600292d-7269-4724-8a03-108544edbbc6");
            context.Config = new Config();
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
            context.Config.ServicesConfiguration = GetTargetTestEnvironment() == string.Empty
                ? Options.Create(_configRoot.GetSection("Services").Get<ServicesConfiguration>()).Value
                : Options.Create(_configRoot.GetSection($"Testing.{GetTargetTestEnvironment()}.Services").Get<ServicesConfiguration>()).Value;
            if (context.Config.ServicesConfiguration == null && GetTargetTestEnvironment() != string.Empty) throw new TestSecretsFileMissingException(GetTargetTestEnvironment());
            ConfigurationManager.VerifyConfigValuesSet(context.Config.ServicesConfiguration);
        }

        private void RegisterTestSettings(TestContext context)
        {
            context.Config.TestSettings = Options.Create(_configRoot.GetSection("Testing").Get<TestSettings>()).Value;
            ConfigurationManager.VerifyConfigValuesSet(context.Config.TestSettings);
        }

        private void RegisterTestUsers(TestContext context)
        {
            List<UserAccount> userAccounts = new ConfigurationBuilder()
                .AddJsonFile("useraccounts.json")
                .Build()
                .GetSection("UserAccounts")
                .Get<List<UserAccount>>();

            context.UserAccounts = userAccounts;
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

        private static string GetTargetTestEnvironment()
        {
            return NUnit.Framework.TestContext.Parameters["TargetTestEnvironment"] ?? string.Empty;
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
