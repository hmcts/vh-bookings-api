using Bookings.DAL.Commands;
using Bookings.DAL.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Net;
using Testing.Common.Assertions;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using Bookings.Infrastructure.Services.IntegrationEvents.Events;

namespace Bookings.UnitTests.Controllers.EndPointController
{
    public class UpdateDisplayNameForEndpointTests : EndPointsControllerTests
    {
        [Test]
        public async Task Should_update_endpoint_for_given_hearing_and_endpointid()
        {
            var hearingId = Guid.NewGuid();
            var endpointId = Guid.NewGuid();

            var response = await Controller.UpdateDisplayNameForEndpoint(hearingId, endpointId,
                new UpdateEndpointRequest { 
                    DisplayName = "Test"
                    });

            response.Should().NotBeNull();
            var result = (NoContentResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateEndPointOfHearingCommand>()), Times.Once);
        }

        [Test]
        public async Task Should_return_badrequest_for_given_empty_hearingid()
        {
            var hearingId = Guid.Empty;

            var result = await Controller.UpdateDisplayNameForEndpoint(hearingId, Guid.NewGuid(), new UpdateEndpointRequest
            {
                DisplayName = "Test"
            });

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
        }

        [Test]
        public async Task Should_return_notfound_for_given_invalid_hearingId()
        {
            CommandHandlerMock.Setup(c => c.Handle(It.IsAny<UpdateEndPointOfHearingCommand>())).ThrowsAsync(new HearingNotFoundException(Guid.NewGuid()));

            var hearingId = Guid.NewGuid();
            var endpointId = Guid.NewGuid();

            var result = await Controller.UpdateDisplayNameForEndpoint(hearingId, endpointId, new UpdateEndpointRequest
            {
                DisplayName = "Test"
            });

            result.Should().NotBeNull();
            var objectResult = (NotFoundObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Should_return_notfound_for_given_invalid_endpointId()
        {
            CommandHandlerMock.Setup(c => c.Handle(It.IsAny<UpdateEndPointOfHearingCommand>()))
                                            .ThrowsAsync(new EndPointNotFoundException(Guid.NewGuid()));

            var hearingId = Guid.NewGuid();
            var endpointId = Guid.Empty;

            var result = await Controller.UpdateDisplayNameForEndpoint(hearingId, endpointId, new UpdateEndpointRequest
            {
                DisplayName = "Test"
            });

            result.Should().NotBeNull();
            var objectResult = (NotFoundObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Should_return_badrequest_error_for_empty_display_name()
        {
            var hearingId = Guid.NewGuid();
            var endpointId = Guid.Empty;

            var result = await Controller.UpdateDisplayNameForEndpoint(hearingId, endpointId, new UpdateEndpointRequest());

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Should_update_endpoint_and_send_event()
        {
            var response = await Controller.UpdateDisplayNameForEndpoint(HearingId, EndpointId,
                new UpdateEndpointRequest
                {
                    DisplayName = "Test"
                });

            response.Should().NotBeNull();
            var result = (NoContentResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.NoContent);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateEndPointOfHearingCommand>()), Times.Once);
            EventPublisher.Verify(e => e.PublishAsync(It.IsAny<EndpointUpdatedIntegrationEvent>()), Times.Once);
        }
    }
}
