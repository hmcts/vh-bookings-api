using BookingsApi.Contract.Requests;
using BookingsApi.Helpers;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using Testing.Common.Builders.Api.Request;

namespace BookingsApi.UnitTests.Validation
{
    public class RepresentativeValidationHelperTests
    {
        [Test]
        public void Should_pass_validation()
        {
            var request = BuildRequest();

            var result = RepresentativeValidationHelper.ValidateRepresentativeInfo(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void Should_fail_validaion_with_empty_representative_representee()
        {
            var request = BuildRequest(true);
            var result = RepresentativeValidationHelper.ValidateRepresentativeInfo(request);

            result.IsValid.Should().BeFalse();
        }

        private List<ParticipantRequest> BuildRequest(bool withInvalid = false)
        {
            var invalidParticipantRequest = new ParticipantRequestBuilder("Defendant", "Representative").Build();
            invalidParticipantRequest.Representee = string.Empty;

            var validParticipantRequest = new ParticipantRequestBuilder("Defendant", "Representative").WithRepresentativeDetails("Test Representee").Build();

            var participantRequests = new List<ParticipantRequest>() { validParticipantRequest };
            
            if (withInvalid)
            {
                participantRequests.Add(invalidParticipantRequest);
            }

            return participantRequests;        
        }
    }
}
