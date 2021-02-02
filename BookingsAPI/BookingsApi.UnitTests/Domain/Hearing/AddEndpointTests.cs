using System;
using Bookings.Domain.Validations;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace BookingsApi.UnitTests.Domain.Hearing
{
    public class AddEndpointTests
    {
        [Test]
        public void Should_add_new_endpoint()
        {
            var hearing = new VideoHearingBuilder().Build();
            var beforeAddCount = hearing.GetEndpoints().Count;
            hearing.AddEndpoint(new Bookings.Domain.Endpoint("DisplayName", "sip@address.com", "1111", null));
            var afterAddCount = hearing.GetEndpoints().Count;
            afterAddCount.Should().BeGreaterThan(beforeAddCount);
        }

        [Test]
        public void should_not_add_existing_endpoint()
        {
            var hearing = new VideoHearingBuilder().Build();
            var ep = new Bookings.Domain.Endpoint("DisplayName", "sip@address.com", "1111", null);
            hearing.AddEndpoint(ep);
            var beforeAddCount = hearing.GetEndpoints().Count;
            Action action = () => hearing.AddEndpoint(ep);

            action.Should().Throw<DomainRuleException>().WithMessage("Endpoint already exists in the hearing");

            var afterAddCount = hearing.GetEndpoints().Count;
            afterAddCount.Should().Be(beforeAddCount);
        }
    }
}


