using System.Collections.Generic;
using System.Net.Http;
using AcceptanceTests.Common.Api;
using AcceptanceTests.Common.Configuration.Users;
using Bookings.DAL;
using Bookings.IntegrationTests.Helper;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Testing.Common.Configuration;

namespace Bookings.IntegrationTests.Contexts
{
    public class TestContext
    {
        public string BearerToken { get; set; }
        public DbContextOptions<BookingsDbContext> BookingsDbContextOptions { get; set; }
        public Config Config { get; set; }
        public HttpContent HttpContent { get; set; }
        public HttpMethod HttpMethod { get; set; }
        public HttpResponseMessage Response { get; set; }
        public TestServer Server { get; set; }
        public TestData TestData { get; set; }
        public TestDataManager TestDataManager { get; set; }
        public string Uri { get; set; }
        public List<UserAccount> UserAccounts { get; set; }

        public HttpClient CreateClient()
        {
            HttpClient client;
            if (Zap.SetupProxy)
            {
                var handler = new HttpClientHandler
                {
                    Proxy = Zap.WebProxy,
                    UseProxy = true,
                };

                client = new HttpClient(handler)
                {
                    BaseAddress = new System.Uri(Config.ServicesConfiguration.BookingsApiUrl)
                };
            }
            else
            {
                client = Server.CreateClient();
            }

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {BearerToken}");
            return client;
        }
    }
}