using System;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Bookings.IntegrationTests.Hooks
{
    public static class RemoveDataHooks
    {
        [AfterScenario(Order = (int)HooksSequence.RemoveData)]
        public static async Task RemoveData(Contexts.TestContext context)
        {
            if (context.TestData.NewHearingId != Guid.Empty)
            {
                NUnit.Framework.TestContext.WriteLine($"Removing test hearing {context.TestData.NewHearingId}");
                await context.TestDataManager.RemoveVideoHearing(context.TestData.NewHearingId);
            }

            if (context.TestData.OldHearingId != Guid.Empty)
            {
                NUnit.Framework.TestContext.WriteLine($"Removing test hearing {context.TestData.OldHearingId}");
                await context.TestDataManager.RemoveVideoHearing(context.TestData.OldHearingId);
            }

            if (context.TestData.RemovedPersons != null && context.TestData.RemovedPersons.Any())
            {
                await context.TestDataManager.ClearUnattachedPersons(context.TestData.RemovedPersons);
            }
        }

        [AfterScenario(Order = (int)HooksSequence.RemoveServer)]
        public static void RemoveServer(Contexts.TestContext context)
        {
            context.Server.Dispose();
        }
    }
}
