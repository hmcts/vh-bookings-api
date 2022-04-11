using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using BookingsApi.IntegrationTests.Contexts;
using Newtonsoft.Json;
using TechTalk.SpecFlow;
using static Testing.Common.Builders.Api.ApiUriFactory.HearingsEndpoints;

namespace BookingsApi.IntegrationTests.Hooks
{
    [Binding]
    public static class RemoveDataHooks
    {
        private const int HearingsLimit = 10000;

        [AfterScenario(Order = (int)HooksSequence.RemoveData)]
        public static async Task RemoveData(TestContext context)
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

            await context.TestDataManager.ClearJudiciaryPersonsAsync();
        }

        [AfterScenario(Order = (int)HooksSequence.RemoveData)]
        public static async Task RemoveAllHearingsWithCaseName(TestContext context)
        {
            var request = new GetHearingRequest { Limit = HearingsLimit };
            
            using var client = context.Server.CreateClient();

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {context.BearerToken}");

            context.Uri = client.BaseAddress + HearingTypesRelativePath;
            context.HttpMethod = HttpMethod.Get;
            context.HttpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(
                new HttpRequestMessage { 
                    Content = context.HttpContent, 
                    Method = context.HttpMethod, 
                    RequestUri = new Uri(context.Uri) });

            var hearings = RequestHelper.Deserialise<BookingsResponse>(await response.Content.ReadAsStringAsync());
            foreach (var hearing in hearings.Hearings.SelectMany(
                hearingsListResponse => hearingsListResponse.Hearings.Where(hearing => hearing.HearingName.Contains(context.TestData.CaseName))))
            {
                await context.TestDataManager.RemoveVideoHearing(hearing.HearingId);
            }
        }

        [AfterScenario(Order = (int)HooksSequence.RemoveServer)]
        public static void RemoveServer(Contexts.TestContext context)
        {
            context.Server.Dispose();
        }
    }
}
