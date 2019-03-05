using FluentAssertions;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;
using Testing.Common.Builders.Api;

namespace Bookings.IntegrationTests.Api
{
    public class HealthCheckControllerTests : ControllerTestsBase
    {
        private readonly HealthCheckEndpoints _endpoints = new ApiUriFactory().HealthCheckEndpoints;
        
        [Test]
        public async Task should_get_ok_for_user_health_check()
        {
            var uri = _endpoints.HealthCheck;
            var response = await SendGetRequestAsync(uri);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}