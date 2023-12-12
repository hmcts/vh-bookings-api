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
    }
}
