using System;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL;
using Bookings.DAL.Commands;
using Bookings.DAL.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.IntegrationTests.Database.Commands
{
    public class UpdateHearingCommandDatabaseTests : DatabaseTestsBase
    {
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;
        private GetHearingVenuesQueryHandler _getHearingVenuesQueryHandler;
        private UpdateHearingCommandHandler _commandHandler;
        private Guid _newHearingId;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(context);
            _getHearingVenuesQueryHandler = new GetHearingVenuesQueryHandler(context);
            _commandHandler = new UpdateHearingCommandHandler(context);
        }
        
        [Test]
        public async Task should_be_able_to_update_video_hearing()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;

            var newVenue = _getHearingVenuesQueryHandler.Handle(new GetHearingVenuesQuery()).Last();
            var newDuration = seededHearing.ScheduledDuration + 10;
            var newDateTime = seededHearing.ScheduledDateTime.AddDays(1);

            await _commandHandler.Handle(new UpdateHearingCommand(_newHearingId, newDateTime, newDuration, newVenue));
            
            var returnedVideoHearing = _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));

            returnedVideoHearing.HearingVenue.Name.Should().Be(newVenue.Name);
            returnedVideoHearing.ScheduledDuration.Should().Be(newDuration);
            returnedVideoHearing.ScheduledDateTime.Should().Be(newDateTime);
        }
    }
}