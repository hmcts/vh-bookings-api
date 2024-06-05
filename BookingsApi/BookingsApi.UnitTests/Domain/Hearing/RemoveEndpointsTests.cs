using BookingsApi.Domain;
using System.Collections.Generic;

namespace BookingsApi.UnitTests.Domain.Hearing
{
    public class RemoveEndpointsTests
    {
        [Test]
        public void Should_remove_endpoint_from_hearing()
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.AddEndpoints(new List<Endpoint>
            {
                new ("new endpoint1", Guid.NewGuid().ToString(), "pin"),
                new ("new endpoint2", Guid.NewGuid().ToString(), "pin"),
                new ("new endpoint2", Guid.NewGuid().ToString(), "pin")
            });
            
            var beforeRemoveCount = hearing.GetEndpoints().Count;
            hearing.RemoveEndpoint(hearing.GetEndpoints()[0]);
            var afterRemoveCount = hearing.GetEndpoints().Count;
            afterRemoveCount.Should().BeLessThan(beforeRemoveCount);
        }
    }
}
