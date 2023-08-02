﻿using BookingsApi.Contract.V1.Requests;
using FluentAssertions;
using BookingsApi.Mappings;
using BookingsApi.Mappings.V1;
using BookingsApi.UnitTests.Utilities;
using NUnit.Framework;
using Testing.Common.Builders.Api.Request;

namespace BookingsApi.UnitTests.Mappings
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
