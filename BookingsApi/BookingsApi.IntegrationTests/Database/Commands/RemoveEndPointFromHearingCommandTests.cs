using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class RemoveEndPointFromHearingCommandTests : DatabaseTestsBase
    {
        private RemoveEndPointFromHearingCommandHandler _commandHandler;
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new RemoveEndPointFromHearingCommandHandler(context);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(context);
        }

        [Test]
        public void Should_throw_exception_when_hearing_does_not_exist()
        {
            var hearingId = Guid.NewGuid();

            Assert.ThrowsAsync<HearingNotFoundException>(() => _commandHandler.Handle(
                new RemoveEndPointFromHearingCommand(hearingId, Guid.NewGuid())));
        }

        [Test]
        public async Task Should_throw_exception_when_endpoint_does_not_exist()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");

            Assert.ThrowsAsync<EndPointNotFoundException>(() => _commandHandler.Handle(
                new RemoveEndPointFromHearingCommand(seededHearing.Id, Guid.NewGuid())));
        }

        [Test]
        public async Task Should_remove_endpoint_from_hearing()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");

            var beforeCount = seededHearing.GetEndpoints().Count;

            var endpoint = seededHearing.GetEndpoints()[0];
            await _commandHandler.Handle(new RemoveEndPointFromHearingCommand(seededHearing.Id, endpoint.Id));


            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            var afterCount = returnedVideoHearing.GetEndpoints().Count;

            afterCount.Should().BeLessThan(beforeCount);
        }

        [Test]
        public async Task Should_remove_endpoint_with_defence_advocate_from_hearing()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");

            var beforeCount = seededHearing.GetEndpoints().Count;

            var endpoint = seededHearing.GetEndpoints().FirstOrDefault(ep => ep.GetDefenceAdvocate() != null);
            await _commandHandler.Handle(new RemoveEndPointFromHearingCommand(seededHearing.Id, endpoint.Id));

            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            var afterCount = returnedVideoHearing.GetEndpoints().Count;

            afterCount.Should().BeLessThan(beforeCount);
        }

    }
}