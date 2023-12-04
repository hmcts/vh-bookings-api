using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Enumerations;

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
        [TestCase(BookingStatus.Created)]
        [TestCase(BookingStatus.Booked)]
        public async Task Shoud_return_hearings_between_48_to_72_hrs_for_notifications(BookingStatus bookingStatus)
        {
            var hearing1 = await Hooks.SeedPastVideoHearing(DateTime.Today.AddDays(2), null, BookingStatus.Created);
            var hearing2 = await Hooks.SeedPastVideoHearing(DateTime.Today.AddDays(2), null, BookingStatus.Created);
            var hearing3 = await Hooks.SeedPastVideoHearing(DateTime.Today.AddDays(3), null, BookingStatus.Created);

            var query = new GetHearingsForNotificationsQuery();
            var result = await _handler.Handle(query);

            var resulthearing1 = result.Find(x => x.Hearing.Id == hearing1.Id);
            var resulthearing2 = result.Find(x => x.Hearing.Id == hearing2.Id);
            var resulthearing3 = result.Find(x => x.Hearing.Id == hearing3.Id);

            resulthearing1.Should().NotBeNull();
            resulthearing2.Should().NotBeNull();
            resulthearing3.Should().BeNull();

        }
    }
}
