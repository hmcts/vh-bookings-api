using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Participants;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class UpdateParticipantCommandDatabaseTests : DatabaseTestsBase
    {
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;
        private GetHearingVenuesQueryHandler _getHearingVenuesQueryHandler;
        private UpdateParticipantCommandHandler _commandHandler;
        private Guid _newHearingId;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(context);
            _getHearingVenuesQueryHandler = new GetHearingVenuesQueryHandler(context);
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
            
            var link = new LinkedParticipantDto
            {
                LinkedParticipantContactEmail = interpreter.Person.ContactEmail, 
                ParticipantContactEmail = interpretee.Person.ContactEmail
            };

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
            var seededHearing = await Hooks.SeedVideoHearingLinkedParticipants(null);
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");

            var individuals = seededHearing.GetParticipants()
                .Where(x => x.HearingRole.UserRole.Name.Equals("Individual")).ToList();
            var interpretee = individuals[0];
            var interpreter = individuals[1];

            var link = new LinkedParticipantDto
            {
                LinkedParticipantContactEmail = interpreter.Person.ContactEmail,
                ParticipantContactEmail = interpretee.Person.ContactEmail
            };

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