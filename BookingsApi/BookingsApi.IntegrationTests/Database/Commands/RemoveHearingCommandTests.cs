using System;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands.V1;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries.V1;
using BookingsApi.Domain.Enumerations;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class RemoveHearingCommandTests : DatabaseTestsBase
    {
        private RemoveHearingCommandHandler _commandHandler;
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;
        private Guid _newHearingId;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new RemoveHearingCommandHandler(context);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(context);
            _newHearingId = Guid.Empty;
        }

        [Test]
        public void Should_throw_exception_when_hearing_does_not_exist()
        {
            var hearingId = Guid.NewGuid();
            Assert.ThrowsAsync<HearingNotFoundException>(() => _commandHandler.Handle(
                new RemoveHearingCommand(hearingId)));
        }

        [Test]
        public async Task Should_remove_hearing()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;

            await _commandHandler.Handle(new RemoveHearingCommand(seededHearing.Id));
                
            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            returnedVideoHearing.Should().BeNull();

            _newHearingId = Guid.Empty;
        }
        
        [Test]
        public async Task Should_remove_hearing_containing_interpreter()
        {
            var seededHearing = await Hooks.SeedVideoHearing(null, false,BookingStatus.Booked,0,false,true);
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;

            await _commandHandler.Handle(new RemoveHearingCommand(seededHearing.Id));
                
            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            returnedVideoHearing.Should().BeNull();

            _newHearingId = Guid.Empty;
        }
    }
}