using System;
using System.Linq;
using AcceptanceTests.Common.Api.Helpers;
using Bookings.AcceptanceTests.Contexts;
using Bookings.Api.Contract.Responses;
using TechTalk.SpecFlow;
using static Testing.Common.Builders.Api.ApiUriFactory.HearingsEndpoints;

namespace Bookings.AcceptanceTests.Hooks
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
            context.Request = context.Get(GetHearingsByAnyCaseType(HearingsLimit));
            context.Response = context.Client().Execute(context.Request);
            var hearings = RequestHelper.DeserialiseSnakeCaseJsonToResponse<BookingsResponse>(context.Response.Content);
            var hearingIds = hearings.Hearings.SelectMany(hearingsListResponse =>
                hearingsListResponse.Hearings.Where(hearing =>
                    hearing.HearingName.Contains(context.TestData.CaseName)))
                .Select(x=> x.GroupId).Distinct().ToList();
            foreach (var id in hearingIds.Where(id => id.HasValue))
            {
                DeleteTheHearing(context, id.Value);
            }
        }

        private static void DeleteTheHearing(TestContext context, Guid hearingId)
        {
            var endpoint = RemoveHearing(hearingId);
            context.Request = context.Delete(endpoint);
            context.Response = context.Client().Execute(context.Request);
        }
    }
}
