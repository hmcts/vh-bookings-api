using System.Linq;
using BookingsApi.Domain;
using BookingsApi.Domain.Validations;

namespace BookingsApi.UnitTests.Domain.Persons
{
    public class UpdatePersonTests
    {
        [Test]
        public void should_update_person()
        {
            var person = new Person("Mr", "John", "Doe", "john.doe@hmcts.net", "john.doe@hmcts.net");
            person.UpdatePerson("New", "Me", "new.me@hmcts.net");

            person.FirstName.Should().Be("New");
            person.LastName.Should().Be("Me");
            person.Username.Should().Be("new.me@hmcts.net");
        }
        
        [Test]
        public void should_throw_exception_when_updating_a_person_with_no_first_name()
        {
            var person = new Person("Mr", "John", "Doe", "john.doe@hmcts.net", "john.doe@hmcts.net");
            var exception = Assert.Throws<DomainRuleException>(() => person.UpdatePerson(null, "Me", "me@me.com", "new.me@hmcts.net"));
            exception.ValidationFailures.Any(x => x.Name == nameof(person.FirstName)).Should().BeTrue();
        }
        
        [Test]
        public void should_throw_exception_when_updating_a_person_with_no_last_name()
        {
            var person = new Person("Mr", "John", "Doe", "john.doe@hmcts.net", "john.doe@hmcts.net");
            var exception = Assert.Throws<DomainRuleException>(() => person.UpdatePerson("New", null, "me@me.com", "new.me@hmcts.net"));
            exception.ValidationFailures.Any(x => x.Name == nameof(person.LastName)).Should().BeTrue();
        }
    }
}