using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Common.Services;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries.V1;
using BookingsApi.Domain;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetBookingsByCaseTypesQueryHandlerTest : DatabaseTestsBase
    {
        private GetBookingsByCaseTypesQueryHandler _handler;
        private BookingsDbContext _context;
        private Mock<IFeatureToggles> FeatureTogglesMock;

        private const string FinancialRemedy = "Financial Remedy";

        [SetUp]
        public void Setup()
        {
            FeatureTogglesMock = new Mock<IFeatureToggles>();

            FeatureTogglesMock.Setup(r => r.AdminSearchToggle()).Returns(false);

            _context = new BookingsDbContext(BookingsDbContextOptions);

            _handler = new GetBookingsByCaseTypesQueryHandler(_context, FeatureTogglesMock.Object);
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

        [Test(Description = "With AdminSearchToggle On")]
        public async Task Should_return_video_hearings_filtered_by_case_number()
        {
            await Hooks.SeedVideoHearing();

            FeatureTogglesMock.Setup(r => r.AdminSearchToggle()).Returns(true);

            var videoHearing = await Hooks.SeedVideoHearing(opt => opt.CaseTypeName = FinancialRemedy);

            var query = new GetBookingsByCaseTypesQuery(new List<int> { videoHearing.CaseTypeId }) 
            { 
                CaseNumber = Hooks.CaseNumber 
            };

            var result = await _handler.Handle(query);

            AssertHearingsAreFilteredByCaseNumber(result, Hooks.CaseNumber);
        }

        [Test(Description = "With AdminSearchToggle On")]
        public async Task Should_return_video_hearings_filtered_by_lastName()
        {
            var participantLastName = "Automation";

            await Hooks.SeedVideoHearing();

            FeatureTogglesMock.Setup(r => r.AdminSearchToggle()).Returns(true);

            var query = new GetBookingsByCaseTypesQuery
            {
                LastName = participantLastName
            };

            var result = await _handler.Handle(query);

            AssertHearingsAreFilteredByLastName(result, participantLastName);
        }
        [Test(Description = "With AdminSearchToggle On")]
        public async Task Should_return_video_hearings_with_no_judge()
        {
            FeatureTogglesMock.Setup(r => r.AdminSearchToggle()).Returns(true);

            var bookingsWithNoJugde = await Hooks.SeedVideoHearingWithNoJudge();

            var bookingsWithJudge = await Hooks.SeedVideoHearing(opt => opt.CaseTypeName = FinancialRemedy);

            var query = new GetBookingsByCaseTypesQuery(new List<int> { bookingsWithJudge.CaseTypeId, bookingsWithNoJugde.CaseTypeId })
            {
                NoJudge = true
            };

            var hearings = await _handler.Handle(query);

            AssertHearingsContainsNoJdge(hearings);
        }

        [Test(Description = "With AdminSearchToggle On")]
        public async Task Should_return_video_hearings_filtered_by_venue_ids()
        {
            FeatureTogglesMock.Setup(r => r.AdminSearchToggle()).Returns(true);

            var venues = await _context.Venues.AsNoTracking().ToListAsync();

            var venue1 = venues[0];
            var venue2 = venues[1];
            var venue3 = venues[2];
  
            await Hooks.SeedVideoHearing(opt => opt.HearingVenue = venue1);
            await Hooks.SeedVideoHearing(opt => opt.HearingVenue = venue2);
            await Hooks.SeedVideoHearing(opt => opt.HearingVenue = venue3);

            var venueIdsToFilterOn = new List<int> { venue1.Id, venue3.Id };
            
            var query = new GetBookingsByCaseTypesQuery
            {
                VenueIds = venueIdsToFilterOn
            };

            var result = await _handler.Handle(query);

            AssertHearingsAreFilteredByVenueIds(result, venueIdsToFilterOn);
        }

        [Test(Description = "With AdminSearchToggle On")]
        public async Task Should_return_video_hearings_filtered_by_multiple_criteria()
        {
            FeatureTogglesMock.Setup(r => r.AdminSearchToggle()).Returns(true);
            var venues = await _context.Venues.AsNoTracking().ToListAsync();
        
            var venue1 = venues[0];
            var venue2 = venues[1];
            var venue3 = venues[2];
        
            await Hooks.SeedVideoHearing(opt => opt.HearingVenue = venue1);
            await Hooks.SeedVideoHearing(opt => opt.HearingVenue = venue2);
            await Hooks.SeedVideoHearing(opt =>
            {
                opt.CaseTypeName = FinancialRemedy;
                opt.HearingVenue = venue3;
            });
            
            var venueIdsToFilterOn = new List<int> { venue1.Id, venue3.Id };
            
            var query = new GetBookingsByCaseTypesQuery
            {
                CaseNumber = Hooks.CaseNumber,
                VenueIds = venueIdsToFilterOn
            };
            
            var result = await _handler.Handle(query);

            AssertHearingsAreFilteredByCaseNumber(result, Hooks.CaseNumber);
            AssertHearingsAreFilteredByVenueIds(result, venueIdsToFilterOn);
        }
        
        [Test(Description = "With AdminSearchToggle On")]
        public async Task Should_return_video_hearings_filtered_by_user_id()
        {
            FeatureTogglesMock.Setup(r => r.AdminSearchToggle()).Returns(true);


            var users = new RefDataBuilder().Users;

            var user1 = users[0];
            var user2 = users[1];

            var usersIdsToFilterOn = new List<Guid> { user1.Id, user2.Id };
            
            var query = new GetBookingsByCaseTypesQuery
            {
                SelectedUsers = usersIdsToFilterOn
            };

            var result = await _handler.Handle(query);

            AssertHearingsAreFilteredByUsersIds(result, usersIdsToFilterOn);
        }
        
        [Test(Description = "With AdminSearchToggle On")]
        public async Task Should_return_video_hearings_filtered_by_not_allocated_empty()
        {
            FeatureTogglesMock.Setup(r => r.AdminSearchToggle()).Returns(true);
            await Hooks.SeedJusticeUserList("team.lead@hearings.reform.hmcts.net", "firstName", "secondname", true);
            
            
            var query = new GetBookingsByCaseTypesQuery
            {
                NoAllocated = true
            };

            var result = await _handler.Handle(query);

            AssertHearingsAreFilteredByNoAllocatedEmpty(result);
        }
        
        [Test(Description = "With AdminSearchToggle On")]
        public async Task Should_return_video_hearings_filtered_by_not_allocated_not_empty()
        {
            FeatureTogglesMock.Setup(r => r.AdminSearchToggle()).Returns(true);
            await Hooks.SeedVideoHearing();
            await Hooks.SeedVideoHearing();
            await Hooks.SeedVideoHearing();
            await Hooks.SeedAllocatedJusticeUser("team.lead@hearings.reform.hmcts.net", "firstName", 
                "secondname");
            
            
            var query = new GetBookingsByCaseTypesQuery
            {
                NoAllocated = true
            };

            var result = await _handler.Handle(query);

            AssertHearingsAreFilteredByNoAllocatedNotEmpty(result);
        }

        private static void AssertHearingsAreFilteredByCaseNumber(IEnumerable<VideoHearing> hearings, string caseNumber)
        {
            var containsHearingsFilteredByCaseNumber = hearings
                .SelectMany(r => r.HearingCases)
                .All(r => r.Case.Number == caseNumber);
            
            containsHearingsFilteredByCaseNumber.Should().BeTrue();
        }

        private static void AssertHearingsContainsNoJdge(IEnumerable<VideoHearing> hearings)
        {
            var containsHearingsFilteredWithNoJudge = hearings
                .SelectMany(r => r.Participants)
                .Distinct()
                .All(r => !r.Discriminator.Equals("judge", StringComparison.InvariantCultureIgnoreCase));

            containsHearingsFilteredWithNoJudge.Should().BeTrue();

            hearings.Count().Should().Be(1);
        }

        private static void AssertHearingsAreFilteredByVenueIds(IEnumerable<VideoHearing> hearings, List<int> venueIds)
        {
            var containsHearingsFilteredByVenues = hearings
                .Select(r => r.HearingVenue)
                .Distinct()
                .All(r => venueIds.Contains(r.Id));
            
            containsHearingsFilteredByVenues.Should().BeTrue();
        }
        
        private static void AssertHearingsAreFilteredByUsersIds(IEnumerable<VideoHearing> hearings, List<Guid> userIds)
        {
            var containsHearingsFilteredByUsers = hearings
                .Select(r => r.AllocatedTo)
                .Distinct()
                .All(r => userIds.Contains(r.Id));
            
            containsHearingsFilteredByUsers.Should().BeTrue();
        }
        
        private static void AssertHearingsAreFilteredByNoAllocatedEmpty(IEnumerable<VideoHearing> hearings)
        {
            var containsHearingsFilteredByUsers = hearings
                .Where(r => r.Allocations.Count <= 0);
            
            containsHearingsFilteredByUsers.Should().BeEmpty();
        }
        
        private static void AssertHearingsAreFilteredByNoAllocatedNotEmpty(IEnumerable<VideoHearing> hearings)
        {
            var containsHearingsFilteredByUsers = hearings
                .Where(r => r.Allocations.Count <= 0);
            
            containsHearingsFilteredByUsers.Should().NotBeEmpty();
        }

        private static void AssertHearingsAreFilteredByLastName(IEnumerable<VideoHearing> hearings, string participantLastName)
        {
            var containsHearingsFilteredByCaseNumber = hearings
            .SelectMany(r => r.Participants)
            .All(r => r.Person.LastName.Contains(participantLastName));

            containsHearingsFilteredByCaseNumber.Should().BeTrue();
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
            var startDate = DateTime.UtcNow.Date.AddDays(3);
            var includedHearings = _context.VideoHearings
                .Include("Participants.Person")
                .Include("Participants.HearingRole.UserRole")
                .Include("Participants.CaseRole")
                .Include("HearingCases.Case")
                .Include(x => x.HearingType)
                .Include(x => x.CaseType)
                .Include(x => x.HearingVenue)
                .AsNoTracking().Where(x => x.ScheduledDateTime > startDate);


            var query = new GetBookingsByCaseTypesQuery(new List<int>()) { Limit = 100, StartDate = startDate };

            var hearings = await _handler.Handle(query);
            var hearingIds = hearings.Select(hearing => hearing.Id).ToList();

            hearingIds.Count.Should().Be(includedHearings.Count());
            foreach (var hearing in includedHearings)
            {
                hearingIds.Should().Contain(hearing.Id);
            }
        }

        [Test]
        public async Task Should_return_hearings_on_or_after_the_from_date_and_before_to_date()
        {
            FeatureTogglesMock.Setup(r => r.AdminSearchToggle()).Returns(true);

            await Hooks.SeedVideoHearing(configureOptions => configureOptions.ScheduledDate = DateTime.UtcNow.AddDays(1));
            await Hooks.SeedVideoHearing(configureOptions => configureOptions.ScheduledDate = DateTime.UtcNow.AddDays(3));
            await Hooks.SeedVideoHearing(configureOptions => configureOptions.ScheduledDate = DateTime.UtcNow.AddDays(4));
            await Hooks.SeedVideoHearing(configureOptions => configureOptions.ScheduledDate = DateTime.UtcNow.AddDays(5));

            var startDate = DateTime.UtcNow.Date.AddDays(2);
            var endDate = DateTime.UtcNow.Date.AddDays(3);

            var includedHearings = _context.VideoHearings
                .Include("Participants.Person")
                .Include("Participants.HearingRole.UserRole")
                .Include("Participants.CaseRole")
                .Include("HearingCases.Case")
                .Include(x => x.HearingType)
                .Include(x => x.CaseType)
                .Include(x => x.HearingVenue)
                .AsNoTracking().Where(x => x.ScheduledDateTime > startDate && x.ScheduledDateTime <= endDate);

            var query = new GetBookingsByCaseTypesQuery(new List<int>()) { Limit = 100, StartDate = startDate, EndDate = endDate };

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
