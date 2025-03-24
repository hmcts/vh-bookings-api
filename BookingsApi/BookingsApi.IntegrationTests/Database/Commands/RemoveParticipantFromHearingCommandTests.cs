using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using BookingsApi.Domain.Validations;
using Microsoft.IdentityModel.Tokens;
using Testing.Common.Builders.Domain;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class RemoveParticipantFromHearingCommandTests : DatabaseTestsBase
    {
        private RemoveParticipantFromHearingCommandHandler _commandHandler;
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;
        private List<string> _personsToRemove;

        [SetUp]
        public void Setup()
        {
            _personsToRemove = new List<string>();
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new RemoveParticipantFromHearingCommandHandler(context);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(context);
        }

        [Test]
        public void Should_throw_exception_when_hearing_does_not_exist()
        {
            var hearingId = Guid.NewGuid();

            var newPerson = new PersonBuilder(true).Build();
            var participant = new Individual(Guid.NewGuid().ToString(), newPerson, new HearingRole(1, "Dummy"), "Dummy");
            Assert.ThrowsAsync<HearingNotFoundException>(() => _commandHandler.Handle(
                new RemoveParticipantFromHearingCommand(hearingId, participant)));
        }

        [Test]
        public async Task Should_throw_exception_when_participant_does_not_exist()
        {
            var seededHearing = await Hooks.SeedVideoHearingV2();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");

            var newPerson = new PersonBuilder(true).Build();
            var participant = new Individual(Guid.NewGuid().ToString(), newPerson, new HearingRole(1, "Dummy"), "Dummy");
            Assert.ThrowsAsync<DomainRuleException>(() => _commandHandler.Handle(
                new RemoveParticipantFromHearingCommand(seededHearing.Id, participant)));
        }

        [Test]
        public async Task Should_remove_participant_from_hearing()
        {
            var seededHearing = await Hooks.SeedVideoHearingV2();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");

            var beforeCount = seededHearing.GetParticipants().Count;
            var participant = seededHearing.GetParticipants()[0];
            _personsToRemove.Add(participant.Person.ContactEmail);
            await _commandHandler.Handle(new RemoveParticipantFromHearingCommand(seededHearing.Id, participant));

            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            var afterCount = returnedVideoHearing.GetParticipants().Count;

            afterCount.Should().BeLessThan(beforeCount);
        }

        [Test]
        public async Task Should_remove_participant_with_endpoint_from_hearing()
        {
            var seededHearing = await Hooks.SeedVideoHearingV2();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            var beforeCount = seededHearing.GetParticipants().Count;

            var endpoint = seededHearing.GetEndpoints().First(ep => !ep.ParticipantsLinked.IsNullOrEmpty());
            var linkedParticpant = endpoint.ParticipantsLinked[0];
            _personsToRemove.Add(linkedParticpant.Person.ContactEmail);
            await _commandHandler.Handle(
                new RemoveParticipantFromHearingCommand(seededHearing.Id, linkedParticpant));

            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            var afterCount = returnedVideoHearing.GetParticipants().Count;

            afterCount.Should().BeLessThan(beforeCount);
        }

        [Test]
        public async Task Should_Remove_ParticipantLink_When_Participant_Is_Removed()
        {
            var seededHearing = await Hooks.SeedVideoHearingV2(withLinkedParticipants: true);
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");

            var participantWithALink = seededHearing.Participants.First(x => x.LinkedParticipants.Any());
            _personsToRemove.Add(participantWithALink.Person.ContactEmail);
            await _commandHandler.Handle(
                new RemoveParticipantFromHearingCommand(seededHearing.Id, participantWithALink));
            var hearingWithNoLinks = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));

            hearingWithNoLinks.Participants.Where(x => x.LinkedParticipants.Any()).Should().BeNullOrEmpty();
        }
        
        [TearDown]
        public new async Task TearDown()
        {
            await base.TearDown();
            await Hooks.ClearUnattachedPersons(_personsToRemove);
        }
    }
}