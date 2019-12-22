using System.Net.Http;
using System.Threading.Tasks;
using Bookings.DAL;
using Bookings.IntegrationTests.Contexts;
using Bookings.IntegrationTests.Helper;
using Microsoft.EntityFrameworkCore;

namespace Bookings.IntegrationTests.Steps
{
    public abstract class StepsBase
    {
        protected DbContextOptions<BookingsDbContext> BookingsDbContextOptions;
        protected TestDataManager TestDataManager { get; set; }
        protected readonly TestContext Context;

        protected StepsBase(TestContext apiTextContext)
        {
            Context = apiTextContext;
            BookingsDbContextOptions = apiTextContext.BookingsDbContextOptions;
            TestDataManager = apiTextContext.TestDataManager;
        }

        protected async Task<HttpResponseMessage> SendGetRequestAsync(TestContext apiTestContext)
        {
            using (var client = apiTestContext.Server.CreateClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiTestContext.BookingsApiToken}");
                return await client.GetAsync(apiTestContext.Uri);
            }
        }

        protected async Task<HttpResponseMessage> SendPatchRequestAsync(TestContext apiTestContext)
        {
            using (var client = apiTestContext.Server.CreateClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiTestContext.BookingsApiToken}");
                return await client.PatchAsync(apiTestContext.Uri, apiTestContext.HttpContent);
            }
        }

        protected async Task<HttpResponseMessage> SendPostRequestAsync(TestContext apiTestContext)
        {
            using (var client = apiTestContext.Server.CreateClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiTestContext.BookingsApiToken}");
                return await client.PostAsync(apiTestContext.Uri, apiTestContext.HttpContent);
            }
        }

        protected async Task<HttpResponseMessage> SendPutRequestAsync(TestContext apiTestContext)
        {
            using (var client = apiTestContext.Server.CreateClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiTestContext.BookingsApiToken}");
                return await client.PutAsync(apiTestContext.Uri, apiTestContext.HttpContent);
            }
        }

        protected async Task<HttpResponseMessage> SendDeleteRequestAsync(TestContext apiTestContext)
        {
            using (var client = apiTestContext.Server.CreateClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiTestContext.BookingsApiToken}");
                return await client.DeleteAsync(apiTestContext.Uri);
            }
        }
    }
}
