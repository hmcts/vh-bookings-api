using System;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL;
using Bookings.DAL.Queries;
using Bookings.Domain;
using Bookings.Domain.Enumerations;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace Bookings.IntegrationTests.Database.Queries
{
    public class GetHearingByUsernameQueryHandlerTests : DatabaseTestsBase
    {
        private GetHearingForUsernameQueryHandler _handler;
        private Guid _newHearingId;
        private Guid _secondHearingId;
        private Guid _thirdHearingId;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetHearingForUsernameQueryHandler(context);
            _newHearingId = Guid.Empty;
            _secondHearingId = Guid.Empty;
            _thirdHearingId = Guid.Empty;
        }
        
        [Test]
        public async Task should_return_empty_list()
        {
            var username = "nobody@test.com";
            var query = new GetHearingsForUsernameQuery(username);
            var hearings = await _handler.Handle(query);

            hearings.Should().BeEmpty();
        }
        
        [Test]
        public async Task should_hearing_for_user()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            var seededHearing2 = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing2.Id}");
            _newHearingId = seededHearing.Id;
            _secondHearingId = seededHearing2.Id;
            var username = seededHearing.GetParticipants().First().Person.Username;
            var query = new GetHearingsForUsernameQuery(username);
            var hearings = await _handler.Handle(query);
            
            hearings.Should().NotBeEmpty();
            hearings.Count.Should().Be(1);
        }

        [Test]
        public async Task should_return_non_closed_hearings()
        {
            var username = await SeedOpenPausedAndClosedHearing();
            var query = new GetHearingsForUsernameQuery(username);
            var hearings = await _handler.Handle(query);
            
            hearings.Should().NotBeEmpty();
            hearings.Count.Should().Be(2);
        }

        private async Task<string> SeedOpenPausedAndClosedHearing()
        {
            var venues = new RefDataBuilder().HearingVenues;
            var caseTypeName = "Civil Money Claims";
            var caseType = Hooks.GetCaseTypeFromDb(caseTypeName);
            var hearingTypeName = "Application to Set Judgment Aside";
            var hearingType = caseType.HearingTypes.First(x => x.Name == hearingTypeName);
            var scheduledDate = DateTime.Today.AddHours(10).AddMinutes(30);
            var duration = 45;
            
            var person1 = new PersonBuilder(true).Build();
            var claimantCaseRole = caseType.CaseRoles.First(x => x.Name == "Claimant");
            var claimantLipHearingRole = claimantCaseRole.HearingRoles.First(x => x.Name == "Claimant LIP");
            
            var openHearing = new VideoHearing(caseType, hearingType, scheduledDate, duration, venues.First())
            {
                CreatedBy = "test@integration.com"
            };
            openHearing.AddIndividual(person1, claimantLipHearingRole, claimantCaseRole,
                $"{person1.FirstName} {person1.LastName}");
            
            var pausedHearing = new VideoHearing(caseType, hearingType, scheduledDate, duration, venues.First())
            {
                CreatedBy = "test@integration.com"
            };
            pausedHearing.AddIndividual(person1, claimantLipHearingRole, claimantCaseRole,
                $"{person1.FirstName} {person1.LastName}");
            pausedHearing.UpdateStatus(HearingStatus.Paused);
            
            var closedHearing = new VideoHearing(caseType, hearingType, scheduledDate, duration, venues.First())
            {
                CreatedBy = "test@integration.com"
            };
            closedHearing.AddIndividual(person1, claimantLipHearingRole, claimantCaseRole,
                $"{person1.FirstName} {person1.LastName}");
            closedHearing.UpdateStatus(HearingStatus.Closed);
            using (var db = new BookingsDbContext(BookingsDbContextOptions))
            {
                await db.VideoHearings.AddRangeAsync(new[] {openHearing, pausedHearing, closedHearing});
                await db.SaveChangesAsync();
            }

            return person1.Username;
        }
        
        [TearDown]
        public async Task TearDown()
        {
            if (_newHearingId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test hearing {_newHearingId}");
                await Hooks.RemoveVideoHearing(_newHearingId);
            }
            
            if (_secondHearingId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test hearing {_secondHearingId}");
                await Hooks.RemoveVideoHearing(_secondHearingId);
            }
            
            if (_thirdHearingId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test hearing {_thirdHearingId}");
                await Hooks.RemoveVideoHearing(_thirdHearingId);
            }
        }
    }
}