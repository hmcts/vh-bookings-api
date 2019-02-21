using System;
using System.Linq;
using Bookings.Domain.Validations;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace Bookings.UnitTests.Domain.Hearing
{
    public class AddCaseTests
    {
        [Test]
        public void should_add_new_case()
        {
            var hearing = new VideoHearingBuilder().Build();
            var beforeAddCount = hearing.GetCases().Count;
            hearing.AddCase("0875", "Test Case Add",false);
            var afterAddCount = hearing.GetCases().Count;
            afterAddCount.Should().BeGreaterThan(beforeAddCount);
        }
        
        [Test]
        public void should_not_add_existing_case()
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.AddCase("0875", "Test Case Add",false);
            
            var beforeAddCount = hearing.GetCases().Count;
            
            Action action = () => hearing.AddCase("0875", "Test Case Add",false);
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Any(x => x.Message == "Case already exists for the hearing").Should().BeTrue();
            
            var afterAddCount = hearing.GetCases().Count;
            afterAddCount.Should().Be(beforeAddCount);
        }
    }
}