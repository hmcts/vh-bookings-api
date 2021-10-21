using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Queries
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
        public async Task Should_return_all_case_types_if_no_filter_is_given()
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
        public async Task Should_only_return_filtered_case_types()
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
        public void Should_throw_on_invalid_cursor()
        {
            Assert.ThrowsAsync<FormatException>(() =>
                _handler.Handle(new GetBookingsByCaseTypesQuery { Cursor = "invalid" }));
        }

        [Test]
        public async Task Should_limit_hearings_returned()
        {
            await Hooks.SeedVideoHearing();
            await Hooks.SeedVideoHearing();
            await Hooks.SeedVideoHearing();


            var query = new GetBookingsByCaseTypesQuery { Limit = 2 };
            var result = await _handler.Handle(query);

            result.Count.Should().Be(2);
        }

        [Test]
        public async Task Should_return_different_hearings_for_each_new_page()
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

        [Test]
        public async Task Should_return_hearings_on_or_after_the_from_date()
        {
            await Hooks.SeedVideoHearing(configureOptions => configureOptions.ScheduledDate = DateTime.UtcNow.AddDays(1));
            await Hooks.SeedVideoHearing(configureOptions => configureOptions.ScheduledDate = DateTime.UtcNow.AddDays(2));
            var fromDate = DateTime.UtcNow.Date.AddDays(3);
            var includedHearings = _context.VideoHearings
                .Include("Participants.Person")
                .Include("Participants.HearingRole.UserRole")
                .Include("Participants.CaseRole")
                .Include("HearingCases.Case")
                .Include(x => x.HearingType)
                .Include(x => x.CaseType)
                .Include(x => x.HearingVenue)
                .AsNoTracking().Where(x => x.ScheduledDateTime > fromDate);


            var query = new GetBookingsByCaseTypesQuery(new List<int>()) { Limit = 100, FromDate = fromDate };

            var hearings = await _handler.Handle(query);
            var hearingIds = hearings.Select(hearing => hearing.Id).ToList();

            hearingIds.Count.Should().Be(includedHearings.Count());
            foreach (var hearing in includedHearings) 
            {
                hearingIds.Should().Contain(hearing.Id);
            }
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
