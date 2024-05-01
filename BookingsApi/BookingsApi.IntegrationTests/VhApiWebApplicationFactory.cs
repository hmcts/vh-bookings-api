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
                const string fakeJwtBearerScheme = "Fake Bearer";
                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = fakeJwtBearerScheme;
                    options.DefaultAuthenticateScheme = fakeJwtBearerScheme;
                    options.DefaultChallengeScheme = fakeJwtBearerScheme;
                }).AddFakeJwtBearer(fakeJwtBearerScheme, options => _ = options);
                
                RegisterStubs(services);
            });
            builder.UseEnvironment("Development");
        }
        
        private static void RegisterStubs(IServiceCollection services)
        {
            services.AddSingleton<IServiceBusQueueClient, ServiceBusQueueClientFake>();
            services.AddSingleton<IFeatureToggles, FeatureTogglesStub>();
        }
        
        protected override void ConfigureClient(HttpClient client)
        {
            base.ConfigureClient(client);
            client.SetFakeBearerToken("admin", new[] { "ROLE_ADMIN", "ROLE_GENTLEMAN" });
        }
    }
}