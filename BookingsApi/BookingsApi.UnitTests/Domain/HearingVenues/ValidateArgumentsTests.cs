using System;
using BookingsApi.Domain;
using BookingsApi.Domain.Validations;

namespace BookingsApi.UnitTests.Domain.HearingVenues
{
    public class ValidateArgumentsTests
    {
        [Test]
        public void Should_throw_exception_when_validation_fails()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new HearingVenue(-1, null);
            action.Should().Throw<DomainRuleException>()
                .And.ValidationFailures.Should()
                .Contain(x => x.Name == "Id")
                .And.Contain(x => x.Name == "Name");
        }
    }
}