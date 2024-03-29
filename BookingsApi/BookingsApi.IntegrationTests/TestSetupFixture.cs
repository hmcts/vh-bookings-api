using AcceptanceTests.Common.Api;
using BookingsApi.Common.Configuration;
using Microsoft.Extensions.Configuration;

namespace BookingsApi.IntegrationTests
{
    [SetUpFixture]
    public class TestSetupFixture
    {
        private ServicesConfiguration ServicesConfiguration => new ConfigurationBuilder()
                                                            .AddJsonFile("appsettings.json")
                                                            .Build()
                                                            .GetSection("Services")
                                                            .Get<ServicesConfiguration>();

        [OneTimeSetUp]
        public void ZapStart()
        {
            Zap.Start();
        }

        [OneTimeTearDown]
        public void ZapReport()
        {
            Zap.ReportAndShutDown("BookingsApi-Integration", ServicesConfiguration.BookingsApiUrl);
        }
    }
}
