using Bookings.AcceptanceTests.Contexts;
using Bookings.API;
using Bookings.Common.Configuration;
using Bookings.Common.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TechTalk.SpecFlow;
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
            testContext.BearerToken = GetClientAccessTokenForApi();
        }

        private static string GetClientAccessTokenForApi()
        {
            var configRootBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddUserSecrets<Startup>();

            var configRoot = configRootBuilder.Build();

            var azureAdConfigurationOptions = Options.Create(configRoot.GetSection("AzureAd").Get<AzureAdConfiguration>());
            var testSettingsOptions = Options.Create(configRoot.GetSection("Testing").Get<TestSettings>());

            var azureAdConfiguration = azureAdConfigurationOptions.Value;
            var testSettings = testSettingsOptions.Value;

            return new AzureTokenProvider(azureAdConfigurationOptions).GetClientAccessToken(
                testSettings.TestClientId, testSettings.TestClientSecret,
                azureAdConfiguration.VhBookingsApiResourceId);
        }

        [AfterTestRun]
        public static void TearDown()
        {
            // Method will be used to cleanup data from tests that require data setup
        }
    }
}
