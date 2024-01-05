using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;

namespace BookingsApi.UnitTests.Domain.Participants
{
    public class PersonAlreadyExistTests
    {
        private Participant _individual;

        [SetUp]
        public void SetUp()
        {
            _individual = new ParticipantBuilder().IndividualParticipantApplicant;
        }
        
        [Test]
        public void Should_return_false_when_new_person_added_as_participant()
        {
            _individual.DoesPersonAlreadyExist().Should().BeFalse();
        }

        [Test]
        public void Should_return_true_when_existing_person_added_as_participant()
        {
            _individual.Person.GetType().GetProperty("CreatedDate").SetValue(_individual.Person,
                _individual.Person.CreatedDate.AddDays(-10), null);

            _individual.DoesPersonAlreadyExist().Should().BeTrue();
        }

        [Test]
        public void Should_return_true_when_person_is_null_as_participant_could_be_judiciary()
        {
            _individual.GetType().GetProperty("Person").SetValue(_individual, null, null);

            _individual.DoesPersonAlreadyExist().Should().BeTrue();
        }
    }
}