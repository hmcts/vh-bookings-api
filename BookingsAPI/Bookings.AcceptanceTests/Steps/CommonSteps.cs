using System.Net;
using Bookings.AcceptanceTests.Contexts;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace Bookings.AcceptanceTests.Steps
{
    [Binding]
    public sealed class CommonSteps : StepsBase
    {
        private readonly ScenarioContext context;
        private readonly AcTestContext _acTestContext;

        public CommonSteps(ScenarioContext injectedContext, AcTestContext acTestContext)
        {
            context = injectedContext;
            _acTestContext = acTestContext;
        }

        [When(@"I send the request to the endpoint")]
        public void WhenISendTheRequestToTheEndpoint()
        {
            _acTestContext.Response = _acTestContext.Client().Execute(_acTestContext.Request);
        }

        [Then(@"the response should have the status (.*) and success status (.*)")]
        public void ThenTheResponseShouldHaveTheStatusAndSuccessStatus(HttpStatusCode httpStatusCode, bool isSuccess)
        {
            _acTestContext.Response.StatusCode.Should().Be(httpStatusCode);
            _acTestContext.Response.IsSuccessful.Should().Be(isSuccess);
        }
    }
}
