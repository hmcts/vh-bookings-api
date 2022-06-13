using System;
using BookingsApi.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Domain.JudiciaryPersons
{
    public class UpdateJudiciaryPersonTests
    {
        [Test]
        public void Should_update_person()
        {
            var person = new JudiciaryPerson(Guid.NewGuid(), "123", "Mr", "Steve", "Allen", "Steve Allen", "nom1", "email1@email.com", true, false, string.Empty);
            person.Update("PersonalCode", "Title", "KnownAs", "Surname", "FullName", "PostNominals", "Email", true, true, "2022-06-08");

            person.PersonalCode.Should().Be("PersonalCode");
            person.Title.Should().Be("Title");
            person.KnownAs.Should().Be("KnownAs");
            person.Surname.Should().Be("Surname");
            person.Fullname.Should().Be("FullName");
            person.PostNominals.Should().Be("PostNominals");
            person.Email.Should().Be("Email");
            person.HasLeft.Should().BeTrue();
            person.Leaver.Should().BeTrue();
            person.LeftOn.Should().Be("2022-06-08");
        }

        [Test]
        public void Should_update_the_leaver_person()
        {
            var person = new JudiciaryPerson(Guid.NewGuid(), "123", "Mr", "Steve", "Allen", "Steve Allen", "nom1", "email1@email.com", false, false, string.Empty);
            person.Update(true);
            
            person.HasLeft.Should().BeTrue();
            person.Fullname.Should().BeNull();
        }
        
        [Test]
        public void Should_set_personal_data_to_null_for_leaver_accounts()
        {
            var person = new JudiciaryPerson(Guid.NewGuid(), "123", "Mr", "Steve", "Allen", "Steve Allen", "nom1", "email1@email.com", false, true, "1-1-2020");
            person.Update(true);

            person.HasLeft.Should().BeTrue();
            person.Fullname.Should().BeNull();
            person.Title.Should().BeNull();
            person.KnownAs.Should().BeNull();
            person.Surname.Should().BeNull();
            person.Email.Should().BeNull();
            person.PostNominals.Should().BeNull();
        }
        
        [Test]
        public void Should_not_set_personal_data_to_null_for_leaver_accounts()
        {
            var person = new JudiciaryPerson(Guid.NewGuid(), "123", "Mr", "Steve", "Allen", "Steve Allen", "nom1", "email1@email.com", false, true, "1-1-2020");
            person.Update(false);

            person.HasLeft.Should().BeFalse();
            person.Fullname.Should().NotBeNull();
            person.Title.Should().NotBeNull();
            person.KnownAs.Should().NotBeNull();
            person.Surname.Should().NotBeNull();
            person.Email.Should().NotBeNull();
            person.PostNominals.Should().NotBeNull();
        }
    }
}