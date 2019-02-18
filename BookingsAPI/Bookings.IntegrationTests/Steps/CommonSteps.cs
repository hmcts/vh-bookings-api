using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Bookings.IntegrationTests.Api;
using Bookings.IntegrationTests.Contexts;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Bookings.IntegrationTests.Steps
{
    [Binding]
    public sealed class CommonSteps : ControllerTestsBase
    {
        private readonly ScenarioContext context;
        private readonly ApiTestContext _apiTestContext;

        public CommonSteps(ScenarioContext injectedContext, ApiTestContext apiTestContext)
        {
            context = injectedContext;
            _apiTestContext = apiTestContext;
        }

        [When(@"I send the request to the endpoint")]
        public async Task WhenISendTheRequestToTheEndpoint()
        {
            _apiTestContext.ResponseMessage = new HttpResponseMessage();
            switch (_apiTestContext.HttpMethod.Method)
            {
                case "GET": _apiTestContext.ResponseMessage = await SendGetRequestAsync(_apiTestContext); break;
                case "POST": _apiTestContext.ResponseMessage = await SendPostRequestAsync(_apiTestContext.Uri, _apiTestContext.HttpContent); break;
                case "PATCH": _apiTestContext.ResponseMessage = await SendPatchRequestAsync(_apiTestContext.Uri, _apiTestContext.StringContent); break;
                case "PUT": _apiTestContext.ResponseMessage = await SendPutRequestAsync(_apiTestContext.Uri, _apiTestContext.StringContent); break;
                case "DELETE": _apiTestContext.ResponseMessage = await SendDeleteRequestAsync(_apiTestContext.Uri); break;
                default: throw new ArgumentOutOfRangeException(_apiTestContext.HttpMethod.ToString(), _apiTestContext.HttpMethod.ToString(), null);
            }
        }

        [Then(@"the response should have the status (.*) and success status (.*)")]
        public void ThenTheResponseShouldHaveStatus(HttpStatusCode statusCode, bool IsSuccess)
        {
            _apiTestContext.ResponseMessage.StatusCode.Should().Be(statusCode);
            _apiTestContext.ResponseMessage.IsSuccessStatusCode.Should().Be(IsSuccess);
            TestContext.WriteLine($"Status Code: {_apiTestContext.ResponseMessage.StatusCode}");
        }
    }
}
