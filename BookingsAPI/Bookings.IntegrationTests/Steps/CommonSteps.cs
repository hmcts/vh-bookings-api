using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace Bookings.IntegrationTests.Steps
{
    [Binding]
    public sealed class CommonSteps : StepsBase
    {
        public CommonSteps(Contexts.TestContext apiTestContext) : base(apiTestContext)
        {
        }

        [When(@"I send the request to the endpoint")]
        [When(@"I send the same request twice")]
        public async Task WhenISendTheRequestToTheEndpoint()
        {
            ApiTestContext.ResponseMessage = new HttpResponseMessage();
            switch (ApiTestContext.HttpMethod.Method)
            {
                case "GET": ApiTestContext.ResponseMessage = await SendGetRequestAsync(ApiTestContext); break;
                case "POST": ApiTestContext.ResponseMessage = await SendPostRequestAsync(ApiTestContext); break;
                case "PATCH": ApiTestContext.ResponseMessage = await SendPatchRequestAsync(ApiTestContext); break;
                case "PUT": ApiTestContext.ResponseMessage = await SendPutRequestAsync(ApiTestContext); break;
                case "DELETE": ApiTestContext.ResponseMessage = await SendDeleteRequestAsync(ApiTestContext); break;
                default: throw new ArgumentOutOfRangeException(ApiTestContext.HttpMethod.ToString(), ApiTestContext.HttpMethod.ToString(), null);
            }
        }

        [Then(@"the response should have the status (.*) and success status (.*)")]
        public void ThenTheResponseShouldHaveStatus(HttpStatusCode statusCode, bool isSuccess)
        {
            ApiTestContext.ResponseMessage.StatusCode.Should().Be(statusCode);
            ApiTestContext.ResponseMessage.IsSuccessStatusCode.Should().Be(isSuccess);
            NUnit.Framework.TestContext.WriteLine($"Status Code: {ApiTestContext.ResponseMessage.StatusCode}");
        }

        [Then(@"the response message should read '(.*)'")]
        [Then(@"the error response message should contain '(.*)'")]
        [Then(@"the error response message should also contain '(.*)'")]
        public void ThenTheResponseShouldContain(string errorMessage)
        {
            ApiTestContext.ResponseMessage.Content.ReadAsStringAsync().Result.Should().Contain(errorMessage);
        }
    }
}
