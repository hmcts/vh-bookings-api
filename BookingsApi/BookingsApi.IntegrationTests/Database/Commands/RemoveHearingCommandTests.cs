using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Enumerations;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class RemoveHearingCommandTests : DatabaseTestsBase
    {
        private RemoveHearingCommandHandler _commandHandler;
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new RemoveHearingCommandHandler(context);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(context);
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

            await _commandHandler.Handle(new RemoveHearingCommand(seededHearing.Id));
                
            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            returnedVideoHearing.Should().BeNull();
        }
        
        [Test]
        public async Task Should_remove_hearing_containing_interpreter()
        {
            var seededHearing = await Hooks.SeedVideoHearing(withLinkedParticipants: true);
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");

            await _commandHandler.Handle(new RemoveHearingCommand(seededHearing.Id));
                
            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            returnedVideoHearing.Should().BeNull();
        }
    }
}