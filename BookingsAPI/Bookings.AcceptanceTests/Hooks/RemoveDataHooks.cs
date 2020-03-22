using Bookings.AcceptanceTests.Contexts;
using TechTalk.SpecFlow;
using static Testing.Common.Builders.Api.ApiUriFactory.HearingsEndpoints;

namespace Bookings.AcceptanceTests.Hooks
{
    [Binding]
    public static class RemoveDataHooks
    {
        [AfterScenario(Order = (int)HooksSequence.RemoveDataHooks)]
        public static void RemoveData(TestContext context)
        {
            if (context.TestData.Hearing == null) return;
            var endpoint = RemoveHearing(context.TestData.Hearing.Id);
            context.Request = context.Delete(endpoint);
            context.Response = context.Client().Execute(context.Request);
        }
    }
}
