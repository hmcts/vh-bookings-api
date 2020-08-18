using Bookings.DAL;
using Bookings.DAL.Queries;
using Bookings.Domain.Enumerations;
using FluentAssertions;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace Bookings.IntegrationTests.Database.Queries
{
    public class GetHearingsByCaseNumberQueryHandlerDatabaseTests : DatabaseTestsBase
    {
        private GetHearingsByCaseNumberQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetHearingsByCaseNumberQueryHandler(context);
        }

        [Test]
        public async Task Should_get_hearing_details_by_case_number()
        {
            var seededHearing1 = await Hooks.SeedVideoHearing(false, BookingStatus.Created);

            TestContext.WriteLine($"New seeded video hearing id: { seededHearing1.Id }");

            var caseData = seededHearing1.HearingCases.FirstOrDefault();
            TestContext.WriteLine($"New seeded video caseNumer: { caseData.Case.Number }");

            var hearing = await _handler.Handle(new GetHearingsByCaseNumberQuery(caseData.Case.Number));

            hearing.Should().NotBeNull();
            hearing.Any().Should().BeTrue();

            hearing[0].ScheduledDateTime.Should().Be(seededHearing1.ScheduledDateTime);
            hearing[0].HearingRoomName.Should().Be(seededHearing1.HearingRoomName);
            hearing[0].HearingVenueName.Should().Be(seededHearing1.HearingVenueName);
        }
    }
}