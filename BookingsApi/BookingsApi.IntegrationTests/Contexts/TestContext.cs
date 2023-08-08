using System.Net.Http;
using AcceptanceTests.Common.Api;
using AcceptanceTests.Common.Configuration.Users;
using BookingsApi.DAL;
using GST.Fake.Authentication.JwtBearer;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Testing.Common.Configuration;

namespace BookingsApi.IntegrationTests.Contexts
{
    public class TestContext
    {
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

            client.SetFakeBearerToken("admin", new[] { "ROLE_ADMIN", "ROLE_GENTLEMAN" });
            return client;
        }
    }
}