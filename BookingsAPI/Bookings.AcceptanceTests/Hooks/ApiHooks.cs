using System;
using System.Net;
using Bookings.AcceptanceTests.Contexts;
using Bookings.AcceptanceTests.Helpers;
using Bookings.API;
using Bookings.Common.Configuration;
using Bookings.Common.Security;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;
using Testing.Common.Builders.Domain;
using Testing.Common.Configuration;

namespace Bookings.AcceptanceTests.Hooks
{
    [Binding]
    public static class ApiHooks
    {
        [BeforeTestRun]
        public static void OneTimeSetup(TestContext testContext)
        {
            var configRootBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddUserSecrets<Startup>();

            var configRoot = configRootBuilder.Build();

            var azureAdConfig = Options.Create(configRoot.GetSection("AzureAd").Get<AzureAdConfiguration>());
            var testConfig = Options.Create(configRoot.GetSection("Testing").Get<TestSettings>());
            var tokenProvider = new TokenProvider();

            testContext.BookingsApiToken = tokenProvider.GetClientAccessToken
            (
                azureAdConfig.Value.TenantId,
                azureAdConfig.Value.ClientId,
                azureAdConfig.Value.ClientSecret,
                new []{ $"{azureAdConfig.Value.Scope}/.default" }
            );

            var apiTestsOptions =
                Options.Create(configRoot.GetSection("AcceptanceTestSettings").Get<AcceptanceTestConfiguration>());
            var apiTestSettings = apiTestsOptions.Value;
            testContext.BaseUrl = apiTestSettings.BookingsApiBaseUrl;
        }

        [BeforeTestRun]
        public static void CheckHealth(TestContext testContext)
        {
            var endpoint = new ApiUriFactory().HealthCheckEndpoints;
            testContext.Request = testContext.Get(endpoint.HealthCheck);
            testContext.Response = testContext.Client().Execute(testContext.Request);
            testContext.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [AfterScenario]
        public static void TearDown(TestContext testContext)
        {
            if (testContext.HearingId == Guid.Empty) return;
            var endpoint = new ApiUriFactory().HearingsEndpoints.RemoveHearing(testContext.HearingId);
            testContext.Request = testContext.Delete(endpoint);
            testContext.Response = testContext.Client().Execute(testContext.Request);
        }
    }
}
