using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using Bookings.Api.Contract.Requests;
using Bookings.Common;
using Bookings.DAL;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;

namespace Bookings.IntegrationTests.Steps
{
    [Binding]
    public class CloneHearingSteps : BaseSteps
    {
        private CloneHearingRequest _cloneRequest;

        public CloneHearingSteps(Contexts.TestContext context) : base(context)
        {
        }

        [Given(@"I have a request to clone a non-existent hearing")]
        public void GivenIHaveARequestToCloneANonexistentHearing()
        {
            var hearingId = Guid.NewGuid();
            var dates = new List<DateTime> {DateTime.Today.AddDays(1)};
            SetupCloneRequest(hearingId, dates);
        }
        
        [Given(@"I have a request to clone a hearing with invalid dates")]
        public void GivenIHaveARequestToCloneAHearingWithInvalidDates()
        {
            var hearingId = Context.TestData.NewHearingId;
            var dates = new List<DateTime>
            {
                DateTime.Today.AddDays(-1),
                DateTime.Today.AddDays(5),
                DateTime.Today.AddDays(5)
            };

            SetupCloneRequest(hearingId, dates);
        }
        
        [Given(@"I have a request to clone a hearing with valid dates")]
        public void GivenIHaveARequestToCloneAHearingWithValidDates()
        {
            var hearingId = Context.TestData.NewHearingId;
            var originalDate = Context.TestData.SeededHearing.ScheduledDateTime;
            var date1 = originalDate.GetNextWorkingDay();
            var date2 = date1.GetNextWorkingDay();
            var date3 = date2.GetNextWorkingDay();
            var dates = new List<DateTime>
            {
                date1,
                date2,
                date3
            };

            SetupCloneRequest(hearingId, dates);
        }
        
        [Then(@"the database should have cloned hearings")]
        public async Task ThenTheDatabaseShouldHaveClonedHearings()
        {
            var hearingId = Context.TestData.NewHearingId;
            await using var db = new BookingsDbContext(Context.BookingsDbContextOptions);
            var hearingsFromDb =
                await db.VideoHearings.Include(x => x.HearingCases).ThenInclude(h => h.Case).AsNoTracking()
                    .Where(x => x.SourceId == hearingId)
                    .OrderBy(x => x.ScheduledDateTime).ToListAsync();
            hearingsFromDb.Count.Should().Be(_cloneRequest.Dates.Count + 1); // +1 to include the original hearing

            var totalDays = hearingsFromDb.Count;
            for (var i = 0; i < hearingsFromDb.Count-1; i++)
            {
                var hearingDay = i + 1;
                hearingsFromDb[i].GetCases().First().Name.Should().EndWith($"Day {hearingDay} of {totalDays}");
            }
        }

        private void SetupCloneRequest(Guid hearingId, List<DateTime> dates)
        {
            var request = new CloneHearingRequest {Dates = dates};
            _cloneRequest = request;
            var jsonBody = RequestHelper.Serialise(request);
            Context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            Context.Uri = ApiUriFactory.HearingsEndpoints.CloneHearing(hearingId);
            Context.HttpMethod = HttpMethod.Post;
        }
    }
}