using Bookings.Api.Contract.Requests;
using Bookings.API.Helpers;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using Testing.Common.Builders.Api.Request;

namespace Bookings.UnitTests.Validation
{
    public class RepresentativeValidationHelperTests
    {
        [Test]
        public void should_pass_validation()
        {
            var request = BuildRequest();

            var result = RepresentativeValidationHelper.ValidateRepresentativeInfo(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void should_fail_validaion_with_empty_solicitors_refernce_and_representee()
        {
            var request = BuildRequest();
            request[0].Representee = string.Empty;
            request[0].SolicitorsReference = string.Empty;
            var result = RepresentativeValidationHelper.ValidateRepresentativeInfo(request);

            result.IsValid.Should().BeFalse();
        }
        private List<ParticipantRequest> BuildRequest()
        {
            var newParticipant = new ParticipantRequestBuilder("Defendant", "Solicitor").WithSolicitorDetails("Test Reference","Test Representee").Build();

            return new List<ParticipantRequest> { newParticipant };
        
        }
    }
}
