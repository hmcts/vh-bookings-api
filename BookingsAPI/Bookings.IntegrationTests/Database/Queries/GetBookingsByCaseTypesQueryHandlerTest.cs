using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookings.Api.Contract.Responses;
using Bookings.API.Mappings;
using Bookings.API.Utilities;
using Bookings.DAL;
using Bookings.DAL.Queries;
using Bookings.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.IntegrationTests.Database.Queries
{
    public class GetBookingsByCaseTypesQueryHandlerTest : DatabaseTestsBase
    {
        private GetBookingsByCaseTypesQueryHandler _handler;
        private GetBookingsByCaseTypesQuery _query;
        private List<Guid> _ids;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetBookingsByCaseTypesQueryHandler(context);
            _query = new GetBookingsByCaseTypesQuery(new List<int>()) { Limit = 2};
            _ids = new List<Guid>();
        }

        [Test]
        public void should_get_CursorCreatedTime_as_a_number()
        {
            var query = new GetBookingsByCaseTypesQuery(new List<int>()) { Cursor = "604527484127", Limit = 2};
            query.CursorCreatedTime.Should().Be(604527484127);
        }

        [Test]
        public void should_get_CursorCreatedTime_as_zero_if_no_value()
        {
            _query.CursorCreatedTime.Should().Be(0);
            var query = new GetBookingsByCaseTypesQuery(new List<int>()) { Limit = 2, Cursor = "" };
            query.CursorCreatedTime.Should().Be(0);
        }

        [Test]
        public async Task should_get_booking_details_for_the_given_case_types()
        {
            var caseTypesIds = new List<int> { 1, 2 };

            _query = new GetBookingsByCaseTypesQuery(caseTypesIds) { Limit = 2 };

            _ids = new List<Guid>();
            long nextCursor = 0;
            for (int i = 0; i < 4; i++)
            {
                var seededHearing = await Hooks.SeedVideoHearing();
                _ids.Add(seededHearing.Id);

                TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            }

            var hearings = await _handler.Handle(_query);

            hearings.Should().NotBeNull();
            hearings.Count.Should().Be(2);

            var mapper = new VideoHearingsToBookingsResponseMapper();
            var response = new PaginationCursorBasedBuilder<BookingsResponse, VideoHearing>(mapper.MapHearingResponses)
               .WithSourceItems(hearings.AsQueryable())
               .Limit(_query.Limit)
               .CaseTypes(_query.CaseTypes)
               .Cursor(_query.Cursor)
               .ResourceUrl("hearings/types")
               .Build();

            response.Should().NotBeNull();
            response.Limit.Should().Be(2);
            response.Hearings.Count.Should().Be(1);
            response.Hearings[0].Hearings.Count.Should().Be(2);
            nextCursor = response.Hearings[0].Hearings[1].CreatedDate.Ticks;
            response.NextCursor.Should().Be(nextCursor.ToString());
        }

        [Test]
        public async Task should_get_booking_details_for_all_case_types()
        {
            _ids = new List<Guid>();
            long nextCursor = 0;
            for (int i = 0; i < 4; i++)
            {
                var seededHearing = await Hooks.SeedVideoHearing();
                _ids.Add(seededHearing.Id);

                TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            }

            var hearings = await _handler.Handle(_query);

            hearings.Should().NotBeNull();
            hearings.Count.Should().Be(2);

            var mapper = new VideoHearingsToBookingsResponseMapper();
            var response = new PaginationCursorBasedBuilder<BookingsResponse, VideoHearing>(mapper.MapHearingResponses)
               .WithSourceItems(hearings.AsQueryable())
               .Limit(_query.Limit)
               .CaseTypes(_query.CaseTypes)
               .Cursor(_query.Cursor)
               .ResourceUrl("hearings/types")
               .Build();

            response.Should().NotBeNull();
            response.Limit.Should().Be(2);
            response.Hearings.Count.Should().Be(1);
            response.Hearings[0].Hearings.Count.Should().Be(2);
            nextCursor = response.Hearings[0].Hearings[1].CreatedDate.Ticks;
            response.NextCursor.Should().Be(nextCursor.ToString());
        }

        [Test]
        public async Task should_get_booking_details_for_given_cursor()
        {
            long nextCursor = DateTime.Now.AddSeconds(-30).Ticks;
            _ids = new List<Guid>();
            for (int i = 0; i < 4; i++)
            {
                var seededHearing = await Hooks.SeedVideoHearing();
                _ids.Add(seededHearing.Id);
                TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            }

            _query.Cursor = nextCursor.ToString();
            var hearings = await _handler.Handle(_query);

            hearings.Should().NotBeNull();
            hearings.Count.Should().Be(2);

            var mapper = new VideoHearingsToBookingsResponseMapper();
            var response = new PaginationCursorBasedBuilder<BookingsResponse, VideoHearing>(mapper.MapHearingResponses)
               .WithSourceItems(hearings.AsQueryable())
               .Limit(_query.Limit)
               .CaseTypes(_query.CaseTypes)
               .Cursor(_query.Cursor)
               .ResourceUrl("hearings/types")
               .Build();

            response.Should().NotBeNull();
            response.Limit.Should().Be(2);
            response.Hearings.Count.Should().Be(1);
            response.Hearings[0].Hearings.Count.Should().Be(2);
            nextCursor = response.Hearings[0].Hearings[1].CreatedDate.Ticks;
            response.NextCursor.Should().Be(nextCursor.ToString());

        }

        [Test]
        public async Task should_returns_no_booking_details_for_given_cursor()
        {
            long nextCursor = DateTime.Now.AddHours(-1).Ticks;
            _ids = new List<Guid>();
            _query.Cursor = nextCursor.ToString();
            var hearings = await _handler.Handle(_query);

            hearings.Should().NotBeNull();
            hearings.Count.Should().Be(0);

            var mapper = new VideoHearingsToBookingsResponseMapper();
            var response = new PaginationCursorBasedBuilder<BookingsResponse, VideoHearing>(mapper.MapHearingResponses)
               .WithSourceItems(hearings.AsQueryable())
               .Limit(_query.Limit)
               .CaseTypes(_query.CaseTypes)
               .Cursor(_query.Cursor)
               .ResourceUrl("hearings/types")
               .Build();

            response.Should().NotBeNull();
            response.Limit.Should().Be(2);
            response.Hearings.Count.Should().Be(0);
            response.NextCursor.Should().Be("0");
        }

        [TearDown]
        public async Task TearDown()
        {
            if (_ids.Any())
            {
                foreach (var item in _ids)
                {
                    TestContext.WriteLine($"Removing test hearing {item}");
                    await Hooks.RemoveVideoHearing(item);
                }
            }
        }
    }
}
