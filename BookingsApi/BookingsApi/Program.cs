using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using VH.Core.Configuration;

namespace BookingsApi
{
    public class Program
    {
        protected Program()
        {
        }

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

// ReSharper disable once MemberCanBePrivate.Global Needed for client generation on build with nswag
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            const string vhInfraCore = "/mnt/secrets/vh-infra-core";
			const string vhBookingsApi = "/mnt/secrets/vh-bookings-api";

            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((configBuilder) =>
                {
                    configBuilder.AddAksKeyVaultSecretProvider(vhInfraCore);
					configBuilder.AddAksKeyVaultSecretProvider(vhBookingsApi);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                    webBuilder.UseIISIntegration();
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureAppConfiguration((configBuilder) =>
                    {
                        configBuilder.AddAksKeyVaultSecretProvider(vhInfraCore);
						configBuilder.AddAksKeyVaultSecretProvider(vhBookingsApi);
                    });
                });
        }
    }
}