using System.Net.Http;
using System.Net.Http.Headers;
using BookingsApi.Client;
using BookingsApi.Common.Configuration;
using BookingsApi.Common.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace BookingsApi.AcceptanceTests.Api;

public abstract class ApiTest
{
    protected IConfigurationRoot _configRoot;
    protected BookingsApiClient _client;
    
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        var userSecretsId = "D76B6EB8-F1A2-4A51-9B8F-21E1B6B81E4F";
        _configRoot = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddUserSecrets(userSecretsId)
            .AddEnvironmentVariables().Build();
        
        // generate token
        var azureOptions = Options.Create(_configRoot.GetSection("AzureAd").Get<AzureAdConfiguration>());
        var serviceConfiguration = _configRoot.GetSection("Services").Get<ServicesConfiguration>();
        var apiToken = new AzureTokenProvider(azureOptions).GetClientAccessToken(
            azureOptions.Value.ClientId, azureOptions.Value.ClientSecret, serviceConfiguration.BookingsApiResourceId);
        
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("bearer", apiToken);
        _client = BookingsApiClient.GetClient(serviceConfiguration.BookingsApiUrl, httpClient);
    }
}