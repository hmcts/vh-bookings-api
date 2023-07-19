using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BookingsApi.Client;
using BookingsApi.Common.Configuration;
using BookingsApi.Common.Security;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Requests.Enums;
using BookingsApi.Contract.Responses;
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
    public async Task OneTimeSetup()
    {
        _configRoot = ConfigRootBuilder.Build();

        RegisterSettings();
        var apiToken = await GenerateApiToken();
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

    private async Task<string> GenerateApiToken()
    {
        return await new AzureTokenProvider(Options.Create(_azureConfiguration)).GetClientAccessToken(_azureConfiguration.ClientId,
            _azureConfiguration.ClientSecret, _serviceConfiguration.BookingsApiResourceId);
    }
    
    protected async Task<JusticeUserResponse> CreateJusticeUser()
    {
        var username = $"automation.allocation{TestSettings.UsernameStem}";
        TestContext.WriteLine("Removed hearing");
        var request = new AddJusticeUserRequest()
        {
            Username = username,
            ContactEmail = username,
            FirstName = "automation",
            LastName = "allocation",
            Roles = new List<JusticeUserRole>() { JusticeUserRole.Vho },
            CreatedBy = "automationtest"
        };
        var cso = await BookingsApiClient.AddJusticeUserAsync(request);
        TestContext.WriteLine($"Created justice user {cso.Id} - {cso.Username}");
        return cso;
    }
}