using System;
using System.Net;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
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
    public class AddEndPointToHearingTests : EndPointsControllerTests
    {
        [Test]
        public async Task Should_add_endpoint_to_hearing()
        {
            var response = await Controller.AddEndPointToHearing(_hearingId, _request);

            response.Should().NotBeNull();
            var result = (NoContentResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<AddEndPointFromHearingCommand>()), Times.Once);
        }

        [Test]
        public async Task Should_return_badrequest_for_given_invalid_hearingid()
        {
            var hearingId = Guid.Empty;
            var result = await Controller.AddEndPointToHearing(Guid.Empty, _request);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
        }

        [Test]
        public async Task Should_return_badrequest_for_given_invalid_request()
        {
            var result = await Controller.AddEndPointToHearing(_hearingId, new AddEndpointRequest());

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage("DisplayName", "DisplayName is required");
        }

        [Test]
        public async Task Should_return_notfound_for_given_invalid_hearingId()
        {
            CommandHandlerMock.Setup(c => c.Handle(It.IsAny<AddEndPointFromHearingCommand>())).ThrowsAsync(new HearingNotFoundException(Guid.NewGuid()));

            var result = await Controller.AddEndPointToHearing(_hearingId, _request);

            result.Should().NotBeNull();
            var objectResult = (NotFoundObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
