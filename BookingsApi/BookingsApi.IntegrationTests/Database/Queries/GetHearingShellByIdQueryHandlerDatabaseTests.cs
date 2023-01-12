using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Participants;
using FluentAssertions;
using NUnit.Framework;

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
            var seededHearing = await Hooks.SeedVideoHearing(false, Domain.Enumerations.BookingStatus.Created, false);
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            var hearing = await _handler.Handle(new GetHearingShellByIdQuery(seededHearing.Id));

            hearing.Should().NotBeNull();
            hearing.OtherInformation.Should().NotBeEmpty();
            hearing.CreatedBy.Should().NotBeNullOrEmpty();
            hearing.Status.Should().Be(Domain.Enumerations.BookingStatus.Created);
        }
    }
}