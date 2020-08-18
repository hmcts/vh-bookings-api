using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Bookings.DAL.Commands;
using Bookings.DAL.Exceptions;
using Bookings.UnitTests.Controllers.EndPointController;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Testing.Common.Assertions;

namespace Bookings.UnitTests.Controllers.HearingsController
{
    public class RemoveEndPointFromHearingTests : EndPointsControllerTests
    {
        [Test]
        public async Task Should_remove_endpoint_from_hearing_for_given_hearing_and_endpointid()
        {
            var hearingId = Guid.NewGuid();
            var endpointId = Guid.NewGuid();

            var response = await Controller.RemoveEndPointFromHearing(hearingId, endpointId);

            response.Should().NotBeNull();
            var result = (NoContentResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<RemoveEndPointFromHearingCommand>()), Times.Once);
        }

        [Test]
        public async Task Should_return_badrequest_for_given_empty_hearingid()
        {
            var hearingId = Guid.Empty;

            var result = await Controller.RemoveEndPointFromHearing(hearingId, Guid.NewGuid());

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

            var result = await Controller.RemoveEndPointFromHearing(hearingId, endpointId);

            result.Should().NotBeNull();
            var objectResult = (NotFoundObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Should_return_notfound_for_given_invalid_enpointId()
        {
            CommandHandlerMock.Setup(c => c.Handle(It.IsAny<RemoveEndPointFromHearingCommand>())).ThrowsAsync(new EndPointNotFoundException(Guid.NewGuid()));

            var hearingId = Guid.NewGuid();
            var endpointId = Guid.Empty;

            var result = await Controller.RemoveEndPointFromHearing(hearingId, endpointId);

            result.Should().NotBeNull();
            var objectResult = (NotFoundObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
