using System;
using System.Net.Http;
using BookingsApi.DAL;
using BookingsApi.IntegrationTests.Contexts;
using Microsoft.EntityFrameworkCore;
using TestContext = BookingsApi.IntegrationTests.Contexts.TestContext;

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
        protected async Task<HttpResponseMessage> SendRequestAsync(TestContext context)
        {
            using var client = context.CreateClient();

            var requestUri = new Uri($"{client.BaseAddress}{context.Uri}");

            return await client.SendAsync(
                   new HttpRequestMessage {
                       Content = context.HttpContent, 
                       Method = context.HttpMethod, 
                       RequestUri = requestUri });
        }
    }
}
