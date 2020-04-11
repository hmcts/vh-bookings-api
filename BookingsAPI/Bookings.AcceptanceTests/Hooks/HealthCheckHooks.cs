using AcceptanceTests.Common.Api;
using AcceptanceTests.Common.Api.Healthchecks;
using Bookings.AcceptanceTests.Contexts;
using System.Net;
using TechTalk.SpecFlow;

namespace Bookings.AcceptanceTests.Hooks
{
    [Binding]
    public static class HealthCheckHooks
    {
        [BeforeScenario(Order = (int)HooksSequence.HealthCheckHooks)]
        public static void CheckApiHealth(TestContext context)
        {
            CheckVideoApiHealth(context.Config.ServicesConfiguration.BookingsApiUrl, context.BearerToken);
        }
        private static void CheckVideoApiHealth(string apiUrl, string bearerToken)
        {
            HealthcheckManager.CheckHealthOfBookingsApi(apiUrl, bearerToken, (WebProxy) Zap.WebProxy);
        }
    }
}
