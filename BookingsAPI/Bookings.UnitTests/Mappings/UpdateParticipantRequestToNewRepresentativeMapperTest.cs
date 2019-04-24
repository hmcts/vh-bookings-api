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
    public class UpdateParticipantRequestToNewRepresentativeMapperTest : TestBase
    {
        private UpdateParticipantRequest UpdateParticipantRequest;

        [SetUp]
        public void Setup()
        {
            UpdateParticipantRequest = new UpdateParticipantRequestBuilder().Build();
        }

        [Test]
        public void should_map_representative_fields_request_to_representative_info()
        {
            var mapper = new UpdateParticipantRequestToNewRepresentativeMapper();
            var representativeInfo = mapper.MapRequestToNewRepresentativeInfo(UpdateParticipantRequest);
            representativeInfo.Should().NotBeNull();
            representativeInfo.Representee.Should().BeEquivalentTo(UpdateParticipantRequest.Representee);
            representativeInfo.SolicitorsReference.Should().BeEquivalentTo(UpdateParticipantRequest.SolicitorsReference);
        }
    }
}
