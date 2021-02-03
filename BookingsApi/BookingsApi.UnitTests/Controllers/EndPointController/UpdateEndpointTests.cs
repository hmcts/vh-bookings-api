using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net;
using Testing.Common.Assertions;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.Domain;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;

namespace BookingsApi.UnitTests.Controllers.EndPointController
{
    public class UpdateEndpointTests : EndPointsControllerTests
    {
        [Test]
        public async Task Should_return_badrequest_for_given_empty_hearingid()
        {
            var hearingId = Guid.Empty;

            var result = await Controller.UpdateEndpointAsync(hearingId, Guid.NewGuid(), new UpdateEndpointRequest
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

            var result = await Controller.UpdateEndpointAsync(hearingId, endpointId, new UpdateEndpointRequest
            {
                DisplayName = "Test"
            });

            result.Should().NotBeNull();
            var objectResult = (NotFoundObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Should_return_notfound_hearing_not_found()
        {
            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
                .ReturnsAsync((VideoHearing) null);
            var hearingId = Guid.NewGuid();
            var endpointId = Guid.Empty;

            var result = await Controller.UpdateEndpointAsync(hearingId, endpointId, new UpdateEndpointRequest
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

            var result = await Controller.UpdateEndpointAsync(hearingId, endpointId, new UpdateEndpointRequest
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

            var result = await Controller.UpdateEndpointAsync(hearingId, endpointId, new UpdateEndpointRequest());

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Should_update_endpoint_and_send_event()
        {
            var endpoint = Hearing.Endpoints.First();
            EndpointId = endpoint.Id;
            var request = new UpdateEndpointRequest
            {
                DisplayName = "Updated Display Name With Defence Advocate Test",
                DefenceAdvocateUsername = null
            };
            var response = await Controller.UpdateEndpointAsync(HearingId, EndpointId, request);

            response.Should().NotBeNull();
            var result = (NoContentResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.NoContent);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateEndPointOfHearingCommand>()), Times.Once);

            EventPublisher.Verify(
                x => x.PublishAsync(It.Is<EndpointUpdatedIntegrationEvent>(r =>
                    r.HearingId == HearingId && r.Sip == endpoint.Sip && r.DisplayName == request.DisplayName &&
                    r.DefenceAdvocateUsername == request.DefenceAdvocateUsername)), Times.Once);
        }

        [Test]
        public async Task Should_update_endpoint_and_not_send_event()
        { 
            var request = new UpdateEndpointRequest
            {
                DisplayName = "Updated Display Name With Defence Advocate Test",
                DefenceAdvocateUsername = null
            };
            var response = await Controller.UpdateEndpointAsync(HearingId, Guid.NewGuid(), request);

            response.Should().NotBeNull();
            var result = (NoContentResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateEndPointOfHearingCommand>()), Times.Once);

            EventPublisher.Verify(
                x => x.PublishAsync(It.IsAny<EndpointUpdatedIntegrationEvent>()), Times.Never);
        }

        [Test]
        public async Task Should_update_endpoint_with_defence_advocate_and_send_event()
        {
            var endpoint = Hearing.Endpoints.First();
            EndpointId = endpoint.Id;
            var rep = Hearing.Participants.First(x => x.HearingRole.UserRole.IsRepresentative);
            var request = new UpdateEndpointRequest
            {
                DisplayName = "Updated Display Name With Defence Advocate Test",
                DefenceAdvocateUsername = rep.Person.Username
            };
            var response = await Controller.UpdateEndpointAsync(HearingId, EndpointId, request);

            response.Should().NotBeNull();
            var result = (NoContentResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.NoContent);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateEndPointOfHearingCommand>()), Times.Once);

            EventPublisher.Verify(
                x => x.PublishAsync(It.Is<EndpointUpdatedIntegrationEvent>(r =>
                    r.HearingId == HearingId && r.Sip == endpoint.Sip && r.DisplayName == request.DisplayName &&
                    r.DefenceAdvocateUsername == request.DefenceAdvocateUsername)), Times.Once);
        }
    }
}
