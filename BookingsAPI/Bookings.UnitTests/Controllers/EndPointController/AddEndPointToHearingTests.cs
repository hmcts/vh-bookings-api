﻿using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
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
    public class AddEndPointToHearingTests : EndPointsControllerTests
    {
        [Test]
        public async Task Should_add_endpoint_to_hearing()
        {
            var updatedHearing = GetVideoHearing(true);
            updatedHearing.AddEndpoint(new Endpoint(Request.DisplayName, "sip", "pin", null));
            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
                .ReturnsAsync(updatedHearing);
            
            var response = await Controller.AddEndPointToHearingAsync(HearingId, Request);

            response.Should().NotBeNull();
            var result = (NoContentResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<AddEndPointToHearingCommand>()), Times.Once);

            EventPublisher.Verify(x => x.PublishAsync(
                    It.Is<EndpointAddedIntegrationEvent>(r =>
                        r.HearingId == HearingId && r.Endpoint.Pin == "pin" && r.Endpoint.Sip == "sip" &&
                        r.Endpoint.DisplayName == Request.DisplayName && r.Endpoint.DefenceAdvocateUsername == null)),
                Times.Once);
        }

        [Test]
        public async Task Should_add_endpoint_wth_defence_advocate_and_send_event()
        {
            var updatedHearing = GetVideoHearing(true);
            var rep = updatedHearing.Participants.First(x => x.HearingRole.UserRole.IsRepresentative);
            Request.DefenceAdvocateUsername = rep.Person.Username;
            
            updatedHearing.AddEndpoint(new Endpoint(Request.DisplayName, "sip", "pin", rep));
            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
                .ReturnsAsync(updatedHearing);

            var response = await Controller.AddEndPointToHearingAsync(HearingId, Request);

            response.Should().NotBeNull();
            var result = (NoContentResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.NoContent);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<AddEndPointToHearingCommand>()), Times.Once);
            EventPublisher.Verify(x => x.PublishAsync(
                    It.Is<EndpointAddedIntegrationEvent>(r =>
                        r.HearingId == HearingId && r.Endpoint.Pin == "pin" && r.Endpoint.Sip == "sip" &&
                        r.Endpoint.DisplayName == Request.DisplayName && r.Endpoint.DefenceAdvocateUsername == Request.DefenceAdvocateUsername)),
                Times.Once);
        }
        
        [Test]
        public async Task Should_return_badrequest_for_given_invalid_hearingid()
        {
            var hearingId = Guid.Empty;
            var result = await Controller.AddEndPointToHearingAsync(hearingId, Request);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
        }

        [Test]
        public async Task Should_return_badrequest_for_given_invalid_request()
        {
            var result = await Controller.AddEndPointToHearingAsync(HearingId, new AddEndpointRequest());

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage("DisplayName", "DisplayName is required");
        }

        [Test]
        public async Task Should_return_notfound_for_given_invalid_hearingId()
        {
            CommandHandlerMock.Setup(c => c.Handle(It.IsAny<AddEndPointToHearingCommand>())).ThrowsAsync(new HearingNotFoundException(Guid.NewGuid()));

            var result = await Controller.AddEndPointToHearingAsync(HearingId, Request);

            result.Should().NotBeNull();
            var objectResult = (NotFoundObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
