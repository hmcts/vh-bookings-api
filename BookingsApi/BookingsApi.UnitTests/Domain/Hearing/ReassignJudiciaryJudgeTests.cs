using BookingsApi.Contract.V1.Enums;
using BookingsApi.Domain.JudiciaryParticipants;
using BookingsApi.Domain.Validations;

namespace BookingsApi.UnitTests.Domain.Hearing
{
    public class ReassignJudiciaryJudgeTests
    {
        [Test]
        public void should_reassign_judiciary_judge()
        {
            // Arrange
            var hearing = new VideoHearingBuilder(addJudge: false)
                .WithJudiciaryJudge()
                .Build();
            var newJudiciaryPerson = new JudiciaryPersonBuilder().Build();
            var newJudiciaryJudge = new JudiciaryJudge("DisplayName", newJudiciaryPerson);

            // Act
            hearing.ReassignJudiciaryJudge(newJudiciaryJudge);
            
            // Assert
            hearing.GetJudge().Should().Be(newJudiciaryJudge);
        }

        [Test]
        public void should_reassign_judiciary_judge_to_hearing_without_judge()
        {
            // Arrange
            var hearing = new VideoHearingBuilder(addJudge: false).Build();
            var newJudiciaryPerson = new JudiciaryPersonBuilder().Build();
            var newJudiciaryJudge = new JudiciaryJudge("DisplayName", newJudiciaryPerson);
            
            // Act
            hearing.ReassignJudiciaryJudge(newJudiciaryJudge);
            
            // Assert
            hearing.GetJudge().Should().Be(newJudiciaryJudge);
        }
        
        [Test]
        public void should_throw_exception_when_reassigning_judiciary_judge_to_cancelled_hearing()
        {
            // Arrange
            var hearing = new VideoHearingBuilder().Build();
            hearing.SetProtected(nameof(hearing.Status), BookingStatus.Cancelled);
            var newJudge = new JudgeBuilder().Build();
            
            // Act
            var action = () => hearing.ReassignJudge(newJudge);
            
            // Assert
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == DomainRuleErrorMessages.CannotEditACancelledHearing).Should().BeTrue();
        }
    }
}
