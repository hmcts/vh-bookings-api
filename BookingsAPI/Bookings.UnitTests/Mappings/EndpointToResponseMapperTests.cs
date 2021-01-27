using Bookings.Api.Contract.Requests;
using Bookings.API.Mappings;
using Bookings.Common.Services;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;

namespace Bookings.UnitTests.Mappings
{
    
    public class EndpointToResponseMapperTests
    {
        [TestCase]
        public void Should_run()
        {
            var sipAddStream = "TestSipStream";
            var randomGen = new Mock<IRandomGenerator>();
            var endpointRequest = new EndpointRequest { DefenceAdvocateUsername = "TestUserName", DisplayName = "TestDispName" };

            var result = EndpointToResponseMapper.MapRequestToNewEndpointDto(endpointRequest,randomGen.Object, sipAddStream);

            result.Should().NotBeNull();
            result.Sip.EndsWith(sipAddStream).Should().BeTrue();
            result.DisplayName.Should().Be(endpointRequest.DisplayName);
            result.DefenceAdvocateUsername.Should().Be(endpointRequest.DefenceAdvocateUsername);

        }
    }
}
