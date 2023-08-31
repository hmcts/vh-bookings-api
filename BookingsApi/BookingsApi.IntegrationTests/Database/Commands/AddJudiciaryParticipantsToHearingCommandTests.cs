using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Validations;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class AddJudiciaryParticipantsToHearingCommandTests : DatabaseTestsBase
    {
        private AddJudiciaryParticipantsToHearingCommandHandler _commandHandler;
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;
        
        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new AddJudiciaryParticipantsToHearingCommandHandler(context);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(context);
        }

        [TestCase(JudiciaryParticipantHearingRoleCode.Judge)]
        [TestCase(JudiciaryParticipantHearingRoleCode.PanelMember)]
        public async Task Should_add_judiciary_participant_to_hearing(JudiciaryParticipantHearingRoleCode judiciaryParticipantHearingRoleCode)
        {
            var seededHearing = await Hooks.SeedVideoHearing(configureOptions: options =>
            {
                options.AddJudge = false;
            });
            var personalCode = Guid.NewGuid().ToString();
            await Hooks.AddJudiciaryPerson(personalCode: personalCode);
            const string displayName = "Display Name";
            var hearingId = seededHearing.Id;
            var participants = new List<NewJudiciaryParticipant>
            {
                 new()
                 {
                     DisplayName = displayName,
                     PersonalCode = personalCode,
                     HearingRoleCode = judiciaryParticipantHearingRoleCode
                 }
            };
            var command = new AddJudiciaryParticipantsToHearingCommand(hearingId, participants);
            var beforeCount = seededHearing.GetJudiciaryParticipants().Count;

            await _commandHandler.Handle(command);
            
            var updatedHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            var newJudiciaryParticipants = updatedHearing.GetJudiciaryParticipants();
            var afterCount = newJudiciaryParticipants.Count;
            afterCount.Should().BeGreaterThan(beforeCount);
            var newJudiciaryParticipant = newJudiciaryParticipants.FirstOrDefault(x => x.JudiciaryPerson.PersonalCode == personalCode);
            newJudiciaryParticipant.Should().NotBeNull();
            newJudiciaryParticipant.DisplayName.Should().Be(displayName);
            newJudiciaryParticipant.JudiciaryPerson.PersonalCode.Should().Be(personalCode);
            newJudiciaryParticipant.HearingRoleCode.Should().Be(judiciaryParticipantHearingRoleCode);
            newJudiciaryParticipant.HearingId.Should().Be(hearingId);
        }
        
        [Test]
        public async Task Should_throw_exception_when_hearing_does_not_exist()
        {
            var personalCode = Guid.NewGuid().ToString();
            await Hooks.AddJudiciaryPerson(personalCode: personalCode);
            const string displayName = "Display Name";
            const JudiciaryParticipantHearingRoleCode hearingRoleCode = JudiciaryParticipantHearingRoleCode.PanelMember;
            var hearingId = Guid.NewGuid();
            var participants = new List<NewJudiciaryParticipant>
            {
                new()
                {
                    DisplayName = displayName,
                    PersonalCode = personalCode,
                    HearingRoleCode = hearingRoleCode
                }
            };
            var command = new AddJudiciaryParticipantsToHearingCommand(hearingId, participants);

            Assert.ThrowsAsync<HearingNotFoundException>(() => _commandHandler.Handle(command));
        }
        
        [Test]
        public async Task Should_throw_exception_when_judiciary_person_does_not_exist()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            const string displayName = "Display Name";
            var personalCode = Guid.NewGuid().ToString();
            const JudiciaryParticipantHearingRoleCode hearingRoleCode = JudiciaryParticipantHearingRoleCode.PanelMember;
            var hearingId = seededHearing.Id;
            var participants = new List<NewJudiciaryParticipant>
            {
                new()
                {
                    DisplayName = displayName,
                    PersonalCode = personalCode,
                    HearingRoleCode = hearingRoleCode
                }
            };
            var command = new AddJudiciaryParticipantsToHearingCommand(hearingId, participants);

            Assert.ThrowsAsync<JudiciaryPersonNotFoundException>(() => _commandHandler.Handle(command));
        }
    }
}
