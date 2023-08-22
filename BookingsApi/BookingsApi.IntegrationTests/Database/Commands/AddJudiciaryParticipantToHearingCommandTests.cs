using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Enumerations;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class AddJudiciaryParticipantToHearingCommandTests : DatabaseTestsBase
    {
        private AddJudiciaryParticipantToHearingCommandHandler _commandHandler;
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;
        
        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new AddJudiciaryParticipantToHearingCommandHandler(context);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(context);
        }

        [TestCase(HearingRoleCode.Judge)]
        [TestCase(HearingRoleCode.PanelMember)]
        public async Task Should_add_judiciary_participant_to_hearing(HearingRoleCode hearingRoleCode)
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            var personalCode = Guid.NewGuid().ToString();
            var seededJudiciaryPerson = await Hooks.AddJudiciaryPerson(personalCode: personalCode);
            const string displayName = "Display Name";
            var judiciaryPersonId = seededJudiciaryPerson.Id;
            var hearingId = seededHearing.Id;
            var command = new AddJudiciaryParticipantToHearingCommand(
                displayName,
                judiciaryPersonId,
                hearingRoleCode,
                hearingId);
            var beforeCount = seededHearing.GetJudiciaryParticipants().Count;

            await _commandHandler.Handle(command);
            
            var updatedHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            var newJudiciaryParticipants = updatedHearing.GetJudiciaryParticipants();
            var afterCount = newJudiciaryParticipants.Count;
            afterCount.Should().BeGreaterThan(beforeCount);
            var newJudiciaryParticipant = newJudiciaryParticipants.FirstOrDefault(x => x.JudiciaryPersonId == judiciaryPersonId);
            newJudiciaryParticipant.Should().NotBeNull();
            newJudiciaryParticipant.DisplayName.Should().Be(displayName);
            newJudiciaryParticipant.JudiciaryPersonId.Should().Be(judiciaryPersonId);
            newJudiciaryParticipant.HearingRoleCode.Should().Be(hearingRoleCode);
            newJudiciaryParticipant.HearingId.Should().Be(hearingId);
        }
        
        [Test]
        public async Task Should_throw_exception_when_hearing_does_not_exist()
        {
            var personalCode = Guid.NewGuid().ToString();
            var seededJudiciaryPerson = await Hooks.AddJudiciaryPerson(personalCode: personalCode);
            const string displayName = "Display Name";
            var judiciaryPersonId = seededJudiciaryPerson.Id;
            const HearingRoleCode hearingRoleCode = HearingRoleCode.PanelMember;
            var hearingId = Guid.NewGuid();
            var command = new AddJudiciaryParticipantToHearingCommand(
                displayName,
                judiciaryPersonId,
                hearingRoleCode,
                hearingId);

            Assert.ThrowsAsync<HearingNotFoundException>(() => _commandHandler.Handle(command));
        }
        
        [Test]
        public async Task Should_throw_exception_when_judiciary_person_does_not_exist()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            const string displayName = "Display Name";
            var judiciaryPersonId = Guid.NewGuid();
            const HearingRoleCode hearingRoleCode = HearingRoleCode.PanelMember;
            var hearingId = seededHearing.Id;
            var command = new AddJudiciaryParticipantToHearingCommand(
                displayName,
                judiciaryPersonId,
                hearingRoleCode,
                hearingId);

            Assert.ThrowsAsync<JudiciaryPersonNotFoundException>(() => _commandHandler.Handle(command));
        }
    }
}
