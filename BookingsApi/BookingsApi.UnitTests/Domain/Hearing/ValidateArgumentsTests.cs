using System;
using BookingsApi.Domain;
using BookingsApi.Domain.Validations;

namespace BookingsApi.UnitTests.Domain.Hearing
{
    public class ValidateArgumentsTests
    {
        [Test]
        public void Should_throw_exception_when_validation_fails()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => 
                new VideoHearing(null, null, default(DateTime), 0, 
                    null, null, null, null, false, true, null);

            action.Should().Throw<DomainRuleException>()
                .And.ValidationFailures.Should()
                .Contain(x => x.Name == "ScheduledDuration")
                .And.Contain(x => x.Name == "ScheduledDateTime")
                .And.Contain(x => x.Name == "HearingVenue")
                .And.Contain(x => x.Name == "HearingType");
        }
    }
}