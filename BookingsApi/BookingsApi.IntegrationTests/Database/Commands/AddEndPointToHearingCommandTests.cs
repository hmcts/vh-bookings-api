using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Dtos;
using BookingsApi.Domain.Enumerations;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class AddEndPointToHearingCommandTests : DatabaseTestsBase
    {
        private AddEndPointToHearingCommandHandler _commandHandler;
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;
        private Guid _newHearingId;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new AddEndPointToHearingCommandHandler(context);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(context);
            _newHearingId = Guid.Empty;
        }

        [Test]
        public void Should_throw_exception_when_hearing_does_not_exist()
        {
            var hearingId = Guid.NewGuid();
            var newEndpoint = new EndpointDto
            {
                DisplayName = "displayName",
                Sip = "sip",
                Pin = "pin"
            };
            Assert.ThrowsAsync<HearingNotFoundException>(() => _commandHandler.Handle(
                new AddEndPointToHearingCommand(hearingId, newEndpoint)));
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
            var newEndpoint = new EndpointDto
            {
                DisplayName = displayName,
                Sip = sip,
                Pin = pin
            };

            await _commandHandler.Handle(new AddEndPointToHearingCommand(_newHearingId, newEndpoint));

            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));

            var newEndPointList = returnedVideoHearing.GetEndpoints();
            var afterCount = newEndPointList.Count;

            afterCount.Should().BeGreaterThan(beforeCount);

            var newlyAddedEndPointInDb = newEndPointList.Single(ep => ep.DisplayName.Equals(displayName));
            newlyAddedEndPointInDb.DisplayName.Should().Be(displayName);
            newlyAddedEndPointInDb.Pin.Should().Be(pin);
            newlyAddedEndPointInDb.Sip.Should().Be(sip);
            newlyAddedEndPointInDb.CreatedDate.Should().Be(newlyAddedEndPointInDb.UpdatedDate.Value);
        }

        [Test]
        public async Task Should_add_endpoint_with_defence_advocate_to_hearing()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;

            var beforeCount = seededHearing.GetEndpoints().Count;

            var dA = seededHearing.Participants.First(x => x.HearingRole.UserRole.IsRepresentative);
            var displayName = "newDisplayName";
            var sip = "newSIP";
            var pin = "9999";
            var newEndpoint = new EndpointDto
            {
                DisplayName = displayName,
                Sip = sip,
                Pin = pin,
                EndpointParticipants = new List<EndpointParticipantDto>()
                {
                    new () { ContactEmail = dA.Person.ContactEmail, Type = LinkedParticipantType.DefenceAdvocate }
                } 
            };

            await _commandHandler.Handle(new AddEndPointToHearingCommand(_newHearingId, newEndpoint));

            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));

            var newEndPointList = returnedVideoHearing.GetEndpoints();
            var afterCount = newEndPointList.Count;

            afterCount.Should().BeGreaterThan(beforeCount);

            var newlyAddedEndPointInDb = newEndPointList.Single(ep => ep.DisplayName.Equals(displayName));
            newlyAddedEndPointInDb.DisplayName.Should().Be(displayName);
            newlyAddedEndPointInDb.Pin.Should().Be(pin);
            newlyAddedEndPointInDb.Sip.Should().Be(sip);
            newlyAddedEndPointInDb.GetDefenceAdvocate().Should().NotBeNull();
            newlyAddedEndPointInDb.GetDefenceAdvocate().Id.Should().Be(dA.Id);
        }
    }
}