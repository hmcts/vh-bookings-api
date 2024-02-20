using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Validations;

namespace BookingsApi.UnitTests.Domain.Hearing;

public class RemoveJudiciaryParticipantByPersonalCodeTests
{
    [TestCase("")]
    [TestCase("UserName")]
    public void Should_remove_judiciary_judge_from_hearing(string removedBy)
    {
        // arrange
        var hearing = new VideoHearingBuilder(addJudge: false).Build();
        var newJudiciaryPerson = new JudiciaryPersonBuilder(Guid.NewGuid().ToString()).Build();
        const string displayName = "Judiciary To Remove";
        hearing.AddJudiciaryJudge(newJudiciaryPerson, displayName, createdBy: removedBy);

        var judiciaryParticipants = hearing.GetJudiciaryParticipants();
        var beforeRemoveCount = judiciaryParticipants.Count;
        var beforeUpdatedDate = hearing.UpdatedDate;
        var judgeToRemove = judiciaryParticipants[0];
        
        // act
        hearing.RemoveJudiciaryParticipantByPersonalCode(judgeToRemove.JudiciaryPerson.PersonalCode, removedBy: removedBy);

        // assert
        var afterRemoveCount = hearing.GetJudiciaryParticipants().Count;
        afterRemoveCount.Should().BeLessThan(beforeRemoveCount);
        hearing.GetJudiciaryParticipants().Should().NotContain(x => x.Id == judgeToRemove.Id);
        hearing.UpdatedDate.Should().BeAfter(beforeUpdatedDate);
        hearing.UpdatedBy.Should().Be(string.IsNullOrEmpty(removedBy) ? "System" : removedBy);
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
        var action = () => hearing.RemoveJudiciaryParticipantByPersonalCode(judiciaryParticipant.JudiciaryPerson.PersonalCode);

        // assert
        action.Should().Throw<DomainRuleException>().And.ValidationFailures
            .Exists(x => x.Message == "Judiciary participant does not exist in the hearing").Should().BeTrue();
        var afterRemoveCount = hearing.GetJudiciaryParticipants().Count;
        afterRemoveCount.Should().Be(beforeRemoveCount);
    }
}