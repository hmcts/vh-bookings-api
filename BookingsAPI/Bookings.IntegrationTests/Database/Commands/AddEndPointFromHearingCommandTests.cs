using System;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL;
using Bookings.DAL.Commands;
using Bookings.DAL.Exceptions;
using Bookings.DAL.Queries;
using Bookings.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.IntegrationTests.Database.Commands
{
    public class AddEndPointFromHearingCommandTests : DatabaseTestsBase
    {
        private AddEndPointFromHearingCommandHandler _commandHandler;
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;
        private Guid _newHearingId;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new AddEndPointFromHearingCommandHandler(context);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(context);
            _newHearingId = Guid.Empty;
        }

        [Test]
        public void Should_throw_exception_when_hearing_does_not_exist()
        {
            var hearingId = Guid.NewGuid();
            var endpoint = new Endpoint("displayName", "sip", "123");

            Assert.ThrowsAsync<HearingNotFoundException>(() => _commandHandler.Handle(
                new AddEndPointFromHearingCommand(hearingId, endpoint)));
        }

        [Test]
        public async Task Should_add_endpoint_to_hearing()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;

            var beforeCount = seededHearing.GetEndpoints().Count;

            var displayName = "newDisplayName";
            var sip = "newSIP";
            var pin = "9999";
            var newEndpoint = new Endpoint(displayName, sip, pin);

            await _commandHandler.Handle(new AddEndPointFromHearingCommand(_newHearingId, newEndpoint));

            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));

            var newEndPointList = returnedVideoHearing.GetEndpoints();
            var afterCount = newEndPointList.Count;

            afterCount.Should().BeGreaterThan(beforeCount);

            var newlyAddedEndPointInDb = newEndPointList.Single(ep => ep.DisplayName.Equals(displayName));
            newlyAddedEndPointInDb.DisplayName.Should().Be(displayName);
            newlyAddedEndPointInDb.Pin.Should().Be(pin);
            newlyAddedEndPointInDb.Sip.Should().Be(sip);
        }

        [TearDown]
        public new async Task TearDown()
        {
            if (_newHearingId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test hearing {_newHearingId}");
                await Hooks.RemoveVideoHearing(_newHearingId);
            }
        }
    }
}