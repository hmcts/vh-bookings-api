using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BookingsApi.Client;
using BookingsApi.Common.Configuration;
using BookingsApi.Common.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NUnit.Framework;
using VHConfigurationManager = AcceptanceTests.Common.Configuration.ConfigurationManager;

namespace BookingsApi.AcceptanceTests.Api;

public abstract class ApiTest
{
    protected IConfigurationRoot _configRoot;
    protected BookingsApiClient _client;
    private AzureAdConfiguration _azureConfiguration;
    private ServicesConfiguration _serviceConfiguration;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        var userSecretsId = "D76B6EB8-F1A2-4A51-9B8F-21E1B6B81E4F";
        _configRoot = VHConfigurationManager.BuildConfig(userSecretsId);
        // _configRoot = new ConfigurationBuilder()
        //     .AddJsonFile("appsettings.json")
        //     .AddUserSecrets(userSecretsId)
        //     // .AddEnvironmentVariables()
        //     .Build();

        RegisterSettings();
        var apiToken = GenerateApiToken();
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("bearer", apiToken);
        _client = BookingsApiClient.GetClient(_serviceConfiguration.BookingsApiUrl, httpClient);
    }

    private void RegisterSettings()
    {
        _azureConfiguration = _configRoot.GetSection("AzureAd").Get<AzureAdConfiguration>();
        VHConfigurationManager.VerifyConfigValuesSet(_azureConfiguration);

        _serviceConfiguration = _configRoot.GetSection("Services").Get<ServicesConfiguration>();
        VHConfigurationManager.VerifyConfigValuesSet(_serviceConfiguration);
    }

    private string GenerateApiToken()
    {
        // We should get rid of this and use azure the AzureTokenProvider we have in the Common Project
     
        TestContext.Progress.WriteLine(JsonConvert.SerializeObject(_azureConfiguration));
        // return VHConfigurationManager.GetBearerToken(adConfig, _serviceConfiguration.BookingsApiResourceId);
        return new AzureTokenProvider(Options.Create(_azureConfiguration)).GetClientAccessToken(_azureConfiguration.ClientId,
            _azureConfiguration.ClientSecret, _serviceConfiguration.BookingsApiResourceId);
    }
}