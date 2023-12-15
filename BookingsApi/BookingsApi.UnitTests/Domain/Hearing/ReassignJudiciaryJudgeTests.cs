using BookingsApi.Contract.V1.Enums;
using BookingsApi.Domain;
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
        
        [Test]
        public void should_throw_exception_when_reassigning_null_judiciary_judge()
        {
            // Arrange
            var hearing = new VideoHearingBuilder().Build();
            
            // Act
            var action = () => hearing.ReassignJudiciaryJudge(null);
            
            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void should_throw_exception_when_adding_new_judiciary_judge_to_hearing_with_non_judiciary_judge()
        {
            // Arrange
            var hearing = new VideoHearingBuilder(addJudge: true).Build();
            var newJudiciaryPerson = new JudiciaryPersonBuilder().Build();
            var newJudiciaryJudge = new JudiciaryJudge("DisplayName", newJudiciaryPerson);
            
            // Act
            var action = () => hearing.ReassignJudiciaryJudge(newJudiciaryJudge);
            
            // Assert
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == DomainRuleErrorMessages.CannotAddJudiciaryJudgeWhenJudgeAlreadyExists).Should().BeTrue();
        }

        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void should_throw_exception_when_judiciary_person_is_a_leaver(bool hasLeft, bool leaver)
        {
            // Arrange
            var hearing = new VideoHearingBuilder(addJudge: false).Build();
            var newJudiciaryPerson = new JudiciaryPersonBuilder().Build();
            newJudiciaryPerson.HasLeft = hasLeft;
            newJudiciaryPerson.Leaver = leaver;
            newJudiciaryPerson.LeftOn = DateTime.UtcNow.AddYears(-1).ToShortDateString();
            var newJudiciaryJudge = new JudiciaryJudge("DisplayName", newJudiciaryPerson);
            
            // Act
            var action = () => hearing.ReassignJudiciaryJudge(newJudiciaryJudge);
            
            // Assert
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == "Cannot add a participant who is a leaver").Should().BeTrue();
        }

        [Test]
        public void should_throw_exception_when_judiciary_person_already_exists_on_hearing_with_different_role()
        {
            // Arrange
            var hearing = new VideoHearingBuilder(addJudge: false)
                .WithJudiciaryJudge()
                .Build();
            var newJudiciaryPerson = new JudiciaryPersonBuilder().Build();
            hearing.AddJudiciaryPanelMember(newJudiciaryPerson, "PanelMemberDisplayName");
            var newJudiciaryJudge = new JudiciaryJudge("DisplayName", newJudiciaryPerson);
            
            // Act
            var action = () => hearing.ReassignJudiciaryJudge(newJudiciaryJudge);
            
            // Assert
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == DomainRuleErrorMessages.JudiciaryPersonAlreadyExists(newJudiciaryPerson.PersonalCode)).Should().BeTrue();
        }

        [Test]
        public void should_reassign_same_judiciary_judge()
        {
            // Arrange
            var hearing = new VideoHearingBuilder(addJudge: false)
                .WithJudiciaryJudge()
                .Build();
            var judiciaryPerson = ((JudiciaryParticipant)hearing.GetJudge()).JudiciaryPerson;
            var judiciaryJudge = new JudiciaryJudge("DisplayName", judiciaryPerson);
            
            // Act
            hearing.ReassignJudiciaryJudge(judiciaryJudge);
            
            // Assert
            hearing.GetJudge().Should().Be(judiciaryJudge);
        }
    }
}
