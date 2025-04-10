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
                .First(x => x.HearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge);
            var personalCode = judiciaryJudge.JudiciaryPerson.PersonalCode;
            var newDisplayName = "New Display Name";

            hearing.UpdateJudiciaryParticipantByPersonalCode(personalCode, newDisplayName,
                JudiciaryParticipantHearingRoleCode.Judge, null, null);

            var updatedJudiciaryJudge = hearing.GetJudiciaryParticipants()
                .First(x => x.JudiciaryPerson.PersonalCode == judiciaryJudge.JudiciaryPerson.PersonalCode);
            updatedJudiciaryJudge.DisplayName.Should().Be(newDisplayName);
            updatedJudiciaryJudge.HearingRoleCode.Should().Be(JudiciaryParticipantHearingRoleCode.Judge);
        }

        [Test]
        public void should_update_panel_member_when_no_host_is_set()
        {
            var hearing = new VideoHearingBuilder(addJudge: false)
                .WithJudiciaryPanelMember()
                .Build();
            hearing
                .UpdateBookingStatusJudgeRequirement(); // need to find a better home for this. shouldn't have to be after instantiating

            var judiciaryPanelMember = hearing.GetJudiciaryParticipants()
                .First(x => x.HearingRoleCode == JudiciaryParticipantHearingRoleCode.PanelMember);

            hearing.UpdateJudiciaryParticipantByPersonalCode(judiciaryPanelMember.JudiciaryPerson.PersonalCode,
                "New Display Name", JudiciaryParticipantHearingRoleCode.PanelMember, null, null);

            hearing.JudiciaryParticipants
                .First(x => x.JudiciaryPerson.PersonalCode == judiciaryPanelMember.JudiciaryPerson.PersonalCode)
                .DisplayName.Should().Be("New Display Name");
        }

        [Test]
        public void should_not_update_participant_when_no_changes_made()
        {
            var hearing = new VideoHearingBuilder(addJudge: false)
                .WithJudiciaryJudge()
                .Build();
            var participant = hearing.GetJudiciaryParticipants()
                .First(x => x.HearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge);
            var updatedDateBefore = hearing.UpdatedDate;

            hearing.UpdateJudiciaryParticipantByPersonalCode(participant.JudiciaryPerson.PersonalCode,
                participant.DisplayName, participant.HearingRoleCode, null, null);

            var updatedDateAfter = hearing.UpdatedDate;
            updatedDateAfter.Should().Be(updatedDateBefore);
        }

        [Test]
        public void Should_raise_exception_when_updating_judiciary_judge_to_panel_member_if_no_other_host_exists()
        {
            var hearing = new VideoHearingBuilder(addJudge: false)
                .WithJudiciaryJudge()
                .Build();
            var judiciaryJudge = hearing.GetJudiciaryParticipants()
                .First(x => x.HearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge);
            var personalCode = judiciaryJudge.JudiciaryPerson.PersonalCode;
            var newDisplayName = "New Display Name";
            var newHearingRoleCode = JudiciaryParticipantHearingRoleCode.PanelMember;

            var action = () =>
                hearing.UpdateJudiciaryParticipantByPersonalCode(personalCode, newDisplayName, newHearingRoleCode, null,
                    null);

            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == DomainRuleErrorMessages.HearingNeedsAHost).Should().BeTrue();
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
                .First(x => x.HearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge);
            var personalCode = judiciaryJudge.JudiciaryPerson.PersonalCode;
            var newDisplayName = displayName;
            var newHearingRoleCode = JudiciaryParticipantHearingRoleCode.PanelMember;

            var action = () =>
                hearing.UpdateJudiciaryParticipantByPersonalCode(personalCode, newDisplayName, newHearingRoleCode, null, null);

            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == "Display name cannot be empty").Should().BeTrue();
        }

        [TestCase(JudiciaryParticipantHearingRoleCode.Judge)]
        [TestCase(JudiciaryParticipantHearingRoleCode.PanelMember)]
        public void Should_update_judiciary_panel_member_in_hearing(
            JudiciaryParticipantHearingRoleCode newHearingRoleCode)
        {
            var hearing = new VideoHearingBuilder(addJudge: false)
                .WithJudiciaryPanelMember()
                .Build();
            if (newHearingRoleCode == JudiciaryParticipantHearingRoleCode.PanelMember)
                hearing.UpdateStatus(BookingStatus.BookedWithoutJudge, "System", "reason");
            var judiciaryPanelMember = hearing.GetJudiciaryParticipants()
                .First(x => x.HearingRoleCode == JudiciaryParticipantHearingRoleCode.PanelMember);
            var personalCode = judiciaryPanelMember.JudiciaryPerson.PersonalCode;
            var newDisplayName = "New Display Name";

            hearing.UpdateJudiciaryParticipantByPersonalCode(personalCode, newDisplayName, newHearingRoleCode, null, null);

            var updatedJudiciaryPanelMember = hearing.GetJudiciaryParticipants()
                .First(
                    x => x.JudiciaryPerson.PersonalCode == judiciaryPanelMember.JudiciaryPerson.PersonalCode);
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
                .First(x => x.HearingRoleCode == JudiciaryParticipantHearingRoleCode.PanelMember);
            var personalCode = judiciaryPanelMember.JudiciaryPerson.PersonalCode;
            var newDisplayName = displayName;
            var newHearingRoleCode = JudiciaryParticipantHearingRoleCode.PanelMember;

            var action = () =>
                hearing.UpdateJudiciaryParticipantByPersonalCode(personalCode, newDisplayName, newHearingRoleCode, null, null);

            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == "Display name cannot be empty").Should().BeTrue();
        }

        [Test]
        public void Should_raise_exception_if_judiciary_participant_does_not_exist()
        {
            var hearing = new VideoHearingBuilder(addJudge: false)
                .Build();
            var personalCode = "Non Existing Personal Code";
            var newDisplayName = "New Display Name";
            var newHearingRoleCode = JudiciaryParticipantHearingRoleCode.PanelMember;

            var action = () =>
                hearing.UpdateJudiciaryParticipantByPersonalCode(personalCode, newDisplayName, newHearingRoleCode, null, null);

            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == DomainRuleErrorMessages.JudiciaryParticipantNotFound).Should().BeTrue();
        }

        [Test]
        public void Should_raise_exception_when_updating_judiciary_panel_member_to_judge_if_judge_already_exists()
        {
            var hearing = new VideoHearingBuilder(addJudge: true)
                .WithJudiciaryPanelMember()
                .Build();
            var judiciaryPanelMember = hearing.GetJudiciaryParticipants()
                .First(x => x.HearingRoleCode == JudiciaryParticipantHearingRoleCode.PanelMember);
            var personalCode = judiciaryPanelMember.JudiciaryPerson.PersonalCode;
            var newDisplayName = "New Display Name";
            var newHearingRoleCode = JudiciaryParticipantHearingRoleCode.Judge;

            var action = () =>
                hearing.UpdateJudiciaryParticipantByPersonalCode(personalCode, newDisplayName, newHearingRoleCode, null, null);

            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == "A participant with Judge role already exists in the hearing").Should()
                .BeTrue();
        }

        [Test]
        public void
            Should_raise_exception_when_updating_judiciary_panel_member_to_judge_if_judiciary_judge_already_exists()
        {
            var hearing = new VideoHearingBuilder(addJudge: false)
                .WithJudiciaryPanelMember()
                .WithJudiciaryJudge()
                .Build();
            var judiciaryPanelMember = hearing.GetJudiciaryParticipants()
                .First(x => x.HearingRoleCode == JudiciaryParticipantHearingRoleCode.PanelMember);
            var personalCode = judiciaryPanelMember.JudiciaryPerson.PersonalCode;
            var newDisplayName = "New Display Name";
            var newHearingRoleCode = JudiciaryParticipantHearingRoleCode.Judge;

            var action = () =>
                hearing.UpdateJudiciaryParticipantByPersonalCode(personalCode, newDisplayName, newHearingRoleCode, null, null);

            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == "A participant with Judge role already exists in the hearing").Should()
                .BeTrue();
        }
    }
}
