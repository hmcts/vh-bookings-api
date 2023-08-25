using System;
using BookingsApi.Domain.Validations;

namespace BookingsApi.UnitTests.Domain.Hearing
{
    public class AddJudiciaryParticipantTests
    {
        [Test]
        public void Should_add_new_judiciary_judge_to_hearing()
        {
            var hearing = new VideoHearingBuilder(addJudge: false).Build();
            var newJudiciaryPerson = new JudiciaryPersonBuilder().Build();
            var beforeAddCount = hearing.GetJudiciaryParticipants().Count;

            hearing.AddJudiciaryJudge(newJudiciaryPerson, "Display Name");
            
            var afterAddCount = hearing.GetJudiciaryParticipants().Count;
            afterAddCount.Should().BeGreaterThan(beforeAddCount);
        }
        
        [Test]
        public void Should_raise_exception_if_judiciary_judge_already_exists()
        {
            var hearing = new VideoHearingBuilder(addJudge: false).Build();
            var existingJudiciaryPerson = new JudiciaryPersonBuilder("Personal Code 1").Build();
            hearing.AddJudiciaryJudge(existingJudiciaryPerson, "Display Name 1");
            var newJudiciaryPerson = new JudiciaryPersonBuilder("Personal Code 2").Build();
            var beforeAddCount = hearing.GetJudiciaryParticipants().Count;

            var action = () => 
                hearing.AddJudiciaryJudge(newJudiciaryPerson, "Display Name 2");
            
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == "A participant with Judge role already exists in the hearing").Should().BeTrue();
            var afterAddCount = hearing.GetJudiciaryParticipants().Count;
            afterAddCount.Should().Be(beforeAddCount);
        }

        [Test]
        public void Should_raise_exception_if_judge_already_exists()
        {
            var hearing = new VideoHearingBuilder(addJudge: true).Build();
            var newJudiciaryPerson = new JudiciaryPersonBuilder().Build();
            var beforeAddCount = hearing.GetJudiciaryParticipants().Count;
            
            var action = () => 
                hearing.AddJudiciaryJudge(newJudiciaryPerson, "Display Name");
            
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == "A participant with Judge role already exists in the hearing").Should().BeTrue();
            var afterAddCount = hearing.GetJudiciaryParticipants().Count;
            afterAddCount.Should().Be(beforeAddCount);
        }
        
        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void Should_raise_exception_if_judiciary_judge_is_a_leaver(bool hasLeft, bool leaver)
        {
            var hearing = new VideoHearingBuilder(addJudge: false).Build();
            var newJudiciaryPerson = new JudiciaryPersonBuilder().Build();
            newJudiciaryPerson.HasLeft = hasLeft;
            newJudiciaryPerson.Leaver = leaver;
            newJudiciaryPerson.LeftOn = DateTime.UtcNow.AddYears(-1).ToShortDateString();
            var beforeAddCount = hearing.GetJudiciaryParticipants().Count;
            
            var action = () => 
                hearing.AddJudiciaryJudge(newJudiciaryPerson, "Display Name");
            
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == "Cannot add a participant who is a leaver").Should().BeTrue();
            var afterAddCount = hearing.GetJudiciaryParticipants().Count;
            afterAddCount.Should().Be(beforeAddCount);
        }
        
        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        public void Should_raise_exception_if_judiciary_judge_display_name_is_not_specified(string displayName)
        {
            var hearing = new VideoHearingBuilder(addJudge: false).Build();
            var newJudiciaryPerson = new JudiciaryPersonBuilder().Build();
            var beforeAddCount = hearing.GetJudiciaryParticipants().Count;
            
            var action = () => 
                hearing.AddJudiciaryJudge(newJudiciaryPerson, displayName);
            
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == "Display name cannot be empty").Should().BeTrue();
            var afterAddCount = hearing.GetJudiciaryParticipants().Count;
            afterAddCount.Should().Be(beforeAddCount);
        }

        [Test]
        public void Should_add_new_judiciary_panel_member_to_hearing()
        {
            var hearing = new VideoHearingBuilder(addJudge: false).Build();
            var newJudiciaryPerson = new JudiciaryPersonBuilder().Build();
            var beforeAddCount = hearing.GetJudiciaryParticipants().Count;

            hearing.AddJudiciaryPanelMember(newJudiciaryPerson, "Display Name");
            
            var afterAddCount = hearing.GetJudiciaryParticipants().Count;
            afterAddCount.Should().BeGreaterThan(beforeAddCount);
        }
        
        [Test]
        public void Should_raise_exception_if_judiciary_panel_member_already_exists()
        {
            var hearing = new VideoHearingBuilder(addJudge: false).Build();
            var existingJudiciaryPerson = new JudiciaryPersonBuilder().Build();
            hearing.AddJudiciaryJudge(existingJudiciaryPerson, "Display Name");
            var beforeAddCount = hearing.GetJudiciaryParticipants().Count;

            var action = () => 
                hearing.AddJudiciaryPanelMember(existingJudiciaryPerson, "Display Name");
            
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == "Judiciary participant already exists in the hearing").Should().BeTrue();
            var afterAddCount = hearing.GetJudiciaryParticipants().Count;
            afterAddCount.Should().Be(beforeAddCount);
        }
        
        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void Should_raise_exception_if_judiciary_panel_member_is_a_leaver(bool hasLeft, bool leaver)
        {
            var hearing = new VideoHearingBuilder(addJudge: false).Build();
            var newJudiciaryPerson = new JudiciaryPersonBuilder().Build();
            newJudiciaryPerson.HasLeft = hasLeft;
            newJudiciaryPerson.Leaver = leaver;
            newJudiciaryPerson.LeftOn = DateTime.UtcNow.AddYears(-1).ToShortDateString();
            var beforeAddCount = hearing.GetJudiciaryParticipants().Count;
            
            var action = () => 
                hearing.AddJudiciaryPanelMember(newJudiciaryPerson, "Display Name");
            
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == "Cannot add a participant who is a leaver").Should().BeTrue();
            var afterAddCount = hearing.GetJudiciaryParticipants().Count;
            afterAddCount.Should().Be(beforeAddCount);
        }
        
        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        public void Should_raise_exception_if_judiciary_panel_member_display_name_is_not_specified(string displayName)
        {
            var hearing = new VideoHearingBuilder(addJudge: false).Build();
            var newJudiciaryPerson = new JudiciaryPersonBuilder().Build();
            var beforeAddCount = hearing.GetJudiciaryParticipants().Count;
            
            var action = () => 
                hearing.AddJudiciaryPanelMember(newJudiciaryPerson, displayName);
            
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == "Display name cannot be empty").Should().BeTrue();
            var afterAddCount = hearing.GetJudiciaryParticipants().Count;
            afterAddCount.Should().Be(beforeAddCount);
        }
    }
}
