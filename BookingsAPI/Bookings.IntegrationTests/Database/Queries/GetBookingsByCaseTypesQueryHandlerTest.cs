using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private BookingsDbContext _context;

        private const string FinancialRemedy = "Financial Remedy";

        [SetUp]
        public void Setup()
        {
            _context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetBookingsByCaseTypesQueryHandler(_context);
        }

        [Test]
        public async Task should_return_all_case_types_if_no_filter_is_given()
        {
            var firstHearing = (await Hooks.SeedVideoHearing()).Id;
            var financialRemedyHearing = (await Hooks.SeedVideoHearing(opts => opts.CaseTypeName = FinancialRemedy)).Id;

            // we have to (potentially) look through all the existing hearings to find these
            var query = new GetBookingsByCaseTypesQuery { Limit = _context.VideoHearings.Count() };
            var result = await _handler.Handle(query);

            var hearingIds = result.Select(hearing => hearing.Id).ToList();
            hearingIds.Should().Contain(firstHearing);
            hearingIds.Should().Contain(financialRemedyHearing);

            var hearingTypes = result.Select(hearing => hearing.CaseType.Name).Distinct().ToList();
            hearingTypes.Count.Should().BeGreaterThan(1);
        }

        [Test]
        public async Task should_only_return_filtered_case_types()
        {
            await Hooks.SeedVideoHearing();
            var financialRemedyHearing = await Hooks.SeedVideoHearing(opt => opt.CaseTypeName = FinancialRemedy);

            var query = new GetBookingsByCaseTypesQuery(new List<int> { financialRemedyHearing.CaseTypeId });
            var result = await _handler.Handle(query);

            var hearingIds = result.Select(hearing => hearing.Id).ToList();
            hearingIds.Should().Contain(financialRemedyHearing.Id);

            var hearingTypes = result.Select(hearing => hearing.CaseType.Name).Distinct().ToList();
            hearingTypes.Should().Equal(FinancialRemedy);
        }

        [Test]
        public void should_throw_on_invalid_cursor()
        {
            Assert.ThrowsAsync<FormatException>(() =>
                _handler.Handle(new GetBookingsByCaseTypesQuery { Cursor = "invalid" }));
        }

        [Test]
        public async Task should_limit_hearings_returned()
        {
            await Hooks.SeedVideoHearing();
            await Hooks.SeedVideoHearing();
            await Hooks.SeedVideoHearing();


            var query = new GetBookingsByCaseTypesQuery { Limit = 2 };
            var result = await _handler.Handle(query);

            result.Count.Should().Be(2);
        }

        [Test]
        public async Task should_get_all_hearings_for_today_returned()
        {
            await Hooks.SeedVideoHearingWithStartDateToday(opt => opt.CaseTypeName = FinancialRemedy);
            var caseTypes = new List<int> { 2 };
            var query = new GetBookingsByCaseTypesQuery(caseTypes) { Limit = 3 };
            var result = await _handler.Handle(query);
            var expectDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            result.Count.Should().BeGreaterThan(0);
            foreach (var item in result)
            {
                item.ScheduledDateTime.Ticks.Should().BeGreaterThan(expectDay.Ticks);
            }
        }

        [Test]
        public async Task should_return_different_hearings_for_each_new_page()
        {
            // When generating the identifiers they may end up being in order accidentally, therefor,
            // seed hearings until they end up in invalid order
            var createdHearings = new List<Guid>();
            while (IdsAreInOrder(createdHearings) || createdHearings.Count < 3)
            {
                createdHearings.Add((await Hooks.SeedVideoHearing()).Id);
            }

            // And paging through the results
            string cursor = null;
            var allHearings = new List<VideoHearing>();
            while (true)
            {
                var result = await _handler.Handle(new GetBookingsByCaseTypesQuery { Limit = 1, Cursor = cursor });
                allHearings.AddRange(result);
                if (result.NextCursor == null) break;
                cursor = result.NextCursor;
            }

            // They should all have different id's
            var ids = allHearings.Select(x => x.Id);
            ids.Distinct().Count().Should().Be(_context.VideoHearings.Count());
        }

        private static bool IdsAreInOrder(List<Guid> ids)
        {
            for (var i = 0; i < ids.Count - 1; ++i)
            {
                if (string.Compare(ids[i].ToString(), ids[i + 1].ToString(), StringComparison.Ordinal) < 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
