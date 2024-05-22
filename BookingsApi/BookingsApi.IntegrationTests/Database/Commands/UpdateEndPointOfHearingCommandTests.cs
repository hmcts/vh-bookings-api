using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Enumerations;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class UpdateEndPointOfHearingCommandTests : DatabaseTestsBase
    {
        private UpdateEndPointOfHearingCommandHandler _commandHandler;
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new UpdateEndPointOfHearingCommandHandler(context);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(context);
        }

        [Test]
        public void Should_throw_exception_when_hearing_does_not_exist()
        {
            var hearingId = Guid.NewGuid();

            Assert.ThrowsAsync<HearingNotFoundException>(() => _commandHandler.Handle(
                new UpdateEndPointOfHearingCommand(hearingId, Guid.NewGuid(), "DP", null)));
        }

        [Test]
        public async Task Should_throw_exception_when_endpoint_does_not_exist()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");

            Assert.ThrowsAsync<EndPointNotFoundException>(() => _commandHandler.Handle(
                new UpdateEndPointOfHearingCommand(seededHearing.Id, Guid.NewGuid(), "DP", null)));
        }

        [Test]
        public async Task Should_throw_exception_when_display_name_is_null()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");

            Assert.ThrowsAsync<ArgumentNullException>(() => _commandHandler.Handle(
                new UpdateEndPointOfHearingCommand(seededHearing.Id, seededHearing.GetEndpoints()[0].Id, string.Empty, null)));
        }

        [Test]
        public async Task Should_update_endpoint_display_name()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");

            var endpoint = seededHearing.GetEndpoints()[0];
            var updatedDisplayName = "updatedDisplayName";
            await _commandHandler.Handle(new UpdateEndPointOfHearingCommand(seededHearing.Id, endpoint.Id, updatedDisplayName, null));


            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            var updatedEndPoint = returnedVideoHearing.GetEndpoints().First(ep => ep.Id == endpoint.Id);
            updatedEndPoint.DisplayName.Should().Be(updatedDisplayName);
        }
        
        [Test]
        public async Task Should_update_endpoint_with_defence_advocate()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");

            var endpoint = seededHearing.GetEndpoints().First(ep => !ep.EndpointParticipants.Any());
            var dA = seededHearing.Participants.First(x => x.HearingRole.UserRole.IsRepresentative);
            var updatedDisplayName = "updatedDisplayName";
            await _commandHandler.Handle(new UpdateEndPointOfHearingCommand(seededHearing.Id, endpoint.Id, updatedDisplayName, (dA, LinkedParticipantType.DefenceAdvocate)));
            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            var updatedEndPoint = returnedVideoHearing.GetEndpoints().First(ep => ep.Id == endpoint.Id);
            updatedEndPoint.DisplayName.Should().Be(updatedDisplayName);
            updatedEndPoint.GetDefenceAdvocate().Should().NotBeNull();
            updatedEndPoint.GetDefenceAdvocate().Id.Should().Be(dA.Id);
            endpoint.CreatedDate.Should().Be(updatedEndPoint.CreatedDate.Value);
            updatedEndPoint.UpdatedDate.Should().BeAfter(updatedEndPoint.CreatedDate.Value);
        }

        [Test]
        public async Task Should_update_endpoint_with_defence_advocate_as_null_for_none()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");

            var endpoint = seededHearing.GetEndpoints()[0];
            var dA = seededHearing.Participants.First(x => x.HearingRole.UserRole.IsRepresentative);
            var updatedDisplayName = "updatedDisplayName";
            await _commandHandler.Handle(new UpdateEndPointOfHearingCommand(seededHearing.Id, endpoint.Id, updatedDisplayName, (dA, LinkedParticipantType.DefenceAdvocate)));

            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            var updatedEndPoint = returnedVideoHearing.GetEndpoints().First(ep => ep.Id == endpoint.Id);
            updatedEndPoint.DisplayName.Should().Be(updatedDisplayName);
            updatedEndPoint.GetDefenceAdvocate().Should().NotBeNull();
            updatedEndPoint.GetDefenceAdvocate().Id.Should().Be(dA.Id);

            await _commandHandler.Handle(new UpdateEndPointOfHearingCommand(seededHearing.Id, endpoint.Id, updatedDisplayName, null));
            returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            updatedEndPoint = returnedVideoHearing.GetEndpoints().First(ep => ep.Id == endpoint.Id);
            updatedEndPoint.DisplayName.Should().Be(updatedDisplayName);
            updatedEndPoint.GetDefenceAdvocate().Should().BeNull();
        }
    }
}
