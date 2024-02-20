using BookingsApi.Domain.Validations;

namespace BookingsApi.UnitTests.Domain.Hearing
{
    public class AddEndpointTests
    {
        [TestCase("")]
        [TestCase("UserName")]
        public void Should_add_new_endpoint(string createdBy)
        {
            var hearing = new VideoHearingBuilder().Build();
            var beforeAddCount = hearing.GetEndpoints().Count;
            var beforeUpdatedDate = hearing.UpdatedDate;
            hearing.AddEndpoint(new BookingsApi.Domain.Endpoint("DisplayName", "sip@address.com", "1111", null), createdBy: createdBy);
            var afterAddCount = hearing.GetEndpoints().Count;
            afterAddCount.Should().BeGreaterThan(beforeAddCount);
            hearing.UpdatedDate.Should().BeAfter(beforeUpdatedDate);
            hearing.UpdatedBy.Should().Be(string.IsNullOrEmpty(createdBy) ? "System" : createdBy);
        }

        [Test]
        public void should_not_add_existing_endpoint()
        {
            var hearing = new VideoHearingBuilder().Build();
            var ep = new BookingsApi.Domain.Endpoint("DisplayName", "sip@address.com", "1111", null);
            hearing.AddEndpoint(ep);
            var beforeAddCount = hearing.GetEndpoints().Count;
            Action action = () => hearing.AddEndpoint(ep);

            action.Should().Throw<DomainRuleException>().WithMessage($"Endpoint {ep.Sip} already exists in the hearing");

            var afterAddCount = hearing.GetEndpoints().Count;
            afterAddCount.Should().Be(beforeAddCount);
        }
    }
}


