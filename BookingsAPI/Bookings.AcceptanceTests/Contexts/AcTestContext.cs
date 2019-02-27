using System.Configuration;
using RestSharp;
using Testing.Common.Builders.Api;

namespace Bookings.AcceptanceTests.Contexts
{
    public class AcTestContext
    {
        public RestRequest Request { get; set; }
        public IRestResponse Response { get; set; }
        public string BearerToken { get; set; }

        public RestClient Client()
        {
            var client = new RestClient(ConfigurationManager.AppSettings["BaseUrl"]);
            client.AddDefaultHeader("Accept", "application/json");
            client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");
            return client;
        }

        public RestRequest Get(string path) => new RestRequest(path, Method.GET);

        public RestRequest Post(string path, object requestBody)
        {
            var request = new RestRequest(path, Method.POST);
            request.AddParameter("Application/json", ApiRequestHelper.SerialiseRequestToSnakeCaseJson(requestBody), ParameterType.RequestBody);
            return request;
        }

        public RestRequest Delete(string path) => new RestRequest(path, Method.DELETE);

        public RestRequest Put(string path, object requestBody)
        {
            var request = new RestRequest(path, Method.PUT);
            request.AddParameter("Application/json", ApiRequestHelper.SerialiseRequestToSnakeCaseJson(requestBody), ParameterType.RequestBody);
            return request;
        }

        public RestRequest Patch(string path, object requestBody = null)
        {
            var request = new RestRequest(path, Method.PATCH);
            request.AddParameter("Application/json", ApiRequestHelper.SerialiseRequestToSnakeCaseJson(requestBody), ParameterType.RequestBody);
            return request;
        }
    }
}