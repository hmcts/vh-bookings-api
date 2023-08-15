using AcceptanceTests.Common.Configuration.Users;
using BookingsApi.Common.Configuration;
using BookingsApi.Common.Services;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using BookingsApi.IntegrationTests.Contexts;
using GST.Fake.Authentication.JwtBearer;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;
using Testing.Common.Configuration;
using Testing.Common.Stubs;
using ConfigurationManager = AcceptanceTests.Common.Configuration.ConfigurationManager;
using TestContext = BookingsApi.IntegrationTests.Contexts.TestContext;
using TestData = Testing.Common.Configuration.TestData;

namespace BookingsApi.IntegrationTests.Hooks
{
    [Binding]
    public class ConfigHooks
    {
        private readonly IConfigurationRoot _configRoot;

        public ConfigHooks(TestContext context)
        {
            _configRoot = ConfigRootBuilder.Build();
            context.Config = new Config();
            context.UserAccounts = new List<UserAccount>();
        }

        [BeforeScenario(Order = (int)HooksSequence.ConfigHooks)]
        public void RegisterSecrets(TestContext context)
        {
            RegisterHearingServices(context);
            RegisterTestSettings(context);
            RegisterDefaultData(context);
            RegisterDatabaseSettings(context);
            RegisterServer(context);
            RegisterApiSettings(context);
        }

        private void RegisterHearingServices(TestContext context)
        {
            context.Config.ServicesConfiguration = _configRoot.GetSection("Services").Get<ServicesConfiguration>();
            context.Config.ServicesConfiguration.BookingsApiResourceId.Should().NotBeNullOrEmpty();
        }

        private void RegisterTestSettings(TestContext context)
        {
            context.Config.TestSettings = _configRoot.GetSection("Testing").Get<TestSettings>();
            ConfigurationManager.VerifyConfigValuesSet(context.Config.TestSettings);
        }

        private static void RegisterDefaultData(TestContext context)
        {
            context.TestData = new TestData
            {
                CaseName = "Bookings Api Integration Test",
                Participants = new List<ParticipantRequest>(),
                RemovedPersons = new List<string>()
            };
            context.TestData.CaseName.Should().NotBeNullOrEmpty();
        }

        private void RegisterDatabaseSettings(TestContext context)
        {
            context.Config.ConnectionStrings = _configRoot.GetSection("ConnectionStrings").Get<ConnectionStrings>();
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<BookingsDbContext>();
            dbContextOptionsBuilder.UseSqlServer(context.Config.ConnectionStrings.VhBookings);
            context.BookingsDbContextOptions = dbContextOptionsBuilder.Options;
            context.TestDataManager = new TestDataManager(context.BookingsDbContextOptions, context.TestData.CaseName);
        }

        private static void RegisterServer(TestContext context)
        {
            var webHostBuilder = WebHost.CreateDefaultBuilder()
                    .UseKestrel(c => c.AddServerHeader = false)
                    .UseEnvironment("Development")
                    .UseStartup<Startup>()
                    .ConfigureTestServices(services =>
                    {
                        services.AddAuthentication(options =>
                        {
                            options.DefaultScheme = FakeJwtBearerDefaults.AuthenticationScheme;
                            options.DefaultAuthenticateScheme = FakeJwtBearerDefaults.AuthenticationScheme;
                            options.DefaultChallengeScheme = FakeJwtBearerDefaults.AuthenticationScheme;
                        }).AddFakeJwtBearer();
                        
                        RegisterStubs(services);
                    });
            context.Server = new TestServer(webHostBuilder);
        }

        private static void RegisterStubs(IServiceCollection services)
        {
            services.AddSingleton<IServiceBusQueueClient, ServiceBusQueueClientFake>();
            services.AddSingleton<IFeatureToggles, FeatureTogglesStub>();
        }

        private static void RegisterApiSettings(TestContext context)
        {
            context.Response = new HttpResponseMessage();
        }
    }
}
