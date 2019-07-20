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
            Context.ResponseMessage = new HttpResponseMessage();
            switch (Context.HttpMethod.Method)
            {
                case "GET": Context.ResponseMessage = await SendGetRequestAsync(Context); break;
                case "POST": Context.ResponseMessage = await SendPostRequestAsync(Context); break;
                case "PATCH": Context.ResponseMessage = await SendPatchRequestAsync(Context); break;
                case "PUT": Context.ResponseMessage = await SendPutRequestAsync(Context); break;
                case "DELETE": Context.ResponseMessage = await SendDeleteRequestAsync(Context); break;
                default: throw new ArgumentOutOfRangeException(Context.HttpMethod.ToString(), Context.HttpMethod.ToString(), null);
            }
        }

        [Then(@"the response should have the status (.*) and success status (.*)")]
        public void ThenTheResponseShouldHaveStatus(HttpStatusCode statusCode, bool isSuccess)
        {
            Context.ResponseMessage.StatusCode.Should().Be(statusCode);
            Context.ResponseMessage.IsSuccessStatusCode.Should().Be(isSuccess);
            NUnit.Framework.TestContext.WriteLine($"Status Code: {Context.ResponseMessage.StatusCode}");
        }

        [Then(@"the response message should read '(.*)'")]
        [Then(@"the error response message should contain '(.*)'")]
        [Then(@"the error response message should also contain '(.*)'")]
        public void ThenTheResponseShouldContain(string errorMessage)
        {
            Context.ResponseMessage.Content.ReadAsStringAsync().Result.Should().Contain(errorMessage);
        }
    }
}
