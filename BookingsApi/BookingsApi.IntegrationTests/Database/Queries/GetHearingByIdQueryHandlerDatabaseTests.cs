using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Participants;

namespace BookingsApi.IntegrationTests.Database.Queries
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
        public async Task Should_get_hearing_details_by_id()
        {
            var seededHearing = await Hooks.SeedVideoHearingV2();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            var hearing = await _handler.Handle(new GetHearingByIdQuery(seededHearing.Id));

            hearing.Should().NotBeNull();

            hearing.CaseType.Should().NotBeNull();
            hearing.HearingVenue.Should().NotBeNull();

            var participants = hearing.GetParticipants();
            participants.Any().Should().BeTrue();
            var individuals = participants.Where(x => x.GetType() == typeof(Individual))
                .ToList();
            individuals.Should().NotBeNullOrEmpty();
            
            var representatives = participants.Where(x => x.GetType() == typeof(Representative));
            representatives.Should().NotBeNullOrEmpty();

            hearing.GetJudge().Should().NotBeNull();
            
            var persons = hearing.GetPersons();
            persons.Count.Should().Be(participants.Count);
            persons[0].Title.Should().NotBeEmpty();
            var cases = hearing.GetCases();
            hearing.GetCases().Any(x => x.IsLeadCase).Should().BeTrue();
            cases.Count.Should().Be(1);
            cases[0].Name.Should().NotBeEmpty();
            hearing.HearingRoomName.Should().NotBeEmpty();
            hearing.OtherInformation.Should().NotBeEmpty();
            hearing.CreatedBy.Should().NotBeNullOrEmpty();
            hearing.Endpoints.Should().NotBeNullOrEmpty();
        }
    }
}