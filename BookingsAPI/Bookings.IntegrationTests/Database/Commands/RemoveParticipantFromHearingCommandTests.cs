using System;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL;
using Bookings.DAL.Commands;
using Bookings.DAL.Exceptions;
using Bookings.DAL.Queries;
using Bookings.Domain.Participants;
using Bookings.Domain.RefData;
using Bookings.Domain.Validations;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace Bookings.IntegrationTests.Database.Commands
{
    public class RemoveParticipantFromHearingCommandTests : DatabaseTestsBase
    {
        private RemoveParticipantFromHearingCommandHandler _commandHandler;
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;
        private Guid _newHearingId;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new RemoveParticipantFromHearingCommandHandler(context);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(context);
            _newHearingId = Guid.Empty;
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

        [Test]
        public void Should_throw_exception_when_hearing_does_not_exist()
        {
            var hearingId = Guid.NewGuid();

            var newPerson = new PersonBuilder(true).Build();
            var participant = new Individual(newPerson, new HearingRole(1, "Dummy"), new CaseRole(1, "Dummy"));
            Assert.ThrowsAsync<HearingNotFoundException>(() => _commandHandler.Handle(
                new RemoveParticipantFromHearingCommand(hearingId, participant)));
        }

        [Test]
        public async Task Should_throw_exception_when_participant_does_not_exist()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;

            var newPerson = new PersonBuilder(true).Build();
            var participant = new Individual(newPerson, new HearingRole(1, "Dummy"), new CaseRole(1, "Dummy"));
            Assert.ThrowsAsync<DomainRuleException>(() => _commandHandler.Handle(
                new RemoveParticipantFromHearingCommand(seededHearing.Id, participant)));
        }

        [Test]
        public async Task Should_remove_participant_from_hearing()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;

            var beforeCount = seededHearing.GetParticipants().Count;

            var participant = seededHearing.GetParticipants().First();
            await _commandHandler.Handle(new RemoveParticipantFromHearingCommand(seededHearing.Id, participant));


            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            var afterCount = returnedVideoHearing.GetParticipants().Count;

            afterCount.Should().BeLessThan(beforeCount);
        }
    }
}