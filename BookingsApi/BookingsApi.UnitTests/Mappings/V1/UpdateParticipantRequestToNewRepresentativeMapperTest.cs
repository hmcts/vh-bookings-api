using BookingsApi.Contract.V1.Requests;
using BookingsApi.Mappings.V1;
using BookingsApi.UnitTests.Utilities;
using Testing.Common.Builders.Api.V1.Request;

namespace BookingsApi.UnitTests.Mappings.V1
{
    public class UpdateParticipantRequestToNewRepresentativeMapperTest : TestBase
    {
        private UpdateParticipantRequest UpdateParticipantRequest;

        [SetUp]
        public void Setup()
        {
            UpdateParticipantRequest = new UpdateParticipantRequestBuilder().Build();
        }

        [Test]
        public void Should_map_representative_fields_request_to_representative_info()
        {
            var representativeInfo = UpdateParticipantRequestToNewRepresentativeMapper.MapRequestToNewRepresentativeInfo(UpdateParticipantRequest);
            representativeInfo.Should().NotBeNull();
            representativeInfo.Representee.Should().BeEquivalentTo(UpdateParticipantRequest.Representee);
        }
    }
}
