using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Queries;

namespace Bookings.IntegrationTests.Database.Commands
{
    public class AnonymiseHearingsCommandTests : DatabaseTestsBase
    {
        private AnonymiseHearingsCommandHandler _commandHandler;
        private Guid _newHearingId;
        private Guid _newHearingId2;
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new AnonymiseHearingsCommandHandler(context);
            _newHearingId = Guid.Empty;
            _newHearingId2 = Guid.Empty;
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(context);
        }

        [Test]
        public async Task Should_anonymise_hearings_and_participant_data_for_hearings_older_than_3_months()
        {
            var seededHearing = await Hooks.SeedPastHearings(DateTime.Today.AddMonths(-3));
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;

            var command = new AnonymiseHearingsCommand();
            await _commandHandler.Handle(command);
            command.RecordsUpdated.Should().Be(6);
            command.UpdatedDate.Date.Should().Be(DateTime.UtcNow.Date);
            var returnedVideoHearing = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            returnedVideoHearing.Should().NotBeNull();

            var hearingData = returnedVideoHearing.HearingCases[0];
            hearingData.Case.Name.Should().NotBe(seededHearing.HearingCases[0].Case.Name);

            foreach (var participant in seededHearing.Participants)
            {
                var updatedParticipant = returnedVideoHearing.Participants.FirstOrDefault(p => p.Id == participant.Id);
                updatedParticipant.DisplayName.Should().NotBe(participant.DisplayName);
            }
            foreach (var person in seededHearing.GetPersons())
            {
                var updatedPerson = returnedVideoHearing.GetPersons().FirstOrDefault(p => p.Id == person.Id);
                updatedPerson.FirstName.Should().Be(person.FirstName);
                updatedPerson.LastName.Should().Be(person.LastName);
                updatedPerson.MiddleNames.Should().Be(person.MiddleNames);
                updatedPerson.Username.Should().Be(person.Username);
                updatedPerson.ContactEmail.Should().Be(person.ContactEmail);
                updatedPerson.TelephoneNumber.Should().Be(person.TelephoneNumber);
            }
        }

        [Test]
        public async Task Should_anonymise_hearings_and_participant_data_for_hearings_within_the_last_2_months()
        {
            var seededHearing = await Hooks.SeedPastHearings(DateTime.Today.AddMonths(-2));
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;

            var command = new AnonymiseHearingsCommand();
            await _commandHandler.Handle(command);
            command.RecordsUpdated.Should().Be(-1);
            command.UpdatedDate.Date.Should().Be(DateTime.UtcNow.Date);

            var returnedVideoHearing = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            returnedVideoHearing.Should().NotBeNull();

            var hearingData = returnedVideoHearing.HearingCases[0];
            hearingData.Case.Name.Should().Be(seededHearing.HearingCases[0].Case.Name);

            foreach (var participant in seededHearing.Participants)
            {
                var updatedParticipant = returnedVideoHearing.Participants.FirstOrDefault(p => p.Id == participant.Id);
                updatedParticipant.DisplayName.Should().Be(participant.DisplayName);
            }
            foreach (var person in seededHearing.GetPersons())
            {
                var updatedPerson = returnedVideoHearing.GetPersons().FirstOrDefault(p => p.Id == person.Id);
                updatedPerson.FirstName.Should().Be(person.FirstName);
                updatedPerson.LastName.Should().Be(person.LastName);
                updatedPerson.MiddleNames.Should().Be(person.MiddleNames);
                updatedPerson.Username.Should().Be(person.Username);
                updatedPerson.ContactEmail.Should().Be(person.ContactEmail);
                updatedPerson.TelephoneNumber.Should().Be(person.TelephoneNumber);
            }
        }

        [Test]
        public async Task Should_not_anonymise_hearings_and_participant_data_that_have_been_anonymised()
        {
            var seededHearing = await Hooks.SeedPastHearings(DateTime.Today.AddMonths(-3));
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;

            var command = new AnonymiseHearingsCommand();
            await _commandHandler.Handle(command);
            command.RecordsUpdated.Should().Be(6);
            command.UpdatedDate.Date.Should().Be(DateTime.UtcNow.Date);
            var returnedVideoHearing = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            returnedVideoHearing.Should().NotBeNull();

            var hearingData = returnedVideoHearing.HearingCases[0];
            hearingData.Case.Name.Should().NotBe(seededHearing.HearingCases[0].Case.Name);

            foreach (var participant in seededHearing.Participants)
            {
                var updatedParticipant = returnedVideoHearing.Participants.FirstOrDefault(p => p.Id == participant.Id);
                updatedParticipant.DisplayName.Should().NotBe(participant.DisplayName);
            }
            foreach (var person in seededHearing.GetPersons())
            {
                var updatedPerson = returnedVideoHearing.GetPersons().FirstOrDefault(p => p.Id == person.Id);
                updatedPerson.FirstName.Should().Be(person.FirstName);
                updatedPerson.LastName.Should().Be(person.LastName);
                updatedPerson.MiddleNames.Should().Be(person.MiddleNames);
                updatedPerson.Username.Should().Be(person.Username);
                updatedPerson.ContactEmail.Should().Be(person.ContactEmail);
                updatedPerson.TelephoneNumber.Should().Be(person.TelephoneNumber);
            }

            command = new AnonymiseHearingsCommand();
            await _commandHandler.Handle(command);
            command.RecordsUpdated.Should().Be(-1);

            var notAnonymisedVideoHearing = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            notAnonymisedVideoHearing.Should().NotBeNull();

            var notAnonymisedHearingData = notAnonymisedVideoHearing.HearingCases[0];
            notAnonymisedHearingData.Case.Name.Should().Be(hearingData.Case.Name);
        }
    }
}
