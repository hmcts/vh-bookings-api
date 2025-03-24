using System.Collections.Generic;
using BookingsApi.Domain;

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
                new Endpoint(Guid.NewGuid().ToString(),"new endpoint1", Guid.NewGuid().ToString(), "pin"),
                new Endpoint(Guid.NewGuid().ToString(),"new endpoint2", Guid.NewGuid().ToString(), "pin"),
                new Endpoint(Guid.NewGuid().ToString(),"new endpoint2", Guid.NewGuid().ToString(), "pin")
            });
            
            var beforeRemoveCount = hearing.GetEndpoints().Count;
            hearing.RemoveEndpoint(hearing.GetEndpoints()[0]);
            var afterRemoveCount = hearing.GetEndpoints().Count;
            afterRemoveCount.Should().BeLessThan(beforeRemoveCount);
        }
    }
}
