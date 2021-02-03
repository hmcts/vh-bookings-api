using System;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using BookingsApi.Domain.Validations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class RemoveParticipantFromHearingCommandTests : DatabaseTestsBase
    {
        private RemoveParticipantFromHearingCommandHandler _commandHandler;
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new RemoveParticipantFromHearingCommandHandler(context);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(context);
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

            var beforeCount = seededHearing.GetParticipants().Count;

            var participant = seededHearing.GetParticipants().First();
            await _commandHandler.Handle(new RemoveParticipantFromHearingCommand(seededHearing.Id, participant));

            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            var afterCount = returnedVideoHearing.GetParticipants().Count;

            afterCount.Should().BeLessThan(beforeCount);
        }

        [Test]
        public async Task Should_remove_participant_with_endpoint_from_hearing()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            var beforeCount = seededHearing.GetParticipants().Count;

            var endpoint = seededHearing.GetEndpoints().First(ep => ep.DefenceAdvocate != null);
            await _commandHandler.Handle(
                new RemoveParticipantFromHearingCommand(seededHearing.Id, endpoint.DefenceAdvocate));

            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            var afterCount = returnedVideoHearing.GetParticipants().Count;

            afterCount.Should().BeLessThan(beforeCount);
        }

        [Test]
        public async Task Should_Remove_ParticipantLink_When_Participant_Is_Removed()
        {
            var seededHearing = await Hooks.SeedVideoHearing(null, false, BookingStatus.Booked, 0, false, true);
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");

            var participantWithALink = seededHearing.Participants.First(x => x.LinkedParticipants.Any());
            await _commandHandler.Handle(
                new RemoveParticipantFromHearingCommand(seededHearing.Id, participantWithALink));

            await using (var db = new BookingsDbContext(BookingsDbContextOptions))
            {
                var links = db.LinkedParticipant
                    .AsNoTracking()
                    .Include(x => x.Participant)
                    .Include(x => x.Linked)
                    .Where(x => x.ParticipantId == participantWithALink.Id)
                    .ToList();

                links.Should().BeNullOrEmpty();
            }
        }
    }
}