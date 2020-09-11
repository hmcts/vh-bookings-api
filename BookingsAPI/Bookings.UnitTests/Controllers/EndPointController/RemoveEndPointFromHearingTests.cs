using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bookings.DAL.Commands;
using Bookings.DAL.Exceptions;
using Bookings.DAL.Queries;
using Bookings.Domain;
using Bookings.Infrastructure.Services.IntegrationEvents.Events;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Testing.Common.Assertions;

namespace Bookings.UnitTests.Controllers.EndPointController
{
    public class RemoveEndPointFromHearingTests : EndPointsControllerTests
    {
        [Test]
        public async Task Should_remove_endpoint_from_hearing_for_given_hearing_and_endpoint_id()
        {
            var hearingId = Guid.NewGuid();
            EndpointId = Hearing.Endpoints.First().Id;
            var response = await Controller.RemoveEndPointFromHearingAsync(hearingId, EndpointId);

            response.Should().NotBeNull();
            var result = (NoContentResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<RemoveEndPointFromHearingCommand>()), Times.Once);
        }

        [Test]
        public async Task Should_return_badrequest_for_given_empty_hearingid()
        {
            var hearingId = Guid.Empty;

            var result = await Controller.RemoveEndPointFromHearingAsync(hearingId, Guid.NewGuid());

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
        }

        [Test]
        public async Task Should_return_notfound_for_given_invalid_hearingId()
        {
            CommandHandlerMock.Setup(c => c.Handle(It.IsAny<RemoveEndPointFromHearingCommand>())).ThrowsAsync(new HearingNotFoundException(Guid.NewGuid()));

            var hearingId = Guid.NewGuid();
            var endpointId = Guid.NewGuid();

            var result = await Controller.RemoveEndPointFromHearingAsync(hearingId, endpointId);

            result.Should().NotBeNull();
            var objectResult = (NotFoundObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Should_return_notfound_for_given_invalid_endpoint_id()
        {
            var endpointId = Guid.NewGuid();
            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
                .ThrowsAsync(new EndPointNotFoundException(endpointId));

            var hearingId = Guid.NewGuid();

            var result = await Controller.RemoveEndPointFromHearingAsync(hearingId, endpointId);

            result.Should().NotBeNull();
            var objectResult = (NotFoundObjectResult) result;
            objectResult.StatusCode.Should().Be((int) HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Should_return_notfound_for_given_invalid_enpointId()
        {
            CommandHandlerMock.Setup(c => c.Handle(It.IsAny<RemoveEndPointFromHearingCommand>())).ThrowsAsync(new EndPointNotFoundException(Guid.NewGuid()));

            var hearingId = Guid.NewGuid();
            var endpointId = Guid.Empty;

            var result = await Controller.RemoveEndPointFromHearingAsync(hearingId, endpointId);

            result.Should().NotBeNull();
            var objectResult = (NotFoundObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Should_remove_endpoint_and_send_event()
        {
            EndpointId = Hearing.Endpoints.First().Id;
            var response = await Controller.RemoveEndPointFromHearingAsync(HearingId, EndpointId);

            response.Should().NotBeNull();
            var result = (NoContentResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.NoContent);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<RemoveEndPointFromHearingCommand>()), Times.Once);
            EventPublisher.Verify(e => e.PublishAsync(It.IsAny<EndpointRemovedIntegrationEvent>()), Times.Once);
        }
    }
}
