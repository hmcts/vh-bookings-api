using RestSharp;
using System.Collections.Generic;
using AcceptanceTests.Common.Api.Helpers;
using AcceptanceTests.Common.Configuration.Users;
using Microsoft.EntityFrameworkCore;
using Testing.Common.Configuration;
using AcceptanceTests.Common.Api;
using BookingsApi.DAL;

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
            var client = new RestClient(Config.ServicesConfiguration.BookingsApiUrl) { Proxy = Zap.WebProxy};
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
}