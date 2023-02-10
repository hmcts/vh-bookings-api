using System.Net.Http;
using System.Net.Http.Headers;
using BookingsApi.Client;
using BookingsApi.Common.Configuration;
using BookingsApi.Common.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NUnit.Framework;

namespace BookingsApi.AcceptanceTests.Api;

public abstract class ApiTest
{
    private IConfigurationRoot _configRoot;
    private AzureAdConfiguration _azureConfiguration;
    private ServicesConfiguration _serviceConfiguration;
    protected BookingsApiClient Client;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        var userSecretsId = "D76B6EB8-F1A2-4A51-9B8F-21E1B6B81E4F";
        _configRoot = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Production.json", true) // CI write variables in the pipeline to this file
            .AddUserSecrets(userSecretsId)
            .AddEnvironmentVariables()
            .Build();

        RegisterSettings();
        var apiToken = GenerateApiToken();
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("bearer", apiToken);
        Client = BookingsApiClient.GetClient(_serviceConfiguration.BookingsApiUrl, httpClient);
    }

    private void RegisterSettings()
    {
        _azureConfiguration = _configRoot.GetSection("AzureAd").Get<AzureAdConfiguration>();
        _serviceConfiguration = _configRoot.GetSection("Services").Get<ServicesConfiguration>();
    }

    private string GenerateApiToken()
    {
        TestContext.Progress.WriteLine(JsonConvert.SerializeObject(_azureConfiguration));
        return new AzureTokenProvider(Options.Create(_azureConfiguration)).GetClientAccessToken(_azureConfiguration.ClientId,
            _azureConfiguration.ClientSecret, _serviceConfiguration.BookingsApiResourceId);
    }
}