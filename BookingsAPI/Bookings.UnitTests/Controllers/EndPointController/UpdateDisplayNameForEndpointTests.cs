﻿using Bookings.DAL.Commands;
using Bookings.DAL.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
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
        public async Task Should_update_endpoint_for_given_hearing_and_endpoint_id()
        {
            var hearingId = Guid.NewGuid();
            EndpointId = Hearing.Endpoints.First().Id;
            var response = await Controller.UpdateEndpointAsync(hearingId, EndpointId,
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
            EndpointId = Hearing.Endpoints.First().Id;
            var response = await Controller.UpdateEndpointAsync(HearingId, EndpointId,
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
        
        [Test]
        public async Task Should_update_endpoint_with_defence_advocate_and_send_event()
        {
            EndpointId = Hearing.Endpoints.First().Id;
            var rep = Hearing.Participants.First(x => x.HearingRole.UserRole.IsRepresentative);
            var response = await Controller.UpdateEndpointAsync(HearingId, EndpointId,
                new UpdateEndpointRequest
                {
                    DisplayName = "Test",
                    DefenceAdvocateUsername = rep.Person.Username
                });

            response.Should().NotBeNull();
            var result = (NoContentResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.NoContent);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateEndPointOfHearingCommand>()), Times.Once);
            EventPublisher.Verify(e => e.PublishAsync(It.IsAny<EndpointUpdatedIntegrationEvent>()), Times.Once);
        }
    }
}
