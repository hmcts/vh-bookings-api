using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetHearingsForNotificationsQueryHandlerTests : DatabaseTestsBase
    {
        private GetHearingsForNotificationsQueryHandler _handler;
        
        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetHearingsForNotificationsQueryHandler(context);
        }
        
        [Test]
        public async Task Shoud_return_hearings_between_48_to_72_hrs_for_notifications()
        {
            var hearing1 = await Hooks.SeedPastVideoHearing(DateTime.Today.AddDays(1), null, false, BookingStatus.Created);
            var hearing2 = await Hooks.SeedPastVideoHearing(DateTime.Today.AddDays(2), null, false, BookingStatus.Created);
            var hearing3 = await Hooks.SeedPastVideoHearing(DateTime.Today.AddDays(3), null, false, BookingStatus.Created);

            var query = new GetHearingsForNotificationsQuery();

            var result = await _handler.Handle(query);

            var returnedHearing1 = result[0];
            var returnedHearing2 = result[1];

            result.Count.Should().Be(2);

            returnedHearing1.Id.Should().Be(hearing1.Id);
            returnedHearing2.Id.Should().Be(hearing2.Id);

            result.Should().NotContain(hearing3);
        }

        /*
        [Test]
        public async Task Should_return_hearings_for_nof()
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
        } */
    }
}