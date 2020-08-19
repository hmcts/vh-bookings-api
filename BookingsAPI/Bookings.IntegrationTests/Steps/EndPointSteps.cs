using Bookings.DAL;
using Bookings.Domain;
using Bookings.Domain.Enumerations;
using Bookings.IntegrationTests.Contexts;
using Bookings.IntegrationTests.Helper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using static Testing.Common.Builders.Api.ApiUriFactory.JVEndPointEndpoints;

namespace Bookings.IntegrationTests.Steps
{
    [Binding]
    public sealed class EndPointSteps : BaseSteps
    {
        private Guid _hearingId;
        private Guid _removedEndPointId;
        public EndPointSteps(Contexts.TestContext context) : base(context)
        {
            _hearingId = Guid.Empty;
            _removedEndPointId = Guid.Empty;
        }

        [Given(@"I have a hearing with endpoints")]
        public async Task GivenIHaveAHearingWithEndpoints()
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing(null, false, BookingStatus.Booked, 3);
            _hearingId = seededHearing.Id;
            Context.TestData.NewHearingId = seededHearing.Id;
        }

        [Given(@"I have a hearing without endpoints")]
        public async Task GivenIHaveAHearingWithoutEndpoints()
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            _hearingId = seededHearing.Id;
            Context.TestData.NewHearingId = seededHearing.Id;
        }

        [Given(@"I have remove non-existent endpoint from a hearing request")]
        public void GivenIHaveRemoveNonExistentEndpointFromAHearingRequest()
        {
            Context.HttpMethod = HttpMethod.Delete;
            Context.Uri = RemoveEndPointFromHearing(_hearingId, Guid.NewGuid());
        }

        [Given(@"I have remove endpoint from a hearing request")]
        public void GivenIHaveRemoveEndpointFromAHearingRequest()
        {
            var hearing = GetHearingFromDb();
            Context.HttpMethod = HttpMethod.Delete;
            Context.Uri = RemoveEndPointFromHearing(_hearingId, hearing.Endpoints.First().Id);
        }

        [Then(@"the endpoint should be removed")]
        public void ThenTheEndpointShouldBeAddedOrRemoved()
        {
            var hearingFromDb = GetHearingFromDb();
            hearingFromDb.GetEndpoints().Any(ep => ep.Id == _removedEndPointId).Should().BeFalse();
        }
        private Hearing GetHearingFromDb()
        {
            Hearing hearingFromDb;
            using (var db = new BookingsDbContext(Context.BookingsDbContextOptions))
            {
                hearingFromDb = db.VideoHearings
                    .Include(hearing => hearing.Endpoints)
                    .AsNoTracking()
                    .Single(x => x.Id == Context.TestData.NewHearingId);
            }
            return hearingFromDb;
        }
    }

}
