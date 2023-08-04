using System.Collections.Generic;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Helpers;
using Testing.Common.Builders.Api.V1.Request;

namespace BookingsApi.UnitTests.Validation.V1
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
            var invalidParticipantRequest = new ParticipantRequestBuilder("Respondent", "Representative").Build();
            invalidParticipantRequest.Representee = string.Empty;

            var validParticipantRequest = new ParticipantRequestBuilder("Respondent", "Representative").WithRepresentativeDetails("Test Representee").Build();

            var participantRequests = new List<ParticipantRequest>() { validParticipantRequest };
            
            if (withInvalid)
            {
                participantRequests.Add(invalidParticipantRequest);
            }

            return participantRequests;        
        }
    }
}
