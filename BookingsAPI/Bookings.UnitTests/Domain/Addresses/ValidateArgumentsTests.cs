using System;
using Bookings.Domain;
using Bookings.Domain.Validations;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.UnitTests.Domain.Addresses
{
    public class ValidateArgumentsTests
    {
        [Test]
        public void should_throw_exception_when_validation_fails()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new Address(null, null, null, null,null);

            action.Should().Throw<DomainRuleException>()
                .And.ValidationFailures.Should()
                .Contain(x => x.Name == "HouseNumber")
                .And.Contain(x => x.Name == "Street")
                .And.Contain(x => x.Name == "City")
                .And.Contain(x => x.Name == "Postcode")
                .And.Contain(x => x.Name == "County");
        }
    }
}