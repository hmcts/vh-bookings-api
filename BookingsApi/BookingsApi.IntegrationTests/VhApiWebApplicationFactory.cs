using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using GST.Fake.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

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
        }
        
        protected override void ConfigureClient(HttpClient client)
        {
            base.ConfigureClient(client);
            client.Timeout = System.TimeSpan.FromMinutes(20); // this should stop timeouts and cancelled ops when debugging
            client.SetFakeBearerToken("admin", ["ROLE_ADMIN", "ROLE_GENTLEMAN"]);
        }
    }
}