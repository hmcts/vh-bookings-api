using RestSharp;
using System.Collections.Generic;
using AcceptanceTests.Common.Api.Helpers;
using AcceptanceTests.Common.Configuration.Users;
using Microsoft.EntityFrameworkCore;
using Testing.Common.Configuration;
using AcceptanceTests.Common.Api;
using BookingsApi.DAL;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;

namespace BookingsApi.AcceptanceTests.Contexts
{
    public class TestContext
    {
        public string BearerToken { get; set; }
        public DbContextOptions<BookingsDbContext> BookingsDbContextOptions { get; set; }
        public Config Config { get; set; }
        public RestRequest Request { get; set; }
        public IRestResponse Response { get; set; }
        public TestData TestData { get; set; }
        public List<UserAccount> UserAccounts { get; set; }

        public RestClient Client()
        {
            var client = new RestClient(Config.ServicesConfiguration.BookingsApiUrl) { Proxy = Zap.WebProxy };
            client.AddDefaultHeader("Accept", "application/json");
            client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");
            return client;
        }

        public RestRequest Get(string path) => new RestRequest(path, Method.GET);

        public RestRequest Post(string path, object requestBody)
        {
            var request = new RestRequest(path, Method.POST);
            request.AddParameter("Application/json", RequestHelper.Serialise(requestBody),
                ParameterType.RequestBody);
            return request;
        }

        public RestRequest Delete(string path) => new RestRequest(path, Method.DELETE);

        public RestRequest Put(string path, object requestBody)
        {
            var request = new RestRequest(path, Method.PUT);
            request.AddParameter("Application/json", RequestHelper.Serialise(requestBody),
                ParameterType.RequestBody);
            return request;
        }

        public RestRequest Patch(string path, object requestBody = null)
        {
            var request = new RestRequest(path, Method.PATCH);
            request.AddParameter("Application/json", RequestHelper.Serialise(requestBody),
                ParameterType.RequestBody);
            return request;
        }
    }

    public class TestHttpClient
    {
        public async Task<HttpResponseMessage> ExecuteAsync<T>(TestContext context, string relativePath, T data, HttpMethod httpMethod)
            where T : class
        {
            var handler = new HttpClientHandler
            {
                Proxy = Zap.WebProxy,
            };

            using var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(context.Config.ServicesConfiguration.BookingsApiUrl)
            };

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {context.BearerToken}");

            var uri = client.BaseAddress + relativePath;

            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(
                new HttpRequestMessage
                {
                    Content = content,
                    Method = httpMethod,
                    RequestUri = new Uri(uri)
                });

            return response;
        }
    }
}