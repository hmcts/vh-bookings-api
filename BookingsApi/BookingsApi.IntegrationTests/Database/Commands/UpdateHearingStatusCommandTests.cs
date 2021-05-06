using System;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.Domain.Enumerations;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class UpdateHearingStatusCommandTests : DatabaseTestsBase
    {
        private UpdateHearingStatusCommandHandler _commandHandler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new UpdateHearingStatusCommandHandler(context);
        }

        [Test]
        public async Task should_be_able_to_update_status()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            var newBookingStatus = BookingStatus.Created;

            var command = new UpdateHearingStatusCommand(seededHearing.Id, newBookingStatus, "test", null);
            await _commandHandler.Handle(command);


            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var hearing = await db.VideoHearings.FindAsync(seededHearing.Id);
            hearing.Status.Should().Be(newBookingStatus);
        }

        [Test]
        public async Task should_be_able_to_transition_across_statuses()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            var expectedFinalStatus = BookingStatus.Created;

            await _commandHandler.Handle(new UpdateHearingStatusCommand(seededHearing.Id, BookingStatus.Created, "test",
                null));

            await _commandHandler.Handle(new UpdateHearingStatusCommand(seededHearing.Id, BookingStatus.Failed, "test",
                null));

            await _commandHandler.Handle(new UpdateHearingStatusCommand(seededHearing.Id, BookingStatus.Created, "test",
                null));


            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var hearing = await db.VideoHearings.FindAsync(seededHearing.Id);
            hearing.Status.Should().Be(expectedFinalStatus);
        }

        [Test]
        public void Should_throw_exception_when_hearing_does_not_exist()
        {
            Assert.ThrowsAsync<HearingNotFoundException>(() => _commandHandler.Handle(
                new UpdateHearingStatusCommand(Guid.NewGuid(), BookingStatus.Cancelled, "test", null)));
        }
    }
}