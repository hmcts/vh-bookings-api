using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Validations;

namespace BookingsApi.UnitTests.Domain.Hearing
{
    public class UpdateJudiciaryParticipantTests
    {
        [Test]
        public void Should_update_judiciary_judge_in_hearing()
        {
            var hearing = new VideoHearingBuilder(addJudge: false)
                .WithJudiciaryJudge()
                .Build();
            var judiciaryJudge = hearing.GetJudiciaryParticipants()
                .FirstOrDefault(x => x.HearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge);
            var newDisplayName = "New Display Name";
            var newHearingRoleCode = JudiciaryParticipantHearingRoleCode.PanelMember;

            hearing.UpdateJudiciaryJudge(judiciaryJudge, newDisplayName, newHearingRoleCode);

            var updatedJudiciaryJudge = hearing.GetJudiciaryParticipants()
                .FirstOrDefault(x => x.JudiciaryPerson.PersonalCode == judiciaryJudge.JudiciaryPerson.PersonalCode);
            updatedJudiciaryJudge.DisplayName.Should().Be(newDisplayName);
            updatedJudiciaryJudge.HearingRoleCode.Should().Be(newHearingRoleCode);
        }

        [TestCase(" ")]
        [TestCase("")]
        [TestCase(null)]
        public void Should_raise_exception_if_judiciary_judge_display_name_is_not_specified(string displayName)
        {
            var hearing = new VideoHearingBuilder(addJudge: false)
                .WithJudiciaryJudge()
                .Build();
            var judiciaryJudge = hearing.GetJudiciaryParticipants()
                .FirstOrDefault(x => x.HearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge);
            var newDisplayName = displayName;
            var newHearingRoleCode = JudiciaryParticipantHearingRoleCode.PanelMember;
            
            var action = () => 
                hearing.UpdateJudiciaryJudge(judiciaryJudge, newDisplayName, newHearingRoleCode);
            
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == "Display name cannot be empty").Should().BeTrue();
        }

        [Test]
        public void Should_update_judiciary_panel_member_in_hearing()
        {
            var hearing = new VideoHearingBuilder(addJudge: false)
                .WithJudiciaryPanelMember()
                .Build();
            var judiciaryPanelMember = hearing.GetJudiciaryParticipants()
                .FirstOrDefault(x => x.HearingRoleCode == JudiciaryParticipantHearingRoleCode.PanelMember);
            var newDisplayName = "New Display Name";
            var newHearingRoleCode = JudiciaryParticipantHearingRoleCode.Judge;

            hearing.UpdateJudiciaryPanelMember(judiciaryPanelMember, newDisplayName, newHearingRoleCode);

            var updatedJudiciaryPanelMember = hearing.GetJudiciaryParticipants()
                .FirstOrDefault(x => x.JudiciaryPerson.PersonalCode == judiciaryPanelMember.JudiciaryPerson.PersonalCode);
            updatedJudiciaryPanelMember.DisplayName.Should().Be(newDisplayName);
            updatedJudiciaryPanelMember.HearingRoleCode.Should().Be(newHearingRoleCode);
        }
        
        [TestCase(" ")]
        [TestCase("")]
        [TestCase(null)]
        public void Should_raise_exception_if_judiciary_panel_member_display_name_is_not_specified(string displayName)
        {
            var hearing = new VideoHearingBuilder(addJudge: false)
                .WithJudiciaryPanelMember()
                .Build();
            var judiciaryPanelMember = hearing.GetJudiciaryParticipants()
                .FirstOrDefault(x => x.HearingRoleCode == JudiciaryParticipantHearingRoleCode.PanelMember);
            var newDisplayName = displayName;
            var newHearingRoleCode = JudiciaryParticipantHearingRoleCode.PanelMember;
            
            var action = () => 
                hearing.UpdateJudiciaryPanelMember(judiciaryPanelMember, newDisplayName, newHearingRoleCode);
            
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == "Display name cannot be empty").Should().BeTrue();
        }
        
        [Test]
        public void Should_raise_exception_when_updating_judiciary_panel_member_to_judge_if_judge_already_exists()
        {
            var hearing = new VideoHearingBuilder(addJudge: true)
                .WithJudiciaryPanelMember()
                .Build();
            var judiciaryPanelMember = hearing.GetJudiciaryParticipants()
                .FirstOrDefault(x => x.HearingRoleCode == JudiciaryParticipantHearingRoleCode.PanelMember);
            var newDisplayName = "New Display Name";
            var newHearingRoleCode = JudiciaryParticipantHearingRoleCode.Judge;
            
            var action = () => 
                hearing.UpdateJudiciaryPanelMember(judiciaryPanelMember, newDisplayName, newHearingRoleCode);
            
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == "A participant with Judge role already exists in the hearing").Should().BeTrue();
        }
        
        [Test]
        public void Should_raise_exception_when_updating_judiciary_panel_member_to_judge_if_judiciary_judge_already_exists()
        {
            var hearing = new VideoHearingBuilder(addJudge: false)
                .WithJudiciaryPanelMember()
                .WithJudiciaryJudge()
                .Build();
            var judiciaryPanelMember = hearing.GetJudiciaryParticipants()
                .FirstOrDefault(x => x.HearingRoleCode == JudiciaryParticipantHearingRoleCode.PanelMember);
            var newDisplayName = "New Display Name";
            var newHearingRoleCode = JudiciaryParticipantHearingRoleCode.Judge;
            
            var action = () => 
                hearing.UpdateJudiciaryPanelMember(judiciaryPanelMember, newDisplayName, newHearingRoleCode);
            
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == "A participant with Judge role already exists in the hearing").Should().BeTrue();
        }
    }
}
