using Bookings.Domain;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Testing.Common.Builders.Domain;

namespace Bookings.UnitTests.Domain.Hearing
{
    public class RemoveEndpointsTests
    {
        [Test]
        public void Should_remove_endpoint_from_hearing()
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.AddEndpoints(new List<Endpoint> 
                {   new Endpoint("new endpoint1", Guid.NewGuid().ToString(), "pin"),
                    new Endpoint("new endpoint2", Guid.NewGuid().ToString(), "pin"),
                    new Endpoint("new endpoint2", Guid.NewGuid().ToString(), "pin")});
            
            var beforeRemoveCount = hearing.GetEndpoints().Count;
            hearing.RemoveEndpoint(hearing.GetEndpoints().First());
            var afterRemoveCount = hearing.GetEndpoints().Count;
            afterRemoveCount.Should().BeLessThan(beforeRemoveCount);
        }
    }
}
