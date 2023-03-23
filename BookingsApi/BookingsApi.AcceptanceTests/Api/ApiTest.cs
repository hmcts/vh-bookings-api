using System.Net.Http;
using System.Net.Http.Headers;
using BookingsApi.Client;
using BookingsApi.Common.Configuration;
using BookingsApi.Common.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Testing.Common.Configuration;

namespace BookingsApi.AcceptanceTests.Api;

public abstract class ApiTest
{
    private IConfigurationRoot _configRoot;
    private AzureAdConfiguration _azureConfiguration;
    private ServicesConfiguration _serviceConfiguration;
    protected TestSettings TestSettings;
    protected BookingsApiClient BookingsApiClient;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _configRoot = ConfigRootBuilder.Build();

        RegisterSettings();
        var apiToken = GenerateApiToken();
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("bearer", apiToken);
        BookingsApiClient = BookingsApiClient.GetClient(_serviceConfiguration.BookingsApiUrl, httpClient);
    }

    private void RegisterSettings()
    {
        _azureConfiguration = _configRoot.GetSection("AzureAd").Get<AzureAdConfiguration>();
        _serviceConfiguration = _configRoot.GetSection("Services").Get<ServicesConfiguration>();
        TestSettings = _configRoot.GetSection("Testing").Get<TestSettings>();
    }

    private string GenerateApiToken()
    {
        return new AzureTokenProvider(Options.Create(_azureConfiguration)).GetClientAccessToken(_azureConfiguration.ClientId,
            _azureConfiguration.ClientSecret, _serviceConfiguration.BookingsApiResourceId);
    }
}