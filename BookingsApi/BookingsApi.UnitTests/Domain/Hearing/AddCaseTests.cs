using System;
using System.Linq;
using Bookings.Domain.Enumerations;
using Bookings.Domain.Validations;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace BookingsApi.UnitTests.Domain.Hearing
{
    public class AddCaseTests
    {
        [Test]
        public void Should_add_new_case()
        {
            var hearing = new VideoHearingBuilder().Build();
            var beforeAddCount = hearing.GetCases().Count;
            hearing.AddCase("0875", "Test Case Add",false);
            var afterAddCount = hearing.GetCases().Count;
            afterAddCount.Should().BeGreaterThan(beforeAddCount);
        }
        
        [Test]
        public void Should_not_add_existing_case()
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

    public class CancelHearingTests
    {
        [Test]
        public void Should_update_status_on_cancel()
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.Status.Should().Be(BookingStatus.Booked);
            
            hearing.CancelHearing();
            
            hearing.Status.Should().Be(BookingStatus.Cancelled);
        }
    }
}