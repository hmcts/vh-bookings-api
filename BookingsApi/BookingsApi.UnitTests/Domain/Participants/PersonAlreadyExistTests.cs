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
            // new participants/persons will have username equal their contact email
            // use set protected to avoid updating timestamps like the domain method
            _individual.Person.SetProtected(nameof(_individual.Person.Username), _individual.Person.ContactEmail);
            _individual.DoesPersonAlreadyExist().Should().BeFalse();
        }
        
        [Test]
        public void should_return_true_when_new_person_added_but_has_been_updated_since()
        {
            var historicalDate = DateTime.UtcNow.AddDays(-10);
            _individual.SetProtected(nameof(_individual.CreatedDate), historicalDate);
            _individual.Person.SetProtected(nameof(_individual.Person.CreatedDate), historicalDate);
            _individual.Person.SetProtected(nameof(_individual.Person.UpdatedDate), historicalDate);
            _individual.Person.SetProtected(nameof(_individual.Person.Username), _individual.Person.ContactEmail);
            
            _individual.DoesPersonAlreadyExist().Should().BeFalse();
            
            // when a person's username is created and assiged, they are no longer a new person
            _individual.Person.UpdateUsername("new_user@test.com");
            _individual.DoesPersonAlreadyExist().Should().BeTrue();
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