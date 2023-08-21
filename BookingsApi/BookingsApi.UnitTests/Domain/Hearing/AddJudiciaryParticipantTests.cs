using System;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Validations;

namespace BookingsApi.UnitTests.Domain.Hearing
{
    public class AddJudiciaryParticipantTests
    {
        [TestCase(HearingRoleCode.Judge)]
        [TestCase(HearingRoleCode.PanelMember)]
        public void Should_add_new_judiciary_participant_to_hearing(HearingRoleCode hearingRoleCode)
        {
            var hearing = new VideoHearingBuilder().Build();
            var newJudiciaryPerson = new JudiciaryPersonBuilder().Build();
            var beforeAddCount = hearing.GetJudiciaryParticipants().Count;

            hearing.AddJudiciaryParticipant(newJudiciaryPerson, "Display Name", hearingRoleCode);
            
            var afterAddCount = hearing.GetJudiciaryParticipants().Count;
            afterAddCount.Should().BeGreaterThan(beforeAddCount);
        }

        [Test]
        public void Should_not_add_existing_judiciary_participant_to_hearing()
        {
            var hearing = new VideoHearingBuilder().Build();
            var existingJudiciaryPerson = new JudiciaryPersonBuilder().Build();
            hearing.AddJudiciaryParticipant(existingJudiciaryPerson, "Display Name", HearingRoleCode.PanelMember);
            var beforeAddCount = hearing.GetJudiciaryParticipants().Count;

            var action = () => 
                hearing.AddJudiciaryParticipant(existingJudiciaryPerson, "Display Name", HearingRoleCode.PanelMember);
            
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == "Judiciary participant already exists in the hearing").Should().BeTrue();
            var afterAddCount = hearing.GetJudiciaryParticipants().Count;
            afterAddCount.Should().Be(beforeAddCount);
        }

        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void Should_not_add_leaver_judiciary_participant_to_hearing(bool hasLeft, bool leaver)
        {
            var hearing = new VideoHearingBuilder().Build();
            var newJudiciaryPerson = new JudiciaryPersonBuilder().Build();
            newJudiciaryPerson.HasLeft = hasLeft;
            newJudiciaryPerson.Leaver = leaver;
            newJudiciaryPerson.LeftOn = DateTime.UtcNow.AddYears(-1).ToShortDateString();
            var beforeAddCount = hearing.GetJudiciaryParticipants().Count;
            
            var action = () => 
                hearing.AddJudiciaryParticipant(newJudiciaryPerson, "Display Name", HearingRoleCode.PanelMember);
            
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == "Cannot add a participant who is a leaver").Should().BeTrue();
            var afterAddCount = hearing.GetJudiciaryParticipants().Count;
            afterAddCount.Should().Be(beforeAddCount);
        }
    }
}
