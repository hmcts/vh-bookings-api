using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Participants;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetHearingShellByIdQueryHandlerDatabaseTests : DatabaseTestsBase
    {
        private GetHearingShellByIdQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetHearingShellByIdQueryHandler(context);
        }

        [Test]
        public async Task Should_get_hearing_shell_by_id()
        {
            var seededHearing = await Hooks.SeedVideoHearing(status: Domain.Enumerations.BookingStatus.Created, isMultiDayFirstHearing: false);
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            var hearing = await _handler.Handle(new GetHearingShellByIdQuery(seededHearing.Id));

            hearing.Should().NotBeNull();
            hearing.OtherInformation.Should().NotBeEmpty();
            hearing.CreatedBy.Should().NotBeNullOrEmpty();
            hearing.Status.Should().Be(Domain.Enumerations.BookingStatus.Created);
        }
    }
}