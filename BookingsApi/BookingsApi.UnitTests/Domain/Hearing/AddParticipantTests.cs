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
            var applicantRepresentativeHearingRole = new HearingRole(2, "Representative");

            var newPerson = new PersonBuilder(true).Build();
            var beforeAddCount = hearing.GetParticipants().Count;
            hearing.AddRepresentative(Guid.NewGuid().ToString(), newPerson, applicantRepresentativeHearingRole,
                "Display Name", string.Empty);

            var afterAddCount = hearing.GetParticipants().Count;

            afterAddCount.Should().BeGreaterThan(beforeAddCount);
        }
        
        [Test]
        public void Should_skip_validation_when_adding_new_participant_to_hearing_without_contact_email()
        {
            var hearing = new VideoHearingBuilder().Build();
            var applicantRepresentativeHearingRole = new HearingRole(2, "Representative");

            var newPerson = new PersonBuilder(true).Build();
            newPerson.ContactEmail = null;
            var beforeAddCount = hearing.GetParticipants().Count;
            hearing.AddRepresentative(Guid.NewGuid().ToString(), newPerson, applicantRepresentativeHearingRole,
                "Display Name", string.Empty);

            hearing.AddIndividual(Guid.NewGuid().ToString(), newPerson, applicantRepresentativeHearingRole,
                "Display Name 22");

            var afterAddCount = hearing.GetParticipants().Count;

            afterAddCount.Should().Be(beforeAddCount+2);
        }

        [Test]
        public void Should_not_add_existing_participant_to_hearing()
        {
            var hearing = new VideoHearingBuilder().Build();
            var representative = (Representative)hearing.GetParticipants().First(x => x.GetType() == typeof(Representative));
            var beforeAddCount = hearing.GetParticipants().Count;

            Action action = () => hearing.AddRepresentative(Guid.NewGuid().ToString(), representative.Person,
                representative.HearingRole, representative.DisplayName, representative.Representee);
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == $"Participant {representative.Person.ContactEmail} already exists in the hearing").Should().BeTrue();

            var afterAddCount = hearing.GetParticipants().Count;
            afterAddCount.Should().Be(beforeAddCount);
        }
        
        [Test]
        public void Should_add_interpreter_to_hearing_and_turn_on_audio_recording()
        {
            var dateTime = DateTime.UtcNow.AddMinutes(25);
            var hearing = new VideoHearingBuilder(scheduledDateTime:dateTime).Build();
            hearing.AudioRecordingRequired = false;
            var interpreterHearingRole = new HearingRole(12, "Interpreter");
            hearing.CaseType.IsAudioRecordingAllowed = true;

            var newPerson = new PersonBuilder(true).Build();
            var beforeAddCount = hearing.GetParticipants().Count;

            hearing.AddIndividual(Guid.NewGuid().ToString(), newPerson, interpreterHearingRole, "Interpreter Display Name");
            var afterAddCount = hearing.GetParticipants().Count;

            hearing.AudioRecordingRequired.Should().BeTrue();

            afterAddCount.Should().BeGreaterThan(beforeAddCount);
        }

        [Test]
        public void Should_add_interpreter_to_hearing_and_not_turn_on_audio_recording_when_audio_recording_is_not_allowed_for_case_type()
        {
            var dateTime = DateTime.UtcNow.AddMinutes(25);
            var hearing = new VideoHearingBuilder(scheduledDateTime:dateTime).Build();
            hearing.AudioRecordingRequired = false;
            var interpreterHearingRole = new HearingRole(12, "Interpreter");
            hearing.CaseType.IsAudioRecordingAllowed = false;

            var newPerson = new PersonBuilder(true).Build();
            var beforeAddCount = hearing.GetParticipants().Count;

            hearing.AddIndividual(Guid.NewGuid().ToString(), newPerson, interpreterHearingRole, "Interpreter Display Name");
            var afterAddCount = hearing.GetParticipants().Count;

            hearing.AudioRecordingRequired.Should().BeFalse();

            afterAddCount.Should().BeGreaterThan(beforeAddCount);
        }
        
        [Test]
        public void Should_add_interpreter_to_confirmed_hearing_close_to_start_time_and_turn_on_audio_recording()
        {
            var dateTime = DateTime.UtcNow.AddMinutes(25);
            var hearing = new VideoHearingBuilder(scheduledDateTime:dateTime).Build();
            hearing.AudioRecordingRequired = false;
            hearing.UpdateStatus(BookingStatus.Created, "test", null);
            
            var interpreterHearingRole = new HearingRole(12, "Interpreter");

            var newPerson = new PersonBuilder(true).Build();
            var beforeAddCount = hearing.GetParticipants().Count;
            
            hearing.AddIndividual(Guid.NewGuid().ToString(), newPerson, interpreterHearingRole, "Interpreter Display Name");
            var afterAddCount = hearing.GetParticipants().Count;
            
            hearing.AudioRecordingRequired.Should().BeTrue();

            afterAddCount.Should().BeGreaterThan(beforeAddCount);
        }
    }
}