using Bookings.Api.Contract.Requests;
using FluentAssertions;
using Bookings.API.Mappings;
using Bookings.UnitTests.Utilities;
using NUnit.Framework;
using Testing.Common.Builders.Api.Request;

namespace Bookings.UnitTests.Mappings
{
    public class UpdateParticipantRequestToNewAddressMapperTest : TestBase
    {
        private UpdateParticipantRequest UpdateParticipantRequest;

        [SetUp]
        public void Setup()
        {
            UpdateParticipantRequest = new UpdateParticipantRequestBuilder().Build();
        }

        [Test]
        public void should_map_address_fields_request_to_address()
        {
            var mapper = new UpdateParticipantRequestToNewAddressMapper();
            var address = mapper.MapRequestToNewAddress(UpdateParticipantRequest);
            address.Should().NotBeNull();
            address.HouseNumber.Should().BeEquivalentTo(UpdateParticipantRequest.HouseNumber);
            address.Street.Should().BeEquivalentTo(UpdateParticipantRequest.Street);
            address.City.Should().BeEquivalentTo(UpdateParticipantRequest.City);
            address.County.Should().BeEquivalentTo(UpdateParticipantRequest.County);
            address.Postcode.Should().BeEquivalentTo(UpdateParticipantRequest.Postcode);
        }
    }
}
