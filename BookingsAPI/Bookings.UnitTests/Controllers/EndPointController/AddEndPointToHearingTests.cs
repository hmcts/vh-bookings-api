using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using Bookings.DAL.Commands;
using Bookings.DAL.Exceptions;
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
            var response = await Controller.AddEndPointToHearingAsync(HearingId, Request);

            response.Should().NotBeNull();
            var result = (NoContentResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<AddEndPointToHearingCommand>()), Times.Once);
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

        [Test]
        public async Task Should_add_endpoint_and_send_event()
        {
            var response = await Controller.AddEndPointToHearingAsync(HearingId, Request);

            response.Should().NotBeNull();
            var result = (NoContentResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.NoContent);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<AddEndPointToHearingCommand>()), Times.Once);
            EventPublisher.Verify(e => e.PublishAsync(It.IsAny<EndpointAddedIntegrationEvent>()), Times.Once);
        }
        
        [Test]
        public async Task Should_add_endpoint_wth_defence_advocate_and_send_event()
        {
            var rep = Hearing.Participants.First(x => x.HearingRole.UserRole.IsRepresentative);
            Request.DefenceAdvocateId = rep.Id;
            var response = await Controller.AddEndPointToHearingAsync(HearingId, Request);

            response.Should().NotBeNull();
            var result = (NoContentResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.NoContent);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<AddEndPointToHearingCommand>()), Times.Once);
            EventPublisher.Verify(e => e.PublishAsync(It.IsAny<EndpointAddedIntegrationEvent>()), Times.Once);
        }
    }
}
