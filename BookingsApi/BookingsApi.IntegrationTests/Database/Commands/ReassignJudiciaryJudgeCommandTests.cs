using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Services;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.JudiciaryParticipants;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class ReassignJudiciaryJudgeCommandTests : DatabaseTestsBase
    {
        private ReassignJudiciaryJudgeCommandHandler _commandHandler;
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new ReassignJudiciaryJudgeCommandHandler(context);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(context);
        }

        [Test]
        public async Task Should_reassign_judiciary_judge_when_hearing_has_a_judge()
        {
            var seededHearing = await Hooks.SeedVideoHearingV2(configureOptions: options =>
            {
                options.AddJudge = true;
            }, status: BookingStatus.Created);
            var personalCode = Guid.NewGuid().ToString();
            await Hooks.AddJudiciaryPerson(personalCode: personalCode);
            var hearingId = seededHearing.Id;
            var newJudiciaryJudge = new NewJudiciaryJudge
            {
                DisplayName = "DisplayName",
                PersonalCode = personalCode
            };
            var command = new ReassignJudiciaryJudgeCommand(hearingId, newJudiciaryJudge);

            await _commandHandler.Handle(command);
            
            var updatedHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            var newAssignedJudge = (JudiciaryParticipant)updatedHearing.GetJudge();
            newAssignedJudge.Should().NotBeNull();
            newAssignedJudge.JudiciaryPerson.PersonalCode.Should().Be(personalCode);
        }

        [Test]
        public async Task Should_reassign_judiciary_judge_when_hearing_does_not_have_a_judge()
        {
            var seededHearing = await Hooks.SeedVideoHearingV2(configureOptions: options =>
            {
                options.AddJudge = false;
            }, status: BookingStatus.Created);
            var personalCode = Guid.NewGuid().ToString();
            await Hooks.AddJudiciaryPerson(personalCode: personalCode);
            var hearingId = seededHearing.Id;
            var newJudiciaryJudge = new NewJudiciaryJudge
            {
                DisplayName = "DisplayName",
                PersonalCode = personalCode
            };
            var command = new ReassignJudiciaryJudgeCommand(hearingId, newJudiciaryJudge);

            await _commandHandler.Handle(command);
            
            var updatedHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            var newAssignedJudge = (JudiciaryParticipant)updatedHearing.GetJudge();
            newAssignedJudge.Should().NotBeNull();
            newAssignedJudge.JudiciaryPerson.PersonalCode.Should().Be(personalCode);
        }
        
        [Test]
        public async Task Should_throw_exception_when_hearing_does_not_exist()
        {
            var personalCode = Guid.NewGuid().ToString();
            await Hooks.AddJudiciaryPerson(personalCode: personalCode);
            var hearingId = Guid.NewGuid();
            var newJudiciaryJudge = new NewJudiciaryJudge
            {
                DisplayName = "DisplayName",
                PersonalCode = personalCode
            };

            var command = new ReassignJudiciaryJudgeCommand(hearingId, newJudiciaryJudge);

            Assert.ThrowsAsync<HearingNotFoundException>(() => _commandHandler.Handle(command));
        }
        
        [Test]
        public async Task Should_throw_exception_when_judiciary_person_does_not_exist()
        {
            var seededHearing = await Hooks.SeedVideoHearingV2(configureOptions: options =>
            {
                options.AddJudge = true;
            }, status: BookingStatus.Created);
            var personalCode = Guid.NewGuid().ToString();
            var hearingId = seededHearing.Id;
            var newJudiciaryJudge = new NewJudiciaryJudge
            {
                DisplayName = "DisplayName",
                PersonalCode = personalCode
            };
            
            var command = new ReassignJudiciaryJudgeCommand(hearingId, newJudiciaryJudge);

            Assert.ThrowsAsync<JudiciaryPersonNotFoundException>(() => _commandHandler.Handle(command));
        }
    }
}
