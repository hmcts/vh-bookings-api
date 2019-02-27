using Bookings.Common.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Testing.Common.Configuration
{
    public class BookingsConfigLoader
    {
        private readonly IConfigurationRoot _configRoot;

        public BookingsConfigLoader()
        {
            var configRootBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddUserSecrets("d76b6eb8-f1a2-4a51-9b8f-21e1b6b81e4f");
            _configRoot = configRootBuilder.Build();
        }
        
        public AzureAdConfiguration ReadAzureAdSettings()
        {
            var azureAdConfig = Options.Create(_configRoot.GetSection("Testing").Get<AzureAdConfiguration>());
            return azureAdConfig.Value;
        }
        
        public TestSettings ReadTestSettings()
        {
            var testSettingsOptions = Options.Create(_configRoot.GetSection("Testing").Get<TestSettings>());
            return testSettingsOptions.Value;
        }

        public IConfigurationRoot GetRoot()
        {
            return _configRoot;
        }
    }
}