using System;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL;
using Bookings.DAL.Queries;
using Bookings.Domain.Participants;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.IntegrationTests.Database.Queries
{
    public class GetHearingByIdQueryHandlerDatabaseTests : DatabaseTestsBase
    {
        private GetHearingByIdQueryHandler _handler;
        private Guid _newHearingId;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetHearingByIdQueryHandler(context);
        }
        
        [Test]
        public async Task should_get_hearing_details_by_id()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;
            var hearing = _handler.Handle(new GetHearingByIdQuery(_newHearingId));
            
            hearing.Should().NotBeNull();
            
            hearing.CaseType.Should().NotBeNull();
            hearing.HearingVenue.Should().NotBeNull();
            hearing.HearingType.Should().NotBeNull();
            
            var participants = hearing.GetParticipants();
            participants.Any().Should().BeTrue();
            participants.Single(x => x.GetType() == typeof(Individual)).Should().NotBeNull();
            participants.Single(x => x.GetType() == typeof(Representative)).Should().NotBeNull();

            var persons = hearing.GetPersons();
            persons.Count.Should().Be(participants.Count);
            persons[0].Title.Should().NotBeEmpty();
            var cases = hearing.GetCases();
            hearing.GetCases().Any(x => x.IsLeadCase).Should().BeTrue();
            cases.Count.Should().Be(2);
            cases[0].Name.Should().NotBeEmpty();
        }
        
        [TearDown]
        public async Task TearDown()
        {
            if (_newHearingId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test hearing {_newHearingId}");
                await Hooks.RemoveVideoHearing(_newHearingId);
            }
        }
    }
}