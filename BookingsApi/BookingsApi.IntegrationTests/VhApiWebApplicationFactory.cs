using System.Net.Http;
using BookingsApi.Common.Services;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using GST.Fake.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Testing.Common.Stubs;

namespace BookingsApi.IntegrationTests
{
    public class VhApiWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = FakeJwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = FakeJwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = FakeJwtBearerDefaults.AuthenticationScheme;
                }).AddFakeJwtBearer();
                
                RegisterStubs(services);
            });
            builder.UseEnvironment("Development");
        }
        
        private static void RegisterStubs(IServiceCollection services)
        {
            services.AddSingleton<IServiceBusQueueClient, ServiceBusQueueClientFake>();
            services.AddSingleton<IFeatureToggles>(new FeatureTogglesStub());
        }
        
        protected override void ConfigureClient(HttpClient client)
        {
            base.ConfigureClient(client);
            client.SetFakeBearerToken("admin", new[] { "ROLE_ADMIN", "ROLE_GENTLEMAN" });
        }
    }
}