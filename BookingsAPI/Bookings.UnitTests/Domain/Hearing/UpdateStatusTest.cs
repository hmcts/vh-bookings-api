using Bookings.Domain.Enumerations;
using Bookings.Domain.Validations;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;
using Testing.Common.Builders.Domain;

namespace Bookings.UnitTests.Domain.Hearing
{
    public class UpdateStatusTest
    {
        [Test]
        public void should_update_hearing_status()
        {
            var hearing = new VideoHearingBuilder().Build();
            var updatedDate = hearing.UpdatedDate;
            hearing.UpdateStatus(BookingStatus.Cancelled, "testuser");
            hearing.UpdatedDate.Should().BeAfter(updatedDate);
            hearing.Status.Should().Be(BookingStatus.Cancelled);
        }

        [Test]
        public void should_throw_domain_exception_Upon_hearing_status_update_to_booked()
        {
            var hearing = new VideoHearingBuilder().Build();
            var updatedDate = hearing.UpdatedDate;
            var newStatus = BookingStatus.Booked;
            Action action = () => hearing.UpdateStatus(newStatus, "testuser");
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Any(x => x.Message == $"Cannot change the booking status from {hearing.Status} to {newStatus}").Should().BeTrue();
            hearing.Status.Should().Be(BookingStatus.Booked);
            hearing.UpdatedDate.Should().Be(updatedDate);
        }

        [Test]
        public void should_throw_domain_exception_Upon_hearing_status_update_to_created()
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.UpdateStatus(BookingStatus.Cancelled, "testuser");

            var updatedDate = hearing.UpdatedDate;
            var newStatus = BookingStatus.Booked;
            Action action = () => hearing.UpdateStatus(newStatus, "testuser");
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Any(x => x.Message == $"Cannot change the booking status from {hearing.Status} to {newStatus}").Should().BeTrue();
            hearing.Status.Should().Be(BookingStatus.Cancelled);
            hearing.UpdatedDate.Should().Be(updatedDate);
        }

        [Test]
        public void should_throw_argument_null_exception_when_updatedby_field_is_empty()
        {
            var hearing = new VideoHearingBuilder().Build();
            
            var newStatus = BookingStatus.Cancelled;
            Action action = () => hearing.UpdateStatus(newStatus, string.Empty);
            action.Should().Throw<ArgumentNullException>();
            hearing.Status.Should().Be(BookingStatus.Booked);
        }

        [Test]
        public void should_throw_argument_null_exception_when_updatedby_field_is_null()
        {
            var hearing = new VideoHearingBuilder().Build();

            var newStatus = BookingStatus.Cancelled;
            Action action = () => hearing.UpdateStatus(newStatus, null);
            action.Should().Throw<ArgumentNullException>();
            hearing.Status.Should().Be(BookingStatus.Booked);
        }

    }
}
