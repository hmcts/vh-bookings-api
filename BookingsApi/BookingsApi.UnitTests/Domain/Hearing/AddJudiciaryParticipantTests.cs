using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
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
            const string displayName = "Display Name";

            hearing.AddJudiciaryJudge(newJudiciaryPerson, displayName);

            var judiciaryParticipants = hearing.GetJudiciaryParticipants();
            var afterAddCount = judiciaryParticipants.Count;
            afterAddCount.Should().BeGreaterThan(beforeAddCount);
            judiciaryParticipants.Should().Contain(x => x.DisplayName == "Display Name");
        }
        
        [Test]
        public void Should_raise_exception_when_adding_a_judge_and_a_judiciary_person_already_exists()
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
        public void Should_raise_exception_when_adding_a_judiciary_person_already_exists()
        {
            var hearing = new VideoHearingBuilder(addJudge: false).Build();
            var existingJudiciaryPerson = new JudiciaryPersonBuilder("Personal Code 1").Build();
            hearing.AddJudiciaryJudge(existingJudiciaryPerson, "Display Name 1");
            var beforeAddCount = hearing.GetJudiciaryParticipants().Count;

            var action = () => 
                hearing.AddJudiciaryJudge(existingJudiciaryPerson, "Display Name 2");
            
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == DomainRuleErrorMessages.JudiciaryPersonAlreadyExists(existingJudiciaryPerson.PersonalCode)).Should().BeTrue();
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
        
        [Test]
        public void Should_add_new_generic_judiciary_judge_to_hearing()
        {
            var hearing = new VideoHearingBuilder(addJudge: false).Build();
            var newJudiciaryPerson = new JudiciaryPersonBuilder(isGeneric: true).Build();
            var beforeAddCount = hearing.GetJudiciaryParticipants().Count;
            const string displayName = "Generic Judge Display Name";
            const string contactTelephone = "0123456789";
            const string contactEmail = "email@email.com";

            hearing.AddJudiciaryJudge(newJudiciaryPerson, displayName, contactTelephone: contactTelephone, contactEmail: contactEmail);

            var judiciaryParticipants = hearing.GetJudiciaryParticipants();
            var afterAddCount = judiciaryParticipants.Count;
            afterAddCount.Should().BeGreaterThan(beforeAddCount);
            judiciaryParticipants.Should().Contain(x => 
                x.DisplayName == displayName && 
                x.ContactTelephone == contactTelephone && 
                x.ContactEmail == contactEmail);
        }
        
        [Test]
        public void Should_add_new_generic_judiciary_panel_member_to_hearing()
        {
            var hearing = new VideoHearingBuilder(addJudge: false).Build();
            var newJudiciaryPerson = new JudiciaryPersonBuilder(isGeneric: true).Build();
            var beforeAddCount = hearing.GetJudiciaryParticipants().Count;
            const string displayName = "Generic Panel Member Display Name";
            const string contactTelephone = "0123456789";
            const string contactEmail = "email@email.com";

            hearing.AddJudiciaryPanelMember(newJudiciaryPerson, displayName, contactTelephone: contactTelephone, contactEmail: contactEmail);
            
            var judiciaryParticipants = hearing.GetJudiciaryParticipants();
            var afterAddCount = judiciaryParticipants.Count;
            afterAddCount.Should().BeGreaterThan(beforeAddCount);
            judiciaryParticipants.Should().Contain(x => 
                x.DisplayName == displayName && 
                x.ContactTelephone == contactTelephone && 
                x.ContactEmail == contactEmail);
        }

        [TestCase(JudiciaryParticipantHearingRoleCode.Judge)]
        [TestCase(JudiciaryParticipantHearingRoleCode.PanelMember)]
        public void Should_raise_exception_if_contact_telephone_specified_and_judiciary_person_is_not_generic(JudiciaryParticipantHearingRoleCode hearingRoleCode)
        {
            var hearing = new VideoHearingBuilder(addJudge: false).Build();
            var newJudiciaryPerson = new JudiciaryPersonBuilder(isGeneric: false).Build();
            var beforeAddCount = hearing.GetJudiciaryParticipants().Count;

            Func<JudiciaryParticipant> action;
            if (hearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge)
            {
                action = () => 
                    hearing.AddJudiciaryJudge(newJudiciaryPerson, "Display Name", contactTelephone: "0123456789");
            }
            else
            {
                action = () => 
                    hearing.AddJudiciaryPanelMember(newJudiciaryPerson, "Display Name", contactTelephone: "0123456789");
            }

            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == DomainRuleErrorMessages.ContactTelephoneForNonGenericJudiciaryPerson).Should().BeTrue();
            var afterAddCount = hearing.GetJudiciaryParticipants().Count;
            afterAddCount.Should().Be(beforeAddCount);
        }
        
        [TestCase(JudiciaryParticipantHearingRoleCode.Judge)]
        [TestCase(JudiciaryParticipantHearingRoleCode.PanelMember)]
        public void Should_raise_exception_if_contact_email_specified_and_judiciary_person_is_not_generic(JudiciaryParticipantHearingRoleCode hearingRoleCode)
        {
            var hearing = new VideoHearingBuilder(addJudge: false).Build();
            var newJudiciaryPerson = new JudiciaryPersonBuilder(isGeneric: false).Build();
            var beforeAddCount = hearing.GetJudiciaryParticipants().Count;
            
            Func<JudiciaryParticipant> action;
            if (hearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge)
            {
                action = () => 
                    hearing.AddJudiciaryJudge(newJudiciaryPerson, "Display Name", contactEmail: "email@email.com");
            }
            else
            {
                action = () => 
                    hearing.AddJudiciaryPanelMember(newJudiciaryPerson, "Display Name", contactEmail: "email@email.com");
            }
            
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == DomainRuleErrorMessages.ContactEmailForNonGenericJudiciaryPerson).Should().BeTrue();
            var afterAddCount = hearing.GetJudiciaryParticipants().Count;
            afterAddCount.Should().Be(beforeAddCount);
        }
    }
}
