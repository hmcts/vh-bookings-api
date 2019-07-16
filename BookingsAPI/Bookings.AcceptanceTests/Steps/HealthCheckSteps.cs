using Bookings.AcceptanceTests.Contexts;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;

namespace Bookings.AcceptanceTests.Steps
{
    [Binding]
    public sealed class HealthCheckSteps
    {
        private readonly TestContext _context;
        private readonly HealthCheckEndpoints _endpoints = new ApiUriFactory().HealthCheckEndpoints;

        public HealthCheckSteps(TestContext context)
        {
            _context = context;
        }

        [Given(@"I have a get health request")]
        public void GivenIHaveAGetHealthRequest()
        {
            _context.Request = _context.Get(_endpoints.HealthCheck);
        }
    }
}
