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
        private GetHearingsByGroupIdQueryHandler _groupByHandler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetHearingsForNotificationsQueryHandler(context);
            _groupByHandler = new GetHearingsByGroupIdQueryHandler(context);
        }

        [Test]
        public async Task Shoud_return_hearings_between_48_to_72_hrs_for_notifications()
        {
            var hearing1 = await Hooks.SeedPastVideoHearing(DateTime.Today.AddDays(2), null, false, BookingStatus.Created, isMultiDayFirstHearing:true);
            var hearing2 = await Hooks.SeedPastVideoHearing(DateTime.Today.AddDays(2), null, false, BookingStatus.Created);
            var hearing3 = await Hooks.SeedPastVideoHearing(DateTime.Today.AddDays(3), null, false, BookingStatus.Created);

            var query = new GetHearingsForNotificationsQuery();

            var result = await _handler.Handle(query);

            var resulthearing1 = result.FirstOrDefault(x => x.Id == hearing1.Id);
            var resulthearing2 = result.FirstOrDefault(x => x.Id == hearing2.Id);
            var resulthearing3 = result.FirstOrDefault(x => x.Id == hearing3.Id);

            resulthearing1.Should().NotBeNull();
            resulthearing2.Should().NotBeNull();
            resulthearing3.Should().BeNull();

        }


        [Test]
        public async Task Should_not_return_multiday_hearings_between_48_to_72_hrs_for_notifications()
        {
            var hearing1 = await Hooks.SeedPastVideoHearing(DateTime.Today.AddDays(2), null, false, BookingStatus.Created);

            var groupId = hearing1.SourceId;

            var dates = new List<DateTime> { DateTime.Now.AddDays(2), DateTime.Now.AddDays(3) };
            await Hooks.CloneVideoHearing(hearing1.Id, dates);

            var groupQuery = new GetHearingsByGroupIdQuery(groupId.Value);
            var groupResults = await _groupByHandler.Handle(groupQuery);

            var query = new GetHearingsForNotificationsQuery();
            var result = await _handler.Handle(query);

            var returnedHearing1 = groupResults[0];
            var returnedHearing2 = groupResults[1];
            var returnedHearing3 = groupResults[2];

            var resulthearing1 = result.FirstOrDefault(x => x.Id == returnedHearing1.Id);
            var resulthearing2 = result.FirstOrDefault(x => x.Id == returnedHearing2.Id);
            var resulthearing3 = result.FirstOrDefault(x => x.Id == returnedHearing3.Id);

            resulthearing1.Should().NotBeNull();
            resulthearing2.Should().BeNull();
            resulthearing3.Should().BeNull();

        }
    }
}