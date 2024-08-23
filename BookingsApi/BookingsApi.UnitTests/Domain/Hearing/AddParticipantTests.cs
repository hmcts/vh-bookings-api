using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using BookingsApi.Domain.Validations;
using BookingsApi.UnitTests.Utilities;

namespace BookingsApi.UnitTests.Domain.Hearing
{
    public class AddParticipantTests : TestBase
    {
        [Test]
        public void Should_add_new_participant_to_hearing()
        {
            var hearing = new VideoHearingBuilder().Build();
            var applicantCaseRole = new CaseRole(1, "Applicant");
            var applicantRepresentativeHearingRole = new HearingRole(2, "Representative");

            var newPerson = new PersonBuilder(true).Build();
            var beforeAddCount = hearing.GetParticipants().Count;
            hearing.AddRepresentative(newPerson, applicantRepresentativeHearingRole, applicantCaseRole, "Display Name",
                string.Empty);

            var afterAddCount = hearing.GetParticipants().Count;

            afterAddCount.Should().BeGreaterThan(beforeAddCount);
        }
        
        [Test]
        public void Should_skip_validation_when_adding_new_participant_to_hearing_without_contact_email()
        {
            var hearing = new VideoHearingBuilder().Build();
            var applicantCaseRole = new CaseRole(1, "Applicant");
            var applicantRepresentativeHearingRole = new HearingRole(2, "Representative");

            var newPerson = new PersonBuilder(true).Build();
            newPerson.ContactEmail = null;
            var beforeAddCount = hearing.GetParticipants().Count;
            hearing.AddRepresentative(newPerson, applicantRepresentativeHearingRole, applicantCaseRole, "Display Name",
                string.Empty);
            
            hearing.AddIndividual(newPerson, applicantRepresentativeHearingRole, applicantCaseRole, "Display Name 22");

            var afterAddCount = hearing.GetParticipants().Count;

            afterAddCount.Should().Be(beforeAddCount+2);
        }

        [Test]
        public void Should_not_add_existing_participant_to_hearing()
        {
            var hearing = new VideoHearingBuilder().Build();
            var representative = (Representative)hearing.GetParticipants().First(x => x.GetType() == typeof(Representative));
            var beforeAddCount = hearing.GetParticipants().Count;

            Action action = () => hearing.AddRepresentative(representative.Person, representative.HearingRole,
                representative.CaseRole, representative.DisplayName,
                representative.Representee);
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == "Participant already exists in the hearing").Should().BeTrue();

            var afterAddCount = hearing.GetParticipants().Count;
            afterAddCount.Should().Be(beforeAddCount);
        }

        [Test]
        public void Should_add_judge_to_hearing()
        {
            var hearing = new VideoHearingBuilder(addJudge: false).Build();
            var judgeCaseRole = new CaseRole(5, "Judge");
            var judgeHearingRole = new HearingRole(13, "Judge");

            var newPerson = new PersonBuilder(true).Build();
            var beforeAddCount = hearing.GetParticipants().Count;

            hearing.AddJudge(newPerson, judgeHearingRole, judgeCaseRole, "Judge Display Name");
            var afterAddCount = hearing.GetParticipants().Count;

            afterAddCount.Should().BeGreaterThan(beforeAddCount);
        }
        
        [Test]
        public void Should_add_interpreter_to_hearing_and_turn_on_audio_recording()
        {
            var dateTime = DateTime.UtcNow.AddMinutes(25);
            var hearing = new VideoHearingBuilder(scheduledDateTime:dateTime).Build();
            hearing.AudioRecordingRequired = false;
            var interpreterCaseRole = new CaseRole(6, "Interpreter");
            var interpreterHearingRole = new HearingRole(12, "Interpreter");

            var newPerson = new PersonBuilder(true).Build();
            var beforeAddCount = hearing.GetParticipants().Count;

            hearing.AddIndividual(newPerson, interpreterHearingRole, interpreterCaseRole, "Interpreter Display Name");
            var afterAddCount = hearing.GetParticipants().Count;

            hearing.AudioRecordingRequired.Should().BeTrue();

            afterAddCount.Should().BeGreaterThan(beforeAddCount);
        }
        
        [Test]
        public void Should_add_interpreter_to_confirmed_hearing_close_to_start_time_and_turn_on_audio_recording()
        {
            var dateTime = DateTime.UtcNow.AddMinutes(25);
            var hearing = new VideoHearingBuilder(scheduledDateTime:dateTime).Build();
            hearing.AudioRecordingRequired = false;
            hearing.UpdateStatus(BookingStatus.Created, "test", null);
            var interpreterCaseRole = new CaseRole(6, "Interpreter");
            var interpreterHearingRole = new HearingRole(12, "Interpreter");

            var newPerson = new PersonBuilder(true).Build();
            var beforeAddCount = hearing.GetParticipants().Count;
            
            hearing.AddIndividual(newPerson, interpreterHearingRole, interpreterCaseRole, "Interpreter Display Name");
            var afterAddCount = hearing.GetParticipants().Count;
            
            hearing.AudioRecordingRequired.Should().BeTrue();

            afterAddCount.Should().BeGreaterThan(beforeAddCount);
        }
        
        [Test]
        public void Should_add_judicial_office_holder_to_hearing()
        {
            var hearing = new VideoHearingBuilder().Build();
            var johCaseRole = new CaseRole(7, "Judicial Office Holder");
            var johHearingRole = new HearingRole(14, "Judicial Office Holder");

            var newPerson = new PersonBuilder(true).Build();
            var beforeAddCount = hearing.GetParticipants().Count;

            hearing.AddJudicialOfficeHolder(newPerson, johHearingRole, johCaseRole, "Joh Display Name");
            var afterAddCount = hearing.GetParticipants().Count;

            afterAddCount.Should().BeGreaterThan(beforeAddCount);
        }

        [Test]
        public void Should_raise_exception_if_adding_judge_twice()
        {
            var hearingBuilder = new VideoHearingBuilder();
            var hearing = hearingBuilder.Build();
            var existingJudge = hearingBuilder.Judge;

            var judgeCaseRole = new CaseRole(5, "Judge");
            var judgeHearingRole = new HearingRole(13, "Judge");
            var newPerson = new PersonBuilder(existingJudge.Username, existingJudge.ContactEmail).Build();

            When(() => hearing.AddJudge(newPerson, judgeHearingRole, judgeCaseRole, "Judge Dredd"))
                .Should().Throw<DomainRuleException>().WithMessage("Judge with given username already exists in the hearing");

        }

        [Test]
        public void Should_raise_exception_if_judiciary_judge_already_exists()
        {
            var hearingBuilder = new VideoHearingBuilder(addJudge: false);
            var hearing = hearingBuilder.Build();
            var existingJudiciaryPerson = new JudiciaryPersonBuilder("Personal Code 1").Build();
            hearing.AddJudiciaryJudge(existingJudiciaryPerson, "Display Name 1");
            
            var judgeCaseRole = new CaseRole(5, "Judge");
            var judgeHearingRole = new HearingRole(13, "Judge");
            var newPerson = new PersonBuilder(true).Build();

            When(() => hearing.AddJudge(newPerson, judgeHearingRole, judgeCaseRole, "Judge Dredd"))
                .Should().Throw<DomainRuleException>().WithMessage("A participant with Judge role already exists in the hearing");
        }
        
        [Test]
        public void Should_raise_exception_if_adding_judicial_office_holder_twice()
        {
            var hearingBuilder = new VideoHearingBuilder();
            var hearing = hearingBuilder.Build();
            var existingJoh = hearingBuilder.JudicialOfficeHolder;

            var johCaseRole = new CaseRole(7, "Judicial Office Holder");
            var johHearingRole = new HearingRole(14, "Judicial Office Holder");
            var newPerson = new PersonBuilder(existingJoh.Username, existingJoh.ContactEmail).Build();

            When(() => hearing.AddJudicialOfficeHolder(newPerson, johHearingRole, johCaseRole, "Joh"))
                .Should().Throw<DomainRuleException>().WithMessage("Judicial office holder already exists in the hearing");

        }
    }
}