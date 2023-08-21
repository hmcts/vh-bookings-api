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
        public void Should_raise_exception_if_judiciary_participant_already_exists()
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
        public void Should_raise_exception_if_judiciary_participant_is_a_leaver(bool hasLeft, bool leaver)
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
        
        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        public void Should_raise_exception_if_display_name_is_not_specified(string displayName)
        {
            var hearing = new VideoHearingBuilder().Build();
            var newJudiciaryPerson = new JudiciaryPersonBuilder().Build();
            var beforeAddCount = hearing.GetJudiciaryParticipants().Count;
            
            var action = () => 
                hearing.AddJudiciaryParticipant(newJudiciaryPerson, displayName, HearingRoleCode.PanelMember);
            
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == "Display name cannot be empty").Should().BeTrue();
            var afterAddCount = hearing.GetJudiciaryParticipants().Count;
            afterAddCount.Should().Be(beforeAddCount);
        }

        [Test]
        public void Should_raise_exception_if_judiciary_person_is_null()
        {
            var hearing = new VideoHearingBuilder().Build();
            var beforeAddCount = hearing.GetJudiciaryParticipants().Count;
            
            var action = () => 
                hearing.AddJudiciaryParticipant(null, "Display Name", HearingRoleCode.PanelMember);
            
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == "Judiciary person cannot be null").Should().BeTrue();
            var afterAddCount = hearing.GetJudiciaryParticipants().Count;
            afterAddCount.Should().Be(beforeAddCount);
        }
    }
}
