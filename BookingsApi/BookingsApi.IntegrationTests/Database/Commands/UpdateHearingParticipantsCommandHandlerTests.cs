using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Services;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testing.Common.Builders.Domain;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class UpdateHearingParticipantsCommandHandlerTests : DatabaseTestsBase
    {
        private UpdateHearingParticipantsCommandHandler _handler;
        private BookingsDbContext _context;
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;
        private VideoHearing _hearing;
        private CaseType _genericCaseType;

        private List<ExistingParticipantDetails> _existingParticipants { get; set; }
        private List<NewParticipant> _newParticipants { get; set; }
        private List<Guid> _removedParticipantIds { get; set; }
        private List<LinkedParticipantDto> _linkedParticipants { get; set; }
        private UpdateHearingParticipantsCommand _command { get; set; }

        [SetUp]
        public async Task Setup()
        {
            _context = new BookingsDbContext(BookingsDbContextOptions);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(_context);
            _genericCaseType = _context.CaseTypes
                    .Include(x => x.CaseRoles)
                    .ThenInclude(x => x.HearingRoles)
                    .ThenInclude(x => x.UserRole)
                    .Include(x => x.HearingTypes)
                    .First(x => x.Name == "Generic");

            var hearingService = new HearingService(_context);

            _hearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {_hearing.Id}");

            _existingParticipants = new List<ExistingParticipantDetails>();
            _newParticipants = new List<NewParticipant>();
            _removedParticipantIds = new List<Guid>();
            _linkedParticipants = new List<LinkedParticipantDto>();

            _handler = new UpdateHearingParticipantsCommandHandler(_context, hearingService);
        }

        [Test]
        public void Should_throw_hearing_not_found_exception_when_hearing_does_not_exist()
        {
            //Arrange
            _command = BuildCommand();
            _command.HearingId = Guid.Empty;

            //Act/Assert
            Assert.ThrowsAsync<HearingNotFoundException>(() => _handler.Handle(_command));
        }

        [Test]
        public async Task Should_add_participants_to_video_hearing()
        {
            //Arrange
            var originalParticipantCount = _hearing.GetParticipants().Count;

            var applicantCaseRole = _genericCaseType.CaseRoles.First(x => x.Name == "Applicant");
            var applicantRepresentativeHearingRole =
                applicantCaseRole.HearingRoles.First(x => x.Name == "Representative");

            var newPerson = new PersonBuilder(true).Build();
            var newParticipant = new NewParticipant()
            {
                Person = newPerson,
                CaseRole = applicantCaseRole,
                HearingRole = applicantRepresentativeHearingRole,
                DisplayName = $"{newPerson.FirstName} {newPerson.LastName}",
                Representee = string.Empty
            };
            
            _newParticipants.Add(newParticipant);
            _command = BuildCommand();

            //Act
            await _handler.Handle(_command);
            var updatedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(_hearing.Id));
            var newParticipantCount = updatedVideoHearing.GetParticipants().Count;

            //Assert
            newParticipantCount.Should().Be(originalParticipantCount + 1);
        }

        [Test]
        public void Should_throw_participant_not_found_exception_when_participant_does_not_exist()
        {
            //Arrange
            _existingParticipants.Add(new ExistingParticipantDetails { ParticipantId = Guid.Empty });
            _command = BuildCommand();

            //Act/Assert
            Assert.ThrowsAsync<ParticipantNotFoundException>(() => _handler.Handle(_command));
        }

        [Test]
        public async Task Should_update_participants_and_remove_any_existing_participant_links()
        {
            //Arrange
            _hearing = await Hooks.SeedVideoHearing(null, withLinkedParticipants: true);
            var beforeUpdatedDate = _hearing.UpdatedDate;
            var participantToUpdate = _hearing.GetParticipants().First(x => x.LinkedParticipants.Any());

            var updateParticipantDetails = new ExistingParticipantDetails
            {
                DisplayName = "UpdatedDisplayName",
                OrganisationName = "UpdatedOrganisation",
                ParticipantId = participantToUpdate.Id,
                TelephoneNumber = "07123456789",
                Title = "UpdatedTitle"
            };

            if (participantToUpdate.HearingRole.UserRole.IsRepresentative)
            {
                updateParticipantDetails.RepresentativeInformation = new RepresentativeInformation { Representee = "UpdatedRepresentee" };
            }

            _existingParticipants.Add(updateParticipantDetails);
            _command = BuildCommand();

            //Act
            await _handler.Handle(_command);

            var updatedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(_hearing.Id));
            var updatedParticipant = updatedVideoHearing.Participants.SingleOrDefault(x => x.Id == participantToUpdate.Id);

            updatedParticipant.Should().NotBeNull();
            updatedParticipant.UpdatedDate.Should().BeAfter(beforeUpdatedDate);
            updatedParticipant.Person.Title.Should().Be(updateParticipantDetails.Title);
            updatedParticipant.DisplayName.Should().Be(updateParticipantDetails.DisplayName);
            updatedParticipant.Person.TelephoneNumber.Should().Be(updateParticipantDetails.TelephoneNumber);
            updatedParticipant.Person.Organisation.Name.Should().Be(updateParticipantDetails.OrganisationName);
            updatedParticipant.LinkedParticipants.Should().BeEmpty();
            if (participantToUpdate.HearingRole.UserRole.IsRepresentative)
            {
                ((Representative)updatedParticipant).Representee.Should()
                     .Be(updateParticipantDetails.RepresentativeInformation.Representee);
            }
        }

        [Test]
        public async Task Should_update_representative_participants()
        {
            //Arrange
            _hearing = await Hooks.SeedVideoHearing(null, withLinkedParticipants: true);
            var beforeUpdatedDate = _hearing.UpdatedDate;
            var participantToUpdate = _hearing.GetParticipants().First(x => x.HearingRole.UserRole.Name.Equals("Representative"));

            var representee = "Represent Innit";
            var updateParticipantDetails = new ExistingParticipantDetails
            {
                DisplayName = "UpdatedDisplayName",
                OrganisationName = "UpdatedOrganisation",
                ParticipantId = participantToUpdate.Id,
                RepresentativeInformation = new RepresentativeInformation
                {
                    Representee = representee
                },
                TelephoneNumber = "07123456789",
                Title = "UpdatedTitle"
            };

            _existingParticipants.Add(updateParticipantDetails);
            _command = BuildCommand();

            //Act
            await _handler.Handle(_command);

            var updatedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(_hearing.Id));
            var updatedRepresentative = (Representative)updatedVideoHearing.Participants.SingleOrDefault(x => x.Id == participantToUpdate.Id);

            updatedRepresentative.Should().NotBeNull();
            updatedRepresentative.UpdatedDate.Should().BeAfter(beforeUpdatedDate);
            updatedRepresentative.Person.Title.Should().Be(updateParticipantDetails.Title);
            updatedRepresentative.DisplayName.Should().Be(updateParticipantDetails.DisplayName);
            updatedRepresentative.Person.TelephoneNumber.Should().Be(updateParticipantDetails.TelephoneNumber);
            updatedRepresentative.Person.Organisation.Name.Should().Be(updateParticipantDetails.OrganisationName);
            updatedRepresentative.LinkedParticipants.Should().BeEmpty();
            updatedRepresentative.Representee.Should().Be(representee);
        }

        [Test]
        public async Task Should_remove_participants()
        {
            //Arrange
            var participantToRemoveIdOne = _hearing.Participants[0].Id;
            var participantToRemoveIdTwo = _hearing.Participants[1].Id;

            _removedParticipantIds = new List<Guid>
            {
                participantToRemoveIdOne, participantToRemoveIdTwo
            };

            _command = BuildCommand();

            //Act
            await _handler.Handle(_command);

            var updatedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(_hearing.Id));
            var removedParticipantOne = (Representative)updatedVideoHearing.Participants.SingleOrDefault(x => x.Id == participantToRemoveIdOne);
            var removedParticipantTwo = (Representative)updatedVideoHearing.Participants.SingleOrDefault(x => x.Id == participantToRemoveIdTwo);

            removedParticipantOne.Should().BeNull();
            removedParticipantTwo.Should().BeNull();
        }

        [Test]
        public async Task Should_add_participant_links()
        {
            //Arrange
            var unlinkedParticipants = _hearing.Participants.Where(x => !x.LinkedParticipants.Any()).ToList();

            var primaryParticipant = unlinkedParticipants[0];
            var secondaryParticipant = unlinkedParticipants[1];

            _linkedParticipants = new List<LinkedParticipantDto>
            {
                new LinkedParticipantDto(primaryParticipant.Person.ContactEmail, secondaryParticipant.Person.ContactEmail, LinkedParticipantType.Interpreter)
            };

            _command = BuildCommand();

            //Act
            await _handler.Handle(_command);

            var updatedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(_hearing.Id));
            var linkedPrimaryParticipant = updatedVideoHearing.Participants.SingleOrDefault(x => x.Id == primaryParticipant.Id);
            var linkedSecondaryParticipant = updatedVideoHearing.Participants.SingleOrDefault(x => x.Id == secondaryParticipant.Id);

            linkedPrimaryParticipant.LinkedParticipants.Should().NotBeEmpty();
            linkedPrimaryParticipant.LinkedParticipants[0].ParticipantId.Should().Be(primaryParticipant.Id);
            linkedPrimaryParticipant.LinkedParticipants[0].LinkedId.Should().Be(secondaryParticipant.Id);

            linkedSecondaryParticipant.LinkedParticipants.Should().NotBeEmpty();
            linkedSecondaryParticipant.LinkedParticipants[0].ParticipantId.Should().Be(secondaryParticipant.Id);
            linkedSecondaryParticipant.LinkedParticipants[0].LinkedId.Should().Be(primaryParticipant.Id);
        }

        [Test]
        public async Task Should_change_judge()
        {
            //Arrange
            var judgeCaseRole = _genericCaseType.CaseRoles.First(x => x.Name == "Judge");
            var judgeHearingRole =
                judgeCaseRole.HearingRoles.First(x => x.Name == "Judge");

            var oldJudgeId = _hearing.GetParticipants().SingleOrDefault(x => x.HearingRole.Id == judgeHearingRole.Id).Id;

            var newJudge = new PersonBuilder(true).Build();

            var newParticipant = new NewParticipant()
            {
                Person = newJudge,
                CaseRole = judgeCaseRole,
                HearingRole = judgeHearingRole,
                DisplayName = $"{newJudge.FirstName} {newJudge.LastName}"
            };

            _removedParticipantIds.Add(oldJudgeId);
            _newParticipants.Add(newParticipant);
            _command = BuildCommand();

            //Act
            await _handler.Handle(_command);
            var updatedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(_hearing.Id));
            var addedJudge = updatedVideoHearing.GetParticipants().SingleOrDefault(x => x.HearingRole.Id == judgeHearingRole.Id);

            //Assert
            addedJudge.Person.FirstName.Should().Be(newJudge.FirstName);
            addedJudge.Person.LastName.Should().Be(newJudge.LastName);
        }


        private UpdateHearingParticipantsCommand BuildCommand()
        {
            return new UpdateHearingParticipantsCommand(_hearing.Id, _existingParticipants, _newParticipants, _removedParticipantIds, _linkedParticipants);
        }

        [TearDown]
        public new async Task TearDown()
        {
            if (_hearing.Id != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test hearing {_hearing.Id}");
                await Hooks.RemoveVideoHearing(_hearing.Id);
            }
        }
    }
}
