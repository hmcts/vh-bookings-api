using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Validations;

namespace BookingsApi.UnitTests.Domain.Hearing;

public class RemoveJudiciaryParticipantTests
{
    [Test]
    public void Should_remove_judiciary_judge_from_hearing()
    {
        // arrange
        var hearing = new VideoHearingBuilder(addJudge: false).Build();
        var newJudiciaryPerson = new JudiciaryPersonBuilder(Guid.NewGuid().ToString()).Build();
        const string displayName = "Judiciary To Remove";
        hearing.AddJudiciaryJudge(newJudiciaryPerson, displayName);

        var judiciaryParticipants = hearing.GetJudiciaryParticipants();
        var beforeRemoveCount = judiciaryParticipants.Count;
        var judgeToRemove = judiciaryParticipants.First();
        
        // act
        hearing.RemoveJudiciaryParticipant(judgeToRemove);

        // assert
        var afterRemoveCount = hearing.GetJudiciaryParticipants().Count;
        afterRemoveCount.Should().BeLessThan(beforeRemoveCount);
        hearing.GetJudiciaryParticipants().Should().NotContain(x => x.Id == judgeToRemove.Id);
    }
    
    [Test]
    public void Should_raise_exception_if_judiciary_judge_does_not_exist()
    {
        // arrange
        var hearing = new VideoHearingBuilder(addJudge: false).Build();
        var judiciaryParticipants = hearing.GetJudiciaryParticipants();
        var beforeRemoveCount = judiciaryParticipants.Count;
        
        var judiciaryPerson = new JudiciaryPersonBuilder(Guid.NewGuid().ToString()).Build();
        var hearingRoleCode = JudiciaryParticipantHearingRoleCode.Judge;
        const string displayName = "Judiciary To Remove";
        var judiciaryParticipant = new JudiciaryParticipant(displayName, judiciaryPerson, hearingRoleCode);

        // act
        var action = () => hearing.RemoveJudiciaryParticipant(judiciaryParticipant);

        // assert
        action.Should().Throw<DomainRuleException>().And.ValidationFailures
            .Exists(x => x.Message == "Judiciary participant does not exist in the hearing").Should().BeTrue();
        var afterRemoveCount = hearing.GetJudiciaryParticipants().Count;
        afterRemoveCount.Should().Be(beforeRemoveCount);
    }

    [Test]
    public void Should_raise_exception_removing_judiciary_participant_results_in_no_host()
    {
        // arrange
        var hearing = new VideoHearingBuilder(addJudge: false, addStaffMember: false).Build();
        var newJudiciaryPerson = new JudiciaryPersonBuilder(Guid.NewGuid().ToString()).Build();
        const string displayName = "Judiciary To Remove";
        hearing.AddJudiciaryJudge(newJudiciaryPerson, displayName);

        var judiciaryParticipants = hearing.GetJudiciaryParticipants();
        var judgeToRemove = judiciaryParticipants.First();
        
        // act
        var action = () => hearing.RemoveJudiciaryParticipant(judgeToRemove);

        // assert
        action.Should().Throw<DomainRuleException>().And.ValidationFailures
            .Exists(x => x.Message == "A hearing must have at least one host").Should().BeTrue();
    }
}