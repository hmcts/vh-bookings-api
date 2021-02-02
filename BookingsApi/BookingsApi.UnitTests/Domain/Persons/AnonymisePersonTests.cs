using Bookings.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Domain.Persons
{
    public class AnonymisePersonTests
    {
        [Test]
        public void should_anonymise_person()
        {
            var personOriginal = new Person("Mr", "John", "Doe", "john.doe@hearings.reform.hmcts.net");
            var person = new Person("Mr", "John", "Doe", "john.doe@hearings.reform.hmcts.net");
            person.AnonymisePerson();
            person.FirstName.Should().NotBe(personOriginal.FirstName);
            person.LastName.Should().NotBe(personOriginal.LastName);
            person.Username.Should().NotBe(personOriginal.Username);
            person.ContactEmail.Should().NotBe(personOriginal.ContactEmail);
        }
    }
}