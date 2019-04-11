using System;
using System.Collections.Generic;
using System.Linq;
using Bookings.Api.Contract.Requests;
using FluentAssertions;
using Bookings.Common;
using Bookings.Api.Contract.Responses;
using Bookings.API.Mappings;
using Bookings.Domain;
using Bookings.Domain.RefData;
using Bookings.UnitTests.Utilities;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
            UpdateParticipantRequest = new UpdateParticipantRequestBuilder("Claimant LIP").Build();
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
