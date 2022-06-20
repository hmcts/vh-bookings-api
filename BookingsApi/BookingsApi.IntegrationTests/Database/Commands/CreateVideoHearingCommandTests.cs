using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Services;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class CreateVideoHearingCommandTests : DatabaseTestsBase
    {
        private GetHearingByIdQueryHandler _queryHandler;
        private CreateVideoHearingCommandHandler _commandHandler;
        private Guid _newHearingId;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _queryHandler = new GetHearingByIdQueryHandler(context);
            var hearingService = new HearingService(context);
            _commandHandler = new CreateVideoHearingCommandHandler(context, hearingService);
            _newHearingId = Guid.Empty;
        }

        [Test]
        public async Task Should_be_able_to_save_video_hearing_to_database()
        {
            var caseTypeName = "Generic";
            var caseType = GetCaseTypeFromDb(caseTypeName);
            var hearingTypeName = "Automated Test";
            var hearingType = caseType.HearingTypes.First(x => x.Name == hearingTypeName);
            var scheduledDate = DateTime.Today.AddHours(10).AddMinutes(30);
            var duration = 45;
            var venue = new RefDataBuilder().HearingVenues.First();

            var applicantCaseRole = caseType.CaseRoles.First(x => x.Name == "Applicant");
            var applicantRepresentativeHearingRole =
                applicantCaseRole.HearingRoles.First(x => x.Name == "Representative");

            var judgeCaseRole = caseType.CaseRoles.First(x => x.Name == "Judge");
            var judgeHearingRole = judgeCaseRole.HearingRoles.First(x => x.Name == "Judge");

            var newPerson = new PersonBuilder(true).Build();
            var newJudgePerson = new PersonBuilder(true).Build();
            var newParticipant = new NewParticipant()
            {
                Person = newPerson,
                CaseRole = applicantCaseRole,
                HearingRole = applicantRepresentativeHearingRole,
                DisplayName = $"{newPerson.FirstName} {newPerson.LastName}",
                Representee = string.Empty
            };
            var newJudgeParticipant = new NewParticipant()
            {
                Person = newJudgePerson,
                CaseRole = judgeCaseRole,
                HearingRole = judgeHearingRole,
                DisplayName = $"{newJudgePerson.FirstName} {newJudgePerson.LastName}",
                Representee = string.Empty
            };
            var participants = new List<NewParticipant>()
            {
                newParticipant, newJudgeParticipant
            };
            var cases = new List<Case> {new Case("01234567890", "Test Add")};
            var hearingRoomName = "Room01";
            var otherInformation = "OtherInformation01";
            var createdBy = "User01";
            const bool questionnaireNotRequired = false;
            const bool audioRecordingRequired = true;

            var endpoints = new List<NewEndpoint>
            {
                new NewEndpoint
                {
                    DisplayName = "display 1",
                    Sip = Guid.NewGuid().ToString(),
                    Pin = "1234",
                    ContactEmail = null
                },
                new NewEndpoint
                {
                    DisplayName = "display 2",
                    Sip = Guid.NewGuid().ToString(),
                    Pin = "5678",
                    ContactEmail = null
                }
            };

            var linkedParticipants = new List<LinkedParticipantDto>
            {
                new LinkedParticipantDto(
                    newParticipant.Person.ContactEmail,
                    newJudgeParticipant.Person.ContactEmail,
                    LinkedParticipantType.Interpreter)
            };

            var command =
                new CreateVideoHearingCommand(caseType, hearingType, scheduledDate, duration, venue,
                    participants, cases, questionnaireNotRequired, audioRecordingRequired, endpoints,
                    linkedParticipants, false)
                {
                    HearingRoomName = hearingRoomName,
                    OtherInformation = otherInformation,
                    CreatedBy = createdBy
                };
            await _commandHandler.Handle(command);
            command.NewHearingId.Should().NotBeEmpty();
            _newHearingId = command.NewHearingId;
            Hooks.AddHearingForCleanup(_newHearingId);
            var returnedVideoHearing = await _queryHandler.Handle(new GetHearingByIdQuery(_newHearingId));

            returnedVideoHearing.Should().NotBeNull();

            returnedVideoHearing.CaseType.Should().NotBeNull();
            returnedVideoHearing.HearingVenue.Should().NotBeNull();
            returnedVideoHearing.HearingType.Should().NotBeNull();

            var participantsFromDb = returnedVideoHearing.GetParticipants();
            participantsFromDb.Any().Should().BeTrue();
            returnedVideoHearing.GetCases().Any().Should().BeTrue();
            returnedVideoHearing.GetEndpoints().Any().Should().BeTrue();
            var linkedParticipantsFromDb = participantsFromDb.SelectMany(x => x.LinkedParticipants).ToList();
            linkedParticipantsFromDb.Should().NotBeEmpty();
            foreach (var linkedParticipant in linkedParticipantsFromDb)
            {
                linkedParticipant.Type.Should().BeAssignableTo<LinkedParticipantType>();
                linkedParticipant.Type.Should().Be(LinkedParticipantType.Interpreter);
                participantsFromDb.Any(x => x.Id == linkedParticipant.LinkedId).Should().BeTrue();
                participantsFromDb.Any(x => x.Id == linkedParticipant.ParticipantId).Should().BeTrue();
                linkedParticipant.CreatedDate.Should().Be(linkedParticipant.UpdatedDate.Value);
            }

            returnedVideoHearing.GetEndpoints()[0].CreatedDate.Should().Be(returnedVideoHearing.GetEndpoints()[0].UpdatedDate.Value);
        }

        private CaseType GetCaseTypeFromDb(string caseTypeName)
        {
            CaseType caseType;
            using (var db = new BookingsDbContext(BookingsDbContextOptions))
            {
                caseType = db.CaseTypes
                    .Include(x => x.CaseRoles)
                    .ThenInclude(x => x.HearingRoles)
                    .ThenInclude(x => x.UserRole)
                    .Include(x => x.HearingTypes)
                    .First(x => x.Name == caseTypeName);
            }

            return caseType;
        }
    }
}