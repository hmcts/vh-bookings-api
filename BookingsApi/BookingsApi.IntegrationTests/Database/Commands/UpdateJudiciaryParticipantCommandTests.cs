using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Validations;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class UpdateJudiciaryParticipantCommandTests : DatabaseTestsBase
    {
        private UpdateJudiciaryParticipantCommandHandler _commandHandler;
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;
        
        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new UpdateJudiciaryParticipantCommandHandler(context);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(context);
        }

        [Test]
        public async Task Should_update_judiciary_judge()
        {
            var seededHearing = await Hooks.SeedVideoHearing(configureOptions: options =>
            {
                options.AddJudge = false;
                options.AddJudiciaryJudge = true;
                options.AddStaffMember = true;
            });
            var hearingId = seededHearing.Id;
            var judiciaryJudge = seededHearing.JudiciaryParticipants.FirstOrDefault(x => x.HearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge);
            var personalCode = judiciaryJudge.JudiciaryPerson.PersonalCode;
            const string displayName = "New Display Name";
            const JudiciaryParticipantHearingRoleCode hearingRoleCode = JudiciaryParticipantHearingRoleCode.PanelMember;
            var command = new UpdateJudiciaryParticipantCommand(hearingId, personalCode, displayName, hearingRoleCode);

            await _commandHandler.Handle(command);
            
            var updatedHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            var newJudiciaryParticipants = updatedHearing.GetJudiciaryParticipants();
            var updatedJudiciaryParticipant = newJudiciaryParticipants.FirstOrDefault(x => x.JudiciaryPerson.PersonalCode == personalCode);
            updatedJudiciaryParticipant.Should().NotBeNull();
            updatedJudiciaryParticipant.DisplayName.Should().Be(displayName);
            updatedJudiciaryParticipant.JudiciaryPerson.PersonalCode.Should().Be(personalCode);
            updatedJudiciaryParticipant.HearingRoleCode.Should().Be(hearingRoleCode);
            updatedJudiciaryParticipant.HearingId.Should().Be(hearingId);
        }
        
        [Test]
        public async Task Should_update_judiciary_panel_member()
        {
            var seededHearing = await Hooks.SeedVideoHearing(configureOptions: options =>
            {
                options.AddJudge = false;
                options.AddJudiciaryPanelMember = true;
                options.AddStaffMember = true;
            });
            var hearingId = seededHearing.Id;
            var judiciaryPanelMember = seededHearing.JudiciaryParticipants.FirstOrDefault(x => x.HearingRoleCode == JudiciaryParticipantHearingRoleCode.PanelMember);
            var personalCode = judiciaryPanelMember.JudiciaryPerson.PersonalCode;
            const string displayName = "New Display Name";
            const JudiciaryParticipantHearingRoleCode hearingRoleCode = JudiciaryParticipantHearingRoleCode.Judge;
            var command = new UpdateJudiciaryParticipantCommand(hearingId, personalCode, displayName, hearingRoleCode);

            await _commandHandler.Handle(command);
            
            var updatedHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            var newJudiciaryParticipants = updatedHearing.GetJudiciaryParticipants();
            var updatedJudiciaryParticipant = newJudiciaryParticipants.FirstOrDefault(x => x.JudiciaryPerson.PersonalCode == personalCode);
            updatedJudiciaryParticipant.Should().NotBeNull();
            updatedJudiciaryParticipant.DisplayName.Should().Be(displayName);
            updatedJudiciaryParticipant.JudiciaryPerson.PersonalCode.Should().Be(personalCode);
            updatedJudiciaryParticipant.HearingRoleCode.Should().Be(hearingRoleCode);
            updatedJudiciaryParticipant.HearingId.Should().Be(hearingId);
        }
        
        [Test]
        public void Should_throw_exception_when_hearing_does_not_exist()
        {
            var hearingId = Guid.NewGuid();
            var personalCode = Guid.NewGuid().ToString();
            const string displayName = "Display Name";
            const JudiciaryParticipantHearingRoleCode hearingRoleCode = JudiciaryParticipantHearingRoleCode.Judge;
            var command = new UpdateJudiciaryParticipantCommand(hearingId, personalCode, displayName, hearingRoleCode);

            Assert.ThrowsAsync<HearingNotFoundException>(() => _commandHandler.Handle(command));
        }
        
        [Test]
        public async Task Should_throw_exception_when_judiciary_person_does_not_exist()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            var hearingId = seededHearing.Id;
            var personalCode = Guid.NewGuid().ToString();
            const string displayName = "Display Name";
            const JudiciaryParticipantHearingRoleCode hearingRoleCode = JudiciaryParticipantHearingRoleCode.PanelMember;
            var command = new UpdateJudiciaryParticipantCommand(hearingId, personalCode, displayName, hearingRoleCode);

            Assert.ThrowsAsync<DomainRuleException>(() => _commandHandler.Handle(command));
        }
    }
}
