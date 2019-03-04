using System.Collections.Generic;
using System.Net;
using Bookings.AcceptanceTests.Contexts;
using Bookings.AcceptanceTests.Helpers;
using Bookings.API;
using Bookings.Common.Configuration;
using Bookings.Common.Security;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;
using Testing.Common.Configuration;

namespace Bookings.AcceptanceTests.Steps
{
    [Binding]
    public abstract class StepsBase
    {
        protected StepsBase()
        {
        }

        [BeforeTestRun]
        public static void OneTimeSetup(AcTestContext testContext)
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

            testContext.BearerToken = new AzureTokenProvider(azureAdConfigurationOptions).GetClientAccessToken(
                testSettings.TestClientId, testSettings.TestClientSecret,
                azureAdConfiguration.VhBookingsApiResourceId);

            var apiTestsOptions =
                Options.Create(configRoot.GetSection("AcceptanceTestSettings").Get<AcceptanceTestConfiguration>());
            var apiTestSettings = apiTestsOptions.Value;
            testContext.BaseUrl = apiTestSettings.BookingsApiBaseUrl;
        }

        [BeforeTestRun]
        public static void CheckHealth(AcTestContext testContext)
        {
            var endpoint = new ApiUriFactory().HealthCheckEndpoints;
            testContext.Request = testContext.Get(endpoint.HealthCheck);
            testContext.Response = testContext.Client().Execute(testContext.Request);
            testContext.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [AfterScenario]
        public static void TearDown(AcTestContext testContext)
        {
            if (testContext.HearingId != null)
            {
                //testContext.Request = testContext.Delete(_endpoints.DeleteHearing(testContext.HearingId));
                //testContext.Response = testContext.Client().Execute(testContext.Request);
                //testContext.Response.Should().Be(HttpStatusCode.OK);
            }
        }
    }
}