using BookingsApi.Domain;
using System.Collections.Generic;

namespace BookingsApi.UnitTests.Domain.Hearing
{
    public class RemoveEndpointsTests : HearingTests
    {
        [TestCase("")]
        [TestCase("UserName")]
        public void Should_remove_endpoint_from_hearing(string removedBy)
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.AddEndpoints(new List<Endpoint>
            {
                new Endpoint("new endpoint1", Guid.NewGuid().ToString(), "pin", null),
                new Endpoint("new endpoint2", Guid.NewGuid().ToString(), "pin", null),
                new Endpoint("new endpoint2", Guid.NewGuid().ToString(), "pin", null)
            });
            
            var beforeRemoveCount = hearing.GetEndpoints().Count;
            var beforeUpdatedDate = hearing.UpdatedDate;
            hearing.RemoveEndpoint(hearing.GetEndpoints().First(), removedBy: removedBy);
            var afterRemoveCount = hearing.GetEndpoints().Count;
            afterRemoveCount.Should().BeLessThan(beforeRemoveCount);
            AssertHearingUpdatedAuditDetailsAreUpdated(hearing, beforeUpdatedDate, removedBy);
        }
    }
}
