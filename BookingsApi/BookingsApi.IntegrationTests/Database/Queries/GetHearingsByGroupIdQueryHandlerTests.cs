using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetHearingsByGroupIdQueryHandlerTests : DatabaseTestsBase
    {
        private GetHearingsByGroupIdQueryHandler _handler;
        
        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetHearingsByGroupIdQueryHandler(context);
        }

        [Test]
        public async Task Should_return_hearings_for_single_day_by_groupId()
        {
            var hearing = await Hooks.SeedVideoHearing();
            var groupId = hearing.SourceId;

            var result = new List<VideoHearing>();
            if (groupId != null)
            {
                var query = new GetHearingsByGroupIdQuery(groupId.Value);
                result = await _handler.Handle(query);
            }

            var returnedHearing = result.First(x => x.Id == hearing.Id);
            
            returnedHearing.Should().NotBeNull();
            returnedHearing.Id.Should().Be(hearing.Id);
        }
        
        [Test]
        public async Task Should_return_hearings_for_multi_day_by_groupId()
        {
            var hearing1 = await Hooks.SeedVideoHearing();
            var groupId = hearing1.SourceId;

            var dates = new List<DateTime> {DateTime.Now.AddDays(2), DateTime.Now.AddDays(3)};
            await Hooks.CloneVideoHearing(hearing1.Id, dates);

            var result = new List<VideoHearing>();
            if (groupId != null)
            {
                var query = new GetHearingsByGroupIdQuery(groupId.Value);
                result = await _handler.Handle(query);
            }

            var returnedHearing1 = result[0];
            var returnedHearing2 = result[1];
            var returnedHearing3 = result[2];

            returnedHearing1.Should().NotBeNull();
            returnedHearing1.Id.Should().Be(hearing1.Id);
            returnedHearing1.SourceId.Should().Be(returnedHearing2.SourceId);
            returnedHearing2.SourceId.Should().Be(returnedHearing3.SourceId);
        }
    }
}