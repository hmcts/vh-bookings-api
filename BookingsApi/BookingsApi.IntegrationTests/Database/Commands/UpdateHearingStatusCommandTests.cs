using System;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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

        [Test]
        public async Task Should_deallocate_when_status_is_changed_to_cancelled()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            var justiceUser = await Hooks.SeedJusticeUser("cso@email.com", "Cso", "Test");
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            db.Allocations.Add(new Allocation
            {
                HearingId = seededHearing.Id,
                JusticeUserId = justiceUser.Id
            });
            await db.SaveChangesAsync();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            var newBookingStatus = BookingStatus.Cancelled;
            var hearing = await GetHearing(seededHearing.Id, db);
            hearing.Should().NotBeNull();
            hearing.AllocatedTo.Should().NotBeNull();
            hearing.AllocatedTo.Id.Should().Be(justiceUser.Id);
            
            var command = new UpdateHearingStatusCommand(seededHearing.Id, newBookingStatus, "test", "Withdrawn");
            await _commandHandler.Handle(command);

            await using var db2 = new BookingsDbContext(BookingsDbContextOptions);
            hearing = await GetHearing(seededHearing.Id, db2);
            hearing.Should().NotBeNull();
            hearing.Status.Should().Be(newBookingStatus);
            hearing.AllocatedTo.Should().BeNull();
        }

        private async Task<VideoHearing> GetHearing(Guid hearingId, BookingsDbContext context)
        {
            var hearing = await context.VideoHearings
                .Include(x => x.Allocations).ThenInclude(x => x.JusticeUser)
                .SingleOrDefaultAsync(x => x.Id == hearingId);

            return hearing;
        }
    }
}