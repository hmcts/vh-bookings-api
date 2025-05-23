﻿using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Services;

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
            _commandHandler = new AddEndPointToHearingCommandHandler(context, new HearingService(context));
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(context);
            _newHearingId = Guid.Empty;
        }

        [Test]
        public void Should_throw_exception_when_hearing_does_not_exist()
        {
            var hearingId = Guid.NewGuid();
            var newEndpoint = new NewEndpoint
            {
                DisplayName = "displayName",
                Sip = "sip",
                Pin = "pin",
                LinkedParticipantEmails = null
            };
            Assert.ThrowsAsync<HearingNotFoundException>(() => _commandHandler.Handle(
                new AddEndPointToHearingCommand(hearingId, newEndpoint)));
        }

        [Test]
        public async Task Should_add_endpoint_to_hearing()
        {
            var seededHearing = await Hooks.SeedVideoHearingV2();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;

            var beforeCount = seededHearing.GetEndpoints().Count;

            var displayName = "newDisplayName";
            var sip = "newSIP";
            var pin = "9999";
            var newEndpoint = new NewEndpoint
            {
                DisplayName = displayName,
                Sip = sip,
                Pin = pin,
                LinkedParticipantEmails = null
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
        }

        [Test]
        public async Task Should_add_endpoint_with_defence_advocate_to_hearing()
        {
            var seededHearing = await Hooks.SeedVideoHearingV2();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;

            var beforeCount = seededHearing.GetEndpoints().Count;

            var dA = seededHearing.Participants.First(x => x.HearingRole.UserRole.IsRepresentative);
            var displayName = "newDisplayName";
            var sip = "newSIP";
            var pin = "9999";
            var newEndpoint = new NewEndpoint
            {
                DisplayName = displayName,
                Sip = sip,
                Pin = pin,
                LinkedParticipantEmails = [dA.Person.ContactEmail]
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
            newlyAddedEndPointInDb.ParticipantsLinked.Should().NotBeEmpty();
            newlyAddedEndPointInDb.ParticipantsLinked.Should().Contain(e => e.Id == dA.Id);
        }
    }
}