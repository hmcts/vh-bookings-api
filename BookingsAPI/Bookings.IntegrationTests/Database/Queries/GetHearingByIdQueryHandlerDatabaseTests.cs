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
            var hearing = await _handler.Handle(new GetHearingByIdQuery(seededHearing.Id));

            hearing.Should().NotBeNull();

            hearing.CaseType.Should().NotBeNull();
            hearing.HearingVenue.Should().NotBeNull();
            hearing.HearingType.Should().NotBeNull();

            var participants = hearing.GetParticipants();
            participants.Any().Should().BeTrue();
            participants.Any(x => x.GetType() == typeof(Individual)).Should().BeTrue();
            participants.Any(x => x.GetType() == typeof(Representative)).Should().BeTrue();
            participants.Any(x => x.GetType() == typeof(Judge)).Should().BeTrue();

            var persons = hearing.GetPersons();
            persons.Count.Should().Be(participants.Count);
            persons[0].Title.Should().NotBeEmpty();
            var cases = hearing.GetCases();
            hearing.GetCases().Any(x => x.IsLeadCase).Should().BeTrue();
            cases.Count.Should().Be(2);
            cases[0].Name.Should().NotBeEmpty();
            hearing.HearingRoomName.Should().NotBeEmpty();
            hearing.OtherInformation.Should().NotBeEmpty();
            hearing.CreatedBy.Should().NotBeNullOrEmpty();
        }
    }
}