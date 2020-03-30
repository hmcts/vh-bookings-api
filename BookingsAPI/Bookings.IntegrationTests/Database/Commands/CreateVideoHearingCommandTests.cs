using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL;
using Bookings.DAL.Commands;
using Bookings.DAL.Queries;
using Bookings.Domain;
using Bookings.Domain.RefData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace Bookings.IntegrationTests.Database.Commands
{
    public class CreateVideoHearingCommandTests : DatabaseTestsBase
    {
        private GetHearingByIdQueryHandler _queryHandler;
        private CreateVideoHearingCommandHandler _commandHandler;
        private Guid _newHearingId;
        private Guid _secondHearingId;
        
        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _queryHandler = new GetHearingByIdQueryHandler(context);
            var hearingService = new HearingService(context);
            _commandHandler = new CreateVideoHearingCommandHandler(context, hearingService);
            _newHearingId = Guid.Empty;
            _secondHearingId = Guid.Empty;
        }

        [Test]
        public async Task Should_be_able_to_save_video_hearing_to_database()
        {
            var caseTypeName = "Civil Money Claims";
            var caseType = GetCaseTypeFromDb(caseTypeName);
            var hearingTypeName = "Application to Set Judgment Aside";
            var hearingType = caseType.HearingTypes.First(x => x.Name == hearingTypeName);
            var scheduledDate = DateTime.Today.AddHours(10).AddMinutes(30);
            var duration = 45;
            var venue = new RefDataBuilder().HearingVenues.First();
            
            var claimantCaseRole = caseType.CaseRoles.First(x => x.Name == "Claimant");
            var claimantSolicitorHearingRole = claimantCaseRole.HearingRoles.First(x => x.Name == "Solicitor");

            var judgeCaseRole = caseType.CaseRoles.First(x => x.Name == "Judge");
            var judgeHearingRole = judgeCaseRole.HearingRoles.First(x => x.Name == "Judge");
            
            var newPerson = new PersonBuilder(true).Build();
            var newJudgePerson = new PersonBuilder(true).Build();
            var newParticipant = new NewParticipant()
            {
                Person = newPerson,
                CaseRole = claimantCaseRole,
                HearingRole = claimantSolicitorHearingRole,
                DisplayName = $"{newPerson.FirstName} {newPerson.LastName}",
                SolicitorsReference = string.Empty,
                Representee = string.Empty
            };
            var newJudgeParticipant = new NewParticipant()
            {
                Person = newJudgePerson,
                CaseRole = judgeCaseRole,
                HearingRole = judgeHearingRole,
                DisplayName = $"{newJudgePerson.FirstName} {newJudgePerson.LastName}",
                SolicitorsReference = string.Empty,
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

            var command =
                new CreateVideoHearingCommand(caseType, hearingType, scheduledDate, duration, venue, 
                    participants, cases, questionnaireNotRequired, audioRecordingRequired)
                {
                    HearingRoomName = hearingRoomName,
                    OtherInformation = otherInformation,
                    CreatedBy = createdBy
                };
            await _commandHandler.Handle(command);
            command.NewHearingId.Should().NotBeEmpty();
            _newHearingId = command.NewHearingId;
            
            var returnedVideoHearing = await  _queryHandler.Handle(new GetHearingByIdQuery(_newHearingId));

            returnedVideoHearing.Should().NotBeNull();
            
            returnedVideoHearing.CaseType.Should().NotBeNull();
            returnedVideoHearing.HearingVenue.Should().NotBeNull();
            returnedVideoHearing.HearingType.Should().NotBeNull();
            
            returnedVideoHearing.GetParticipants().Any().Should().BeTrue();
            returnedVideoHearing.GetCases().Any().Should().BeTrue();
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
        
        [TearDown]
        public new async Task TearDown()
        {
            if (_newHearingId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test hearing {_newHearingId}");
                await Hooks.RemoveVideoHearing(_newHearingId);
            }
            
            if (_secondHearingId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test hearing {_secondHearingId}");
                await Hooks.RemoveVideoHearing(_secondHearingId);
            }
        }
    }
}