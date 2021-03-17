using System;
using BookingsApi.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Domain.JudiciaryPersons
{
    public class UpdateJudiciaryPersonTests
    {
        [Test]
        public void should_update_person()
        {
            var person = new JudiciaryPerson(Guid.NewGuid(), "123", "Mr", "Steve", "Allen", "Steve Allen", "nom1", "email1@email.com");
            person.Update("changed", "changed", "changed", "changed", "changed", "changed", "changed");

            person.PersonalCode.Should().Be("changed");
            person.Title.Should().Be("changed");
            person.KnownAs.Should().Be("changed");
            person.Surname.Should().Be("changed");
            person.Fullname.Should().Be("changed");
            person.PostNominals.Should().Be("changed");
            person.Email.Should().Be("changed");
        }
    }
}