using System;
using BookingsApi.Domain;
using BookingsApi.Domain.Validations;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Domain.Persons
{
    public class ValidateArgumentsTests
    {
        [Test]
        public void Should_throw_exception_when_validation_fails()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new Person(null, null, null, null);

            action.Should().Throw<DomainRuleException>()
                .And.ValidationFailures.Should()
                .Contain(x => x.Name == "FirstName")
                .And.Contain(x => x.Name == "LastName")
                .And.Contain(x => x.Name == "Username");
        }
    }
}