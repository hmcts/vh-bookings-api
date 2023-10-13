using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Services;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class UpdateParticipantCommandDatabaseTests : DatabaseTestsBase
    {
        private UpdateParticipantCommandHandler _commandHandler;
        private Guid _newHearingId;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            var hearingService = new HearingService(context);
            _commandHandler = new UpdateParticipantCommandHandler(context, hearingService);
            _newHearingId = Guid.Empty;
        }
        
        [Test]
        public async Task Should_be_able_to_update_individual_participant()
        {
            var editPrefix = " _Edit";
            var seededHearing = await Hooks.SeedVideoHearing();
            var beforeUpdatedDate = seededHearing.UpdatedDate;
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;
            var individualParticipant = seededHearing.GetParticipants().First(x=>x.HearingRole.UserRole.Name.Equals("Individual"));

            var title = individualParticipant.Person.Title + editPrefix;
            var displayName = individualParticipant.DisplayName + editPrefix;
            var telephoneNumber = "11112222333";
            var organisationName = "Organisation" + editPrefix;

            var updateParticipantCommand = new UpdateParticipantCommand(_newHearingId, individualParticipant.Id, title, displayName, telephoneNumber, organisationName, null, null);
            await _commandHandler.Handle(updateParticipantCommand);

            var updatedIndividual = (Individual)updateParticipantCommand.UpdatedParticipant;

            updatedIndividual.Should().NotBeNull();
            updatedIndividual.UpdatedDate.Should().BeAfter(beforeUpdatedDate);
            updatedIndividual.Person.Title.Should().Be(title);
            updatedIndividual.DisplayName.Should().Be(displayName);
            updatedIndividual.Person.TelephoneNumber.Should().Be(telephoneNumber);
            updatedIndividual.UpdatedDate.Should().BeAfter(updatedIndividual.CreatedDate);
        }

        [Test]
        public async Task Should_be_able_to_update_representative_participant()
        {
            var editPrefix = " _Edit";
            var seededHearing = await Hooks.SeedVideoHearing();
            var beforeUpdatedDate = seededHearing.UpdatedDate;
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;
            var representativeParticipant = seededHearing.GetParticipants().First(x => x.HearingRole.UserRole.Name.Equals("Representative"));

            var title = representativeParticipant.Person.Title + editPrefix;
            var displayName = representativeParticipant.DisplayName + editPrefix;
            var telephoneNumber = "11112222333";
            var organisationName = "Organisation" + editPrefix;
            
            var representee = "Iron Man Inc.";
            RepresentativeInformation repInfo = new RepresentativeInformation()
            {
                Representee = representee
            };
            var updateParticipantCommand = new UpdateParticipantCommand(_newHearingId, representativeParticipant.Id, title, displayName, telephoneNumber, organisationName, repInfo, null);
            await _commandHandler.Handle(updateParticipantCommand);

            var updatedRepresentative=(Representative) updateParticipantCommand.UpdatedParticipant;

            updatedRepresentative.Should().NotBeNull();
            updatedRepresentative.UpdatedDate.Should().BeAfter(beforeUpdatedDate);
            updatedRepresentative.Person.Title.Should().Be(title);
            updatedRepresentative.DisplayName.Should().Be(displayName);
            updatedRepresentative.Person.TelephoneNumber.Should().Be(telephoneNumber);
            updatedRepresentative.Person.Organisation.Should().NotBeNull();
            updatedRepresentative.Person.Organisation.Name.Should().Be(organisationName);
            updatedRepresentative.Representee.Should().Be(repInfo.Representee);
        }

        [Test]
        public async Task Should_Update_Participant_With_Links()
        {
            var editPrefix = " _Edit";
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");

            var individuals = seededHearing.GetParticipants()
                .Where(x => x.HearingRole.UserRole.Name.Equals("Individual")).ToList();
            var interpretee = individuals[0];
            var interpreter = individuals[1];

            var link = new LinkedParticipantDto(
                interpreter.Person.ContactEmail, 
                interpretee.Person.ContactEmail,
                LinkedParticipantType.Interpreter);

            var title = interpreter.Person.Title + editPrefix;
            var displayName = interpreter.DisplayName + editPrefix;
            var telephoneNumber = "11112222333";

            var links = new List<LinkedParticipantDto> {link};
            var updateParticipantCommand = new UpdateParticipantCommand(seededHearing.Id, interpreter.Id,
                title, displayName, telephoneNumber, null, null, links);
            await _commandHandler.Handle(updateParticipantCommand);
            
            var updatedRepresentative = updateParticipantCommand.UpdatedParticipant;
            
            updatedRepresentative.Should().NotBeNull();
            updatedRepresentative.LinkedParticipants.Should().NotBeNull();
            updatedRepresentative.GetLinkedParticipants().Should().NotBeEmpty();
        }

        [Test]
        public async Task Should_Update_Participant_With_Exisiting_Links()
        {
            var editPrefix = " _Edit";
            var seededHearing = await Hooks.SeedVideoHearing(withLinkedParticipants: true);
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");

            var individuals = seededHearing.GetParticipants().Where(x => x is Individual).ToList();
            var interpretee = individuals[0];
            var interpreter = individuals[1];

            var link = new LinkedParticipantDto(
                interpreter.Person.ContactEmail, 
                interpretee.Person.ContactEmail,
                LinkedParticipantType.Interpreter);

            var title = interpreter.Person.Title + editPrefix;
            var displayName = interpreter.DisplayName + editPrefix;
            var telephoneNumber = "11112222333";

            var links = new List<LinkedParticipantDto> { link };
            var updateParticipantCommand = new UpdateParticipantCommand(seededHearing.Id, interpreter.Id,
                title, displayName, telephoneNumber, null, null, links);
            await _commandHandler.Handle(updateParticipantCommand);

            var updatedRepresentative = updateParticipantCommand.UpdatedParticipant;

            updatedRepresentative.Should().NotBeNull();
            updatedRepresentative.LinkedParticipants.Should().NotBeNull();
            updatedRepresentative.GetLinkedParticipants().Should().NotBeEmpty();
        }

        [Test]
        public async Task Should_update_additional_information_when_passed_in()
        {
            var editPrefix = " _Edit";
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;
            var individualParticipant = seededHearing.GetParticipants().First(x=>x.HearingRole.UserRole.Name.Equals("Individual"));

            var title = individualParticipant.Person.Title;
            var displayName = individualParticipant.DisplayName;
            var telephoneNumber = individualParticipant.Person.TelephoneNumber;
            var organisationName = individualParticipant.Person.Organisation?.Name;
            var firstName = individualParticipant.Person.FirstName + editPrefix;
            var middleNames = individualParticipant.Person.MiddleNames + editPrefix;
            var lastName = individualParticipant.Person.LastName + editPrefix;
            var additionalInformation = new AdditionalInformation(firstName, lastName)
            {
                MiddleNames = middleNames
            };

            var updateParticipantCommand = new UpdateParticipantCommand(_newHearingId, individualParticipant.Id, title, displayName, telephoneNumber, organisationName, null, null, additionalInformation: additionalInformation);
            await _commandHandler.Handle(updateParticipantCommand);

            var updatedIndividual = (Individual)updateParticipantCommand.UpdatedParticipant;

            updatedIndividual.Should().NotBeNull();
            updatedIndividual.Person.FirstName.Should().Be(firstName);
            updatedIndividual.Person.MiddleNames.Should().Be(middleNames);
            updatedIndividual.Person.LastName.Should().Be(lastName);
        }
        
        [Test]
        public async Task Should_not_update_additional_information_when_not_passed_in()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;
            var individualParticipant = seededHearing.GetParticipants().First(x=>x.HearingRole.UserRole.Name.Equals("Individual"));

            var title = individualParticipant.Person.Title;
            var displayName = individualParticipant.DisplayName;
            var telephoneNumber = individualParticipant.Person.TelephoneNumber;
            var organisationName = individualParticipant.Person.Organisation?.Name;

            var updateParticipantCommand = new UpdateParticipantCommand(_newHearingId, individualParticipant.Id, title, displayName, telephoneNumber, organisationName, null, null, additionalInformation: null);
            await _commandHandler.Handle(updateParticipantCommand);

            var updatedIndividual = (Individual)updateParticipantCommand.UpdatedParticipant;

            updatedIndividual.Should().NotBeNull();
            updatedIndividual.Person.FirstName.Should().Be(individualParticipant.Person.FirstName);
            updatedIndividual.Person.MiddleNames.Should().Be(individualParticipant.Person.MiddleNames);
            updatedIndividual.Person.LastName.Should().Be(individualParticipant.Person.LastName);
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
    }
}