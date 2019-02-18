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
                case "GET": _apiTestContext.ResponseMessage = await SendGetRequest(_apiTestContext); break;
                case "POST": _apiTestContext.ResponseMessage = SendPostRequestAsync(_apiTestContext.Uri, _apiTestContext.HttpContent).Result; break;
                case "PATCH": _apiTestContext.ResponseMessage = SendPatchRequestAsync(_apiTestContext.Uri, _apiTestContext.StringContent).Result; break;
                case "PUT": _apiTestContext.ResponseMessage = SendPutRequestAsync(_apiTestContext.Uri, _apiTestContext.StringContent).Result; break;
                case "DELETE": _apiTestContext.ResponseMessage = SendDeleteRequestAsync(_apiTestContext.Uri).Result; break;
                default: throw new ArgumentOutOfRangeException(_apiTestContext.HttpMethod.ToString(), _apiTestContext.HttpMethod.ToString(), null);
            }
        }

        [Then(@"the response should have the status (.*)")]
        public void ThenTheResponseShouldHaveStatus(HttpStatusCode statusCode)
        {
            _apiTestContext.ResponseMessage.StatusCode.Should().Be(statusCode);
            TestContext.WriteLine($"Status Code: {_apiTestContext.ResponseMessage.StatusCode}");
        }
    }
}
