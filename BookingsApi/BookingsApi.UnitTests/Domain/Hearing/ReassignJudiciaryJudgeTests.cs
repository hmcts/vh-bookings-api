using BookingsApi.Domain.JudiciaryParticipants;

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
    }
}
