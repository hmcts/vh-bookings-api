using System.Linq;
using BookingsApi.Domain;
using BookingsApi.Domain.Validations;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Domain.Persons
{
    public class UpdatePersonTests
    {
        [Test]
        public void should_update_person()
        {
            var person = new Person("Mr", "John", "Doe", "john.doe@hearings.reform.hmcts.net");
            person.UpdatePerson("New", "Me", "new.me@test.com");

            person.FirstName.Should().Be("New");
            person.LastName.Should().Be("Me");
            person.Username.Should().Be("new.me@test.com");
        }
        
        [Test]
        public void should_throw_exception_when_updating_a_person_with_no_first_name()
        {
            var person = new Person("Mr", "John", "Doe", "john.doe@hearings.reform.hmcts.net");
            var exception = Assert.Throws<DomainRuleException>(() => person.UpdatePerson(null, "Me", "new.me@test.com"));
            exception.ValidationFailures.Any(x => x.Name == nameof(person.FirstName)).Should().BeTrue();
        }
        
        [Test]
        public void should_throw_exception_when_updating_a_person_with_no_last_name()
        {
            var person = new Person("Mr", "John", "Doe", "john.doe@hearings.reform.hmcts.net");
            var exception = Assert.Throws<DomainRuleException>(() => person.UpdatePerson("New", null, "new.me@test.com"));
            exception.ValidationFailures.Any(x => x.Name == nameof(person.LastName)).Should().BeTrue();
        }
        
        [Test]
        public void should_throw_exception_when_updating_a_person_with_no_username()
        {
            var person = new Person("Mr", "John", "Doe", "john.doe@hearings.reform.hmcts.net");
            var exception = Assert.Throws<DomainRuleException>(() => person.UpdatePerson("New", "Me", null));
            exception.ValidationFailures.Any(x => x.Name == nameof(person.Username)).Should().BeTrue();
        }
    }
}