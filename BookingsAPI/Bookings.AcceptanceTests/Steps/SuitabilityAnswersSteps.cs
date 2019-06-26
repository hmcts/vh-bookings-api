using Bookings.AcceptanceTests.Contexts;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;

namespace Bookings.AcceptanceTests.Steps
{
    [Binding]
    public sealed class SuitabilityAnswersSteps
    {
        private readonly TestContext _acTestContext;
        private readonly SuitabilityAnswerEndpoints _endpoints = new ApiUriFactory().SuitabilityAnswerEndpoints;

        public SuitabilityAnswersSteps(TestContext acTestContext)
        {
            _acTestContext = acTestContext;
        }

        [Given(@"I have a get suitability answers request")]
        public void GivenIHaveAGetSuitabilityAnswersRequest()
        {
            
        }
    }
}