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
            var person = new JudiciaryPerson(Guid.NewGuid(), "123", "Mr", "Steve", "Allen", "Steve Allen", "nom1", "email1@email.com", true);
            person.Update("PersonalCode", "Title", "KnownAs", "Surname", "FullName", "PostNominals", "Email", true);

            person.PersonalCode.Should().Be("PersonalCode");
            person.Title.Should().Be("Title");
            person.KnownAs.Should().Be("KnownAs");
            person.Surname.Should().Be("Surname");
            person.Fullname.Should().Be("FullName");
            person.PostNominals.Should().Be("PostNominals");
            person.Email.Should().Be("Email");
            person.HasLeft.Should().BeTrue();
        }
    }
}