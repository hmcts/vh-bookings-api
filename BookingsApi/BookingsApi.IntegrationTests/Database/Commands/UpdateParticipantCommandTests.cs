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
            var seededHearing = await Hooks.SeedVideoHearingV2();
            var beforeUpdatedDate = seededHearing.UpdatedDate;
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;
            var individualParticipant = seededHearing.GetParticipants().First(x=>x.HearingRole.UserRole.Name.Equals("Individual"));

            var title = individualParticipant.Person.Title + editPrefix;
            var displayName = individualParticipant.DisplayName + editPrefix;
            var telephoneNumber = "11112222333";
            var organisationName = "Organisation" + editPrefix;

            var requiredDto = new UpdateParticipantCommandRequiredDto(_newHearingId, individualParticipant.Id, title, displayName, telephoneNumber, organisationName, null);
            var updateParticipantCommand = new UpdateParticipantCommand(requiredDto);
            await _commandHandler.Handle(updateParticipantCommand);

            var updatedIndividual = (Individual)updateParticipantCommand.UpdatedParticipant;

            updatedIndividual.Should().NotBeNull();
            updatedIndividual.UpdatedDate.Should().BeAfter(beforeUpdatedDate);
            updatedIndividual.Person.Title.Should().Be(title);
            updatedIndividual.DisplayName.Should().Be(displayName);
            updatedIndividual.Person.TelephoneNumber.Should().Be(telephoneNumber);
            updatedIndividual.UpdatedDate.Should().BeAfter(updatedIndividual.CreatedDate.Value);
        }

        [Test]
        public async Task Should_be_able_to_update_representative_participant()
        {
            var editPrefix = " _Edit";
            var seededHearing = await Hooks.SeedVideoHearingV2();
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
            var requiredDto = new UpdateParticipantCommandRequiredDto(_newHearingId, representativeParticipant.Id, title, displayName, telephoneNumber, organisationName, null);
            var optionalDto =
                new UpdateParticipantCommandOptionalDto(repInfo, null, null, null, null, null, null, null);
            var updateParticipantCommand = new UpdateParticipantCommand(requiredDto, optionalDto);
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
            var seededHearing = await Hooks.SeedVideoHearingV2();
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
            var requiredDto = new UpdateParticipantCommandRequiredDto(seededHearing.Id, interpreter.Id, title, displayName, telephoneNumber, null, links);
            var updateParticipantCommand = new UpdateParticipantCommand(requiredDto);
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
            var seededHearing = await Hooks.SeedVideoHearingV2(withLinkedParticipants: true);
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
            var requiredDto = new UpdateParticipantCommandRequiredDto(seededHearing.Id, interpreter.Id, title, displayName, telephoneNumber, null, links);
            var updateParticipantCommand = new UpdateParticipantCommand(requiredDto);
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
            var seededHearing = await Hooks.SeedVideoHearingV2();
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

            var requiredDto = new UpdateParticipantCommandRequiredDto(_newHearingId, individualParticipant.Id, title, displayName, telephoneNumber, organisationName, null);
            var optionalDto = new UpdateParticipantCommandOptionalDto(null, additionalInformation, null, null, null, null, null, null);
            var updateParticipantCommand = new UpdateParticipantCommand(requiredDto, optionalDto);
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
            var seededHearing = await Hooks.SeedVideoHearingV2();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;
            var individualParticipant = seededHearing.GetParticipants().First(x=>x.HearingRole.UserRole.Name.Equals("Individual"));

            var title = individualParticipant.Person.Title;
            var displayName = individualParticipant.DisplayName;
            var telephoneNumber = individualParticipant.Person.TelephoneNumber;
            var organisationName = individualParticipant.Person.Organisation?.Name;

            var requiredDto = new UpdateParticipantCommandRequiredDto(_newHearingId, individualParticipant.Id, title, displayName, telephoneNumber, organisationName, null);
            var updateParticipantCommand = new UpdateParticipantCommand(requiredDto);
            await _commandHandler.Handle(updateParticipantCommand);

            var updatedIndividual = (Individual)updateParticipantCommand.UpdatedParticipant;

            updatedIndividual.Should().NotBeNull();
            updatedIndividual.Person.FirstName.Should().Be(individualParticipant.Person.FirstName);
            updatedIndividual.Person.MiddleNames.Should().Be(individualParticipant.Person.MiddleNames);
            updatedIndividual.Person.LastName.Should().Be(individualParticipant.Person.LastName);
        }

        [Test]
        public async Task Should_update_contact_email_when_passed_in()
        {
            var seededHearing = await Hooks.SeedVideoHearingV2();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;
            var individualParticipant = seededHearing.GetParticipants().First(x=>x.HearingRole.UserRole.Name.Equals("Individual"));

            var title = individualParticipant.Person.Title;
            var displayName = individualParticipant.DisplayName;
            var telephoneNumber = individualParticipant.Person.TelephoneNumber;
            var organisationName = individualParticipant.Person.Organisation?.Name;
            var firstName = individualParticipant.Person.FirstName;
            var middleNames = individualParticipant.Person.MiddleNames;
            var lastName = individualParticipant.Person.LastName;
            var additionalInformation = new AdditionalInformation(firstName, lastName)
            {
                MiddleNames = middleNames
            };
            var contactEmail = "editedContactEmail@email.com";

            var requiredDto = new UpdateParticipantCommandRequiredDto(_newHearingId, individualParticipant.Id, title, displayName, telephoneNumber, organisationName, null);
            var optionalDto = new UpdateParticipantCommandOptionalDto(null, additionalInformation, contactEmail, null,
                null, null, null, null);
            var updateParticipantCommand = new UpdateParticipantCommand(requiredDto, optionalDto);
            await _commandHandler.Handle(updateParticipantCommand);

            var updatedIndividual = (Individual)updateParticipantCommand.UpdatedParticipant;

            updatedIndividual.Should().NotBeNull();
            updatedIndividual.Person.ContactEmail.Should().Be(contactEmail);
        }
        
        [Test]
        public async Task Should_not_update_contact_email_when_not_passed_in()
        {
            var seededHearing = await Hooks.SeedVideoHearingV2();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;
            var individualParticipant = seededHearing.GetParticipants().First(x=>x.HearingRole.UserRole.Name.Equals("Individual"));

            var title = individualParticipant.Person.Title;
            var displayName = individualParticipant.DisplayName;
            var telephoneNumber = individualParticipant.Person.TelephoneNumber;
            var organisationName = individualParticipant.Person.Organisation?.Name;
            var firstName = individualParticipant.Person.FirstName;
            var middleNames = individualParticipant.Person.MiddleNames;
            var lastName = individualParticipant.Person.LastName;
            var additionalInformation = new AdditionalInformation(firstName, lastName)
            {
                MiddleNames = middleNames
            };

            var requiredDto = new UpdateParticipantCommandRequiredDto(_newHearingId, individualParticipant.Id, title, displayName, telephoneNumber, organisationName, null);
            var optionalDto =
                new UpdateParticipantCommandOptionalDto(null, additionalInformation, null, null, null, null, null,
                    null);
            var updateParticipantCommand = new UpdateParticipantCommand(requiredDto, optionalDto);
            await _commandHandler.Handle(updateParticipantCommand);

            var updatedIndividual = (Individual)updateParticipantCommand.UpdatedParticipant;

            updatedIndividual.Should().NotBeNull();
            updatedIndividual.Person.ContactEmail.Should().Be(individualParticipant.Person.ContactEmail);
        }
        
        [Test]
        public async Task Should_use_contact_email_for_different_person_when_passed_in_contact_email_is_same_as_different_person()
        {
            var seededHearing = await Hooks.SeedVideoHearingV2();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;
            var individualParticipant = seededHearing.GetParticipants().First(x=>x.HearingRole.UserRole.Name.Equals("Individual"));

            var seededHearing2 = await Hooks.SeedVideoHearingV2();
            var individualParticipant2 = seededHearing2.GetParticipants().First(x => x.HearingRole.UserRole.Name.Equals("Individual"));
            
            var title = individualParticipant.Person.Title;
            var displayName = individualParticipant.DisplayName;
            var telephoneNumber = individualParticipant.Person.TelephoneNumber;
            var organisationName = individualParticipant.Person.Organisation?.Name;
            var firstName = individualParticipant.Person.FirstName;
            var middleNames = individualParticipant.Person.MiddleNames;
            var lastName = individualParticipant.Person.LastName;
            var additionalInformation = new AdditionalInformation(firstName, lastName)
            {
                MiddleNames = middleNames
            };
            var contactEmail = individualParticipant2.Person.ContactEmail;

            var requiredDto = new UpdateParticipantCommandRequiredDto(_newHearingId, individualParticipant.Id, title, displayName, telephoneNumber, organisationName, null);
            var optionalDto = new UpdateParticipantCommandOptionalDto(null, additionalInformation, contactEmail, null,
                null, null, null, null);
            var updateParticipantCommand = new UpdateParticipantCommand(requiredDto, optionalDto);
            await _commandHandler.Handle(updateParticipantCommand);

            var updatedIndividual = (Individual)updateParticipantCommand.UpdatedParticipant;

            updatedIndividual.Should().NotBeNull();
            updatedIndividual.Person.ContactEmail.Should().Be(individualParticipant2.Person.ContactEmail);
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