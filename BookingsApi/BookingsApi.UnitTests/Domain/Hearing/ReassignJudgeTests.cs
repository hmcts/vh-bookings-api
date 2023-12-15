using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Validations;

namespace BookingsApi.UnitTests.Domain.Hearing
{
    public class ReassignJudgeTests
    {
        [Test]
        public void should_reassign_judge()
        {
            // Arrange
            var hearing = new VideoHearingBuilder().Build();
            var newJudge = new JudgeBuilder().Build();

            // Act
            hearing.ReassignJudge(newJudge);
            
            // Assert
            hearing.GetJudge().Should().Be(newJudge);
        }

        [Test]
        public void should_reassign_judge_to_hearing_without_judge()
        {
            // Arrange
            var hearing = new VideoHearingBuilder(addJudge: false).Build();
            var newJudge = new JudgeBuilder().Build();
            
            // Act
            hearing.ReassignJudge(newJudge);
            
            // Assert
            hearing.GetJudge().Should().Be(newJudge);
        }

        [Test]
        public void should_throw_exception_when_reassigning_judge_to_cancelled_hearing()
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

        [Test]
        public void should_throw_exception_when_reassigning_null_judge()
        {
            // Arrange
            var hearing = new VideoHearingBuilder().Build();
            
            // Act
            var action = () => hearing.ReassignJudge(null);
            
            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void should_throw_exception_when_adding_new_judge_to_hearing_with_judiciary_judge()
        {
            // Arrange
            var hearing = new VideoHearingBuilder(addJudge: false)
                .WithJudiciaryJudge()
                .Build();
            var newJudge = new JudgeBuilder().Build();
            
            // Act
            var action = () => hearing.ReassignJudge(newJudge);
            
            // Assert
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == DomainRuleErrorMessages.CannotAddJudgeWhenJudiciaryJudgeAlreadyExists).Should().BeTrue();
        }
    }
}
