﻿using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Services;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using BookingsApi.Domain.Validations;
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
        private List<string> _personsToRemove;

        private List<ExistingParticipantDetails> _existingParticipants { get; set; }
        private List<NewParticipant> _newParticipants { get; set; }
        private List<Guid> _removedParticipantIds { get; set; }
        private List<LinkedParticipantDto> _linkedParticipants { get; set; }
        private UpdateHearingParticipantsCommand _command { get; set; }

        [SetUp]
        public async Task Setup()
        {
            _personsToRemove = new List<string>();
            _context = new BookingsDbContext(BookingsDbContextOptions);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(_context);
            _genericCaseType = _context.CaseTypes
                    .Include(x => x.CaseRoles)
                    .ThenInclude(x => x.HearingRoles)
                    .ThenInclude(x => x.UserRole)
                    .Include(x => x.HearingTypes)
                    .First(x => x.Name == "Generic");

            var hearingService = new HearingService(_context);

            _hearing = await Hooks.SeedVideoHearing(status: BookingStatus.Created);
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
            var participantToRemoveOne = _hearing.Participants.First(p => p.HearingRole.Name != "Judge");
            var participantToRemoveTwo = _hearing.Participants.Last(p => p.HearingRole.Name != "Judge");
            var participantToRemoveIdOne = participantToRemoveOne.Id;
            var participantToRemoveIdTwo = participantToRemoveTwo.Id;

            _personsToRemove.Add(participantToRemoveOne.Person.ContactEmail);
            _personsToRemove.Add(participantToRemoveTwo.Person.ContactEmail);
            
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

            var oldJudge = _hearing.GetParticipants().Single(x => x.HearingRole.Id == judgeHearingRole.Id);
            _personsToRemove.Add(oldJudge.Person.ContactEmail);
            var oldJudgeId = oldJudge.Id;

            var newJudge = new PersonBuilder(true).Build();
            await _context.Persons.AddAsync(newJudge);
            await _context.SaveChangesAsync();
            _personsToRemove.Add(newJudge.ContactEmail);
            var displayName = $"{newJudge.FirstName} {newJudge.LastName}";
            var newParticipant = new NewParticipant()
            {
                Person = newJudge,
                CaseRole = judgeCaseRole,
                HearingRole = judgeHearingRole,
                DisplayName = displayName
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
            addedJudge.DisplayName.Should().Be(displayName);
        }

        [Test]
        public async Task Should_change_judge_within_30_minutes_of_hearing_starting()
        {
            // Arrange
            await UpdateHearingScheduledDateTime(DateTime.UtcNow.AddMinutes(15));

            var judgeCaseRole = _genericCaseType.CaseRoles.First(x => x.Name == "Judge");
            var judgeHearingRole =
                judgeCaseRole.HearingRoles.First(x => x.Name == "Judge");

            var oldJudge = _hearing.GetParticipants().Single(x => x.HearingRole.Id == judgeHearingRole.Id);
            _personsToRemove.Add(oldJudge.Person.ContactEmail);
            var oldJudgeId = oldJudge.Id;

            var newJudge = new PersonBuilder(true).Build();
            await _context.Persons.AddAsync(newJudge);
            await _context.SaveChangesAsync();
            _personsToRemove.Add(newJudge.ContactEmail);

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
            
            // Act
            await _handler.Handle(_command);
            var updatedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(_hearing.Id));
            var addedJudge = updatedVideoHearing.GetParticipants().SingleOrDefault(x => x.HearingRole.Id == judgeHearingRole.Id);
            
            // Assert
            addedJudge.Person.FirstName.Should().Be(newJudge.FirstName);
            addedJudge.Person.LastName.Should().Be(newJudge.LastName);
        }

        [Test]
        public async Task Should_not_be_able_to_remove_judge_within_30_minutes_of_hearing_starting()
        {
            // Arrange
            await UpdateHearingScheduledDateTime(DateTime.UtcNow.AddMinutes(15));

            var judgeCaseRole = _genericCaseType.CaseRoles.First(x => x.Name == "Judge");
            var judgeHearingRole =
                judgeCaseRole.HearingRoles.First(x => x.Name == "Judge");

            var oldJudge = _hearing.GetParticipants().Single(x => x.HearingRole.Id == judgeHearingRole.Id);
            _personsToRemove.Add(oldJudge.Person.ContactEmail);
            var oldJudgeId = oldJudge.Id;

            _removedParticipantIds.Add(oldJudgeId);
            _newParticipants.Clear();
            _command = BuildCommand();
            
            // Act & Assert
            Assert.ThrowsAsync<DomainRuleException>(async () =>
            {
                await _handler.Handle(_command);
            })!.Message.Should().Be(DomainRuleErrorMessages.CannotRemoveParticipantCloseToStartTime);
        }
        
        [Test]
        public async Task Should_not_be_able_to_remove_non_host_participant_within_30_minutes_of_hearing_starting()
        {
            // Arrange
            await UpdateHearingScheduledDateTime(DateTime.UtcNow.AddMinutes(15));

            var participantToRemove = _hearing.GetParticipants().First(x => x.Discriminator == "Individual");
            _personsToRemove.Add(participantToRemove.Person.ContactEmail);
            var participantToRemoveId = participantToRemove.Id;

            _removedParticipantIds.Add(participantToRemoveId);
            _newParticipants.Clear();
            _command = BuildCommand();
            
            // Act & Assert
            Assert.ThrowsAsync<DomainRuleException>(async () =>
            {
                await _handler.Handle(_command);
            })!.Message.Should().Be(DomainRuleErrorMessages.CannotRemoveParticipantCloseToStartTime);
        }

        private UpdateHearingParticipantsCommand BuildCommand()
        {
            return new UpdateHearingParticipantsCommand(_hearing.Id, _existingParticipants, _newParticipants, _removedParticipantIds, _linkedParticipants);
        }

        private async Task UpdateHearingScheduledDateTime(DateTime newScheduledDateTime)
        {
            var hearing = await _context.VideoHearings.FirstAsync(x => x.Id == _hearing.Id);
            hearing.SetProtected(nameof(_hearing.ScheduledDateTime), newScheduledDateTime);
            await _context.SaveChangesAsync();
        }

        [TearDown]
        public new async Task TearDown()
        {
            if (_hearing?.Id != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test hearing {_hearing.Id}");
                await Hooks.RemoveVideoHearing(_hearing.Id);
                _hearing = null;
            }

            await Hooks.ClearUnattachedPersons(_personsToRemove);
        }
    }
}
