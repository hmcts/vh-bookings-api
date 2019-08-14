using System;
using Bookings.Domain;
using Bookings.Domain.Validations;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.UnitTests.Domain.Hearing
{
    public class ValidateArgumentsTests
    {
        [Test]
        public void should_throw_exception_when_validation_fails()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => 
                new VideoHearing(null, null, default(DateTime), 0, 
                    null, null, null, null, false);

            action.Should().Throw<DomainRuleException>()
                .And.ValidationFailures.Should()
                .Contain(x => x.Name == "ScheduledDuration")
                .And.Contain(x => x.Name == "ScheduledDateTime")
                .And.Contain(x => x.Name == "HearingVenue")
                .And.Contain(x => x.Name == "HearingType");
        }
    }
}