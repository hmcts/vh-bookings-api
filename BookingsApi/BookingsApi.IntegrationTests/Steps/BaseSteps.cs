using System.Net.Http;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.IntegrationTests.Contexts;
using BookingsApi.IntegrationTests.Helper;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.IntegrationTests.Steps
{
    public abstract class BaseSteps
    {
        protected DbContextOptions<BookingsDbContext> BookingsDbContextOptions;
        protected TestDataManager TestDataManager { get; set; }
        protected readonly TestContext Context;

        protected BaseSteps(TestContext testContext)
        {
            Context = testContext;
            BookingsDbContextOptions = testContext.BookingsDbContextOptions;
            TestDataManager = testContext.TestDataManager;
        }

        protected async Task<HttpResponseMessage> SendGetRequestAsync(TestContext apiTestContext)
        {
            using var client = apiTestContext.CreateClient();
            return await client.GetAsync(apiTestContext.Uri);
        }

        protected async Task<HttpResponseMessage> SendPatchRequestAsync(TestContext apiTestContext)
        {
            using var client = apiTestContext.CreateClient();
            return await client.PatchAsync(apiTestContext.Uri, apiTestContext.HttpContent);
        }

        protected async Task<HttpResponseMessage> SendPostRequestAsync(TestContext apiTestContext)
        {
            using var client = apiTestContext.CreateClient();
            return await client.PostAsync(apiTestContext.Uri, apiTestContext.HttpContent);
        }

        protected async Task<HttpResponseMessage> SendPutRequestAsync(TestContext apiTestContext)
        {
            using var client = apiTestContext.CreateClient();
            return await client.PutAsync(apiTestContext.Uri, apiTestContext.HttpContent);
        }

        protected async Task<HttpResponseMessage> SendDeleteRequestAsync(TestContext apiTestContext)
        {
            using var client = apiTestContext.CreateClient();
            return await client.DeleteAsync(apiTestContext.Uri);
        }
    }
}
