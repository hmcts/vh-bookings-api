using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Validations;
using Testing.Common.Builders.Domain;

namespace BookingsApi.IntegrationTests.Database.Commands;

public class RemoveJudiciaryParticipantFromHearingCommandTests : DatabaseTestsBase
{
    private RemoveJudiciaryParticipantFromHearingCommandHandler _commandHandler;
    private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;
    
    [SetUp]
    public void Setup()
    {
        var context = new BookingsDbContext(BookingsDbContextOptions);
        _commandHandler = new RemoveJudiciaryParticipantFromHearingCommandHandler(context);
        _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(context);
    }
    
    [Test]
    public void Should_throw_exception_when_hearing_does_not_exist()
    {
        var hearingId = Guid.NewGuid();

        var hearingRoleCode = JudiciaryParticipantHearingRoleCode.Judge;
        const string displayName = "Judiciary To Remove";
        var judiciaryPerson = new JudiciaryPersonBuilder(Guid.NewGuid().ToString()).Build();
        var judiciaryParticipant = new JudiciaryParticipant(displayName, judiciaryPerson, hearingRoleCode);
        Assert.ThrowsAsync<HearingNotFoundException>(() => _commandHandler.Handle(
            new RemoveJudiciaryParticipantFromHearingCommand(hearingId, judiciaryParticipant.JudiciaryPerson.PersonalCode)));
    }
    
    [Test]
    public async Task Should_throw_exception_when_participant_does_not_exist()
    {
        var seededHearing = await Hooks.SeedVideoHearing();
        TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");

        var judiciaryPerson = new JudiciaryPersonBuilder(Guid.NewGuid().ToString()).Build();
        var hearingRoleCode = JudiciaryParticipantHearingRoleCode.Judge;
        const string displayName = "Judiciary To Remove";
        var judiciaryParticipant = new JudiciaryParticipant(displayName, judiciaryPerson, hearingRoleCode);
        Assert.ThrowsAsync<DomainRuleException>(() => _commandHandler.Handle(
            new RemoveJudiciaryParticipantFromHearingCommand(seededHearing.Id, judiciaryParticipant.JudiciaryPerson.PersonalCode)));
    }
    
    [Test]
    public async Task Should_remove_judiciary_judge_from_hearing()
    {
        var seededHearing = await Hooks.SeedVideoHearingV2(options =>
        {
            options.AddJudge = true;
            options.AddStaffMember = true;
        });
        TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");

        var judiciaryParticipants = seededHearing.GetJudiciaryParticipants();
        var beforeRemoveCount = judiciaryParticipants.Count;
        var judgeToRemove = judiciaryParticipants[0];
        
        await _commandHandler.Handle(new RemoveJudiciaryParticipantFromHearingCommand(seededHearing.Id, judgeToRemove.JudiciaryPerson.PersonalCode));
        
        var updatedHearing = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
        var afterRemoveCount = updatedHearing.GetJudiciaryParticipants().Count;
        afterRemoveCount.Should().BeLessThan(beforeRemoveCount);
        updatedHearing.GetJudiciaryParticipants().Should().NotContain(x => x.Id == judgeToRemove.Id);
    }
}