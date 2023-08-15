using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Controllers;
using BookingsApi.Controllers.V1;
using BookingsApi.Domain;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using Microsoft.AspNetCore.Mvc;

namespace BookingsApi.UnitTests.Controllers
{
    
    public class CheckServiceHealthTests
    {
        private HealthCheckController _controller;
        private Mock<IQueryHandler> _quryHandlerMock;
        [SetUp]
        public void Setup()
        {
            _quryHandlerMock = new Mock<IQueryHandler>();
            _controller = new HealthCheckController(_quryHandlerMock.Object);
        }
        
        [Test]
        public async Task Should_return_server_error_when_service_is_unhealthy()
        {
            _quryHandlerMock
                .Setup(x => x.Handle<GetHearingVenuesQuery, List<HearingVenue>>(It.IsAny<GetHearingVenuesQuery>()))
                .ReturnsAsync(new List<HearingVenue>());

            var result = await _controller.CheckServiceHealth();
            var typedResult = (ObjectResult) result;
            typedResult.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
            var response = (BookingsApiHealthResponse) typedResult.Value;
            response.DatabaseHealth.Successful.Should().BeFalse();
            response.DatabaseHealth.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        }
        
        [Test]
        public async Task Should_return_ok_when_service_is_healthy()
        {
            _quryHandlerMock
                .Setup(x => x.Handle<GetHearingVenuesQuery, List<HearingVenue>>(It.IsAny<GetHearingVenuesQuery>()))
                .ReturnsAsync(new RefDataBuilder().HearingVenues);

            var result = await _controller.CheckServiceHealth();
            var typedResult = (ObjectResult) result;
            typedResult.StatusCode.Should().Be((int) HttpStatusCode.OK);
            var response = (BookingsApiHealthResponse) typedResult.Value;
            response.DatabaseHealth.Successful.Should().BeTrue();
            response.DatabaseHealth.ErrorMessage.Should().BeNullOrWhiteSpace();
            response.DatabaseHealth.Data.Should().BeNull();
        }

        [Test]
        public async Task Should_return_the_application_version_from_assembly()
        {
            var result = await _controller.CheckServiceHealth();
            var typedResult = (ObjectResult)result;
            var response = (BookingsApiHealthResponse)typedResult.Value;
            response.AppVersion.FileVersion.Should().NotBeNullOrEmpty();
            response.AppVersion.InformationVersion.Should().NotBeNullOrEmpty();
        }
    }
}