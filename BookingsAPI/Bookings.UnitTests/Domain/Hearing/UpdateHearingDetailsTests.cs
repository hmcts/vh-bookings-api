using System;
using System.Linq;
using Bookings.Domain;
using Bookings.Domain.Validations;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace Bookings.UnitTests.Domain.Hearing
{
    public class UpdateHearingDetailsTests
    {
        [Test]
        public void should_update_hearing_details()
        {
            var hearing = new VideoHearingBuilder().Build();
            var beforeUpdatedDate = hearing.UpdatedDate;
            var newVenue = new RefDataBuilder().HearingVenues.Last();
            var newDateTime = DateTime.Today.AddDays(10).AddHours(14);
            var newDuration = 150; 
            hearing.UpdateHearingDetails(newVenue, newDateTime, newDuration);

            hearing.UpdatedDate.Should().BeAfter(beforeUpdatedDate);
        }
        
        [Test]
        public void should_throw_exception_when_validation_fails()
        {
            var hearing = new VideoHearingBuilder().Build();
            var beforeUpdatedDate = hearing.UpdatedDate;
            HearingVenue newVenue = null;
            var newDateTime = DateTime.Today.AddDays(-10);
            var newDuration = -10; 
            
            Action action = () => hearing.UpdateHearingDetails(newVenue, newDateTime, newDuration);
            action.Should().Throw<DomainRuleException>()
                .And.ValidationFailures.Should()
                .Contain(x => x.Name == "ScheduledDuration")
                .And.Contain(x => x.Name == "ScheduledDateTime")
                .And.Contain(x => x.Name == "Venue");
            
            hearing.UpdatedDate.Should().Be(beforeUpdatedDate);
        }
    }
}