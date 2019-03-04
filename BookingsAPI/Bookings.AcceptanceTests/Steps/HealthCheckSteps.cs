using Bookings.AcceptanceTests.Contexts;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;

namespace Bookings.AcceptanceTests.Steps
{
    [Binding]
    public sealed class HealthCheckSteps : StepsBase
    {
        private readonly AcTestContext _acTestContext;
        private readonly HealthCheckEndpoints _endpoints = new ApiUriFactory().HealthCheckEndpoints;

        public HealthCheckSteps(AcTestContext acTestContext)
        {
            _acTestContext = acTestContext;
        }

        [Given(@"I have a get health request")]
        public void GivenIHaveAGetHealthRequest()
        {
            _acTestContext.Request = _acTestContext.Get(_endpoints.HealthCheck);
        }
    }
}
