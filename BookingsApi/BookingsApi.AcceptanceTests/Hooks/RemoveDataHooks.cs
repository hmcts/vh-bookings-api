using System;
using System.Linq;
using System.Net.Http;
using AcceptanceTests.Common.Api.Helpers;
using BookingsApi.AcceptanceTests.Contexts;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using TechTalk.SpecFlow;
using static Testing.Common.Builders.Api.ApiUriFactory.HearingsEndpoints;

namespace BookingsApi.AcceptanceTests.Hooks
{
    [Binding]
    public static class RemoveDataHooks
    {
        private const int HearingsLimit = 10000;

        [AfterScenario(Order = (int)HooksSequence.RemoveDataHooks)]
        public static void RemoveData(TestContext context)
        {
            if (context.TestData.Hearing == null) return;
            DeleteTheHearing(context, context.TestData.Hearing.Id);

        }

        [AfterScenario(Order = (int)HooksSequence.RemoveAllDataHooks)]
        public static void RemoveAllData(TestContext context)
        {
            // If we have a hearing id, then we have created a hearing in this test run, so delete it
            if (context.TestData.Hearing != null)
            {
                DeleteTheHearing(context, context.TestData.Hearing.Id);
            }
            else
            {
                var request = new GetHearingRequest {Limit = HearingsLimit};

                var client = new TestHttpClient();

                var response = client.ExecuteAsync(
                        context, HearingTypesRelativePath,
                        request, HttpMethod.Get)
                    .Result;

                var hearings = RequestHelper.Deserialise<BookingsResponse>(response.Content.ReadAsStringAsync().Result);

                foreach (var hearing in hearings.Hearings.SelectMany(hearingsListResponse =>
                             hearingsListResponse.Hearings.Where(hearing =>
                                 hearing.HearingName.Contains(context.TestData.CaseName))))
                {
                    DeleteTheHearing(context, hearing.HearingId);
                }
            }
        }

        private static void DeleteTheHearing(TestContext context, Guid hearingId)
        {
            var endpoint = RemoveHearing(hearingId);
            context.Request = context.Delete(endpoint);
            context.Response = context.Client().Execute(context.Request);
            NUnit.Framework.TestContext.WriteLine($"Deleted hearing {hearingId}");
        }
    }
}
