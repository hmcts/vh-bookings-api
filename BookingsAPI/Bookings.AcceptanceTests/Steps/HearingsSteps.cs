using NUnit.Framework;
using TechTalk.SpecFlow;
using Testing.Common.Configuration;

namespace Bookings.AcceptanceTests.Steps
{
    [Binding]
    public sealed class HearingsSteps
    {
        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

        private readonly ScenarioContext context;

        public HearingsSteps(ScenarioContext injectedContext)
        {
            context = injectedContext;
        }

        [Given("I have entered (.*) into the calculator")]
        public void GivenIHaveEnteredSomethingIntoTheCalculator(int number)
        {
            //TODO: implement arrange (precondition) logic
            // For storing and retrieving scenario-specific data see https://go.specflow.org/doc-sharingdata 
            // To use the multiline text or the table argument of the scenario,
            // additional string/Table parameters can be defined on the step definition
            // method. 

        }

        [When("I press add")]
        public void WhenIPressAdd()
        {
            //TODO: implement act (action) logic

        }

        [Then("the result should be (.*) on the screen")]
        public void ThenTheResultShouldBe(int result)
        {
            //TODO: implement assert (verification) logic

        }
    }
}
