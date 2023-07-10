using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.Domain;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Queries;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Testing.Common.Assertions;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Domain.Validations;
using BookingsApi.Domain.Enumerations;

namespace BookingsApi.UnitTests.Controllers.HearingsController
{
    public class CancelBookingTests : HearingsControllerTests
    {

        [Test]
        public async Task Should_cancel_the_booking()
        {
            var controller = GetControllerObject(true);
            var request = new CancelBookingRequest
            {
                UpdatedBy = "email@hmcts.net",
                CancelReason = "Adjournment"
            };
            var hearingId = Guid.NewGuid();
            var hearing = GetHearing("123");

            QueryHandlerMock
                .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
                .ReturnsAsync(hearing);

            var result = await controller.CancelBooking(hearingId, request);

            result.Should().NotBeNull();
            var objectResult = (NoContentResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

            var message = SbQueueClient.ReadMessageFromQueue();
            var typedMessage = (HearingCancelledIntegrationEvent)message.IntegrationEvent;
            typedMessage.Should().NotBeNull();
            typedMessage.HearingId.Should().Be(hearingId);
        }

        [Test]
        public async Task Should_not_cancel_booking_and_return_badrequest_with_an_invalid_hearingid()
        {
            var request = new CancelBookingRequest();
            var hearingId = Guid.Empty;

            var result = await Controller.CancelBooking(hearingId, request);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(hearingId),
                $"Please provide a valid {nameof(hearingId)}");
        }

        [Test]
        public async Task Should_not_cancel_booking_and_throw_domainrule_Exception_return_conflict()
        {
            var controller = GetControllerObject(true);
            var request = new CancelBookingRequest
            {
                UpdatedBy = "email@hmcts.net",
                CancelReason = "Adjournment"
            };
            var hearingId = Guid.NewGuid();
            var hearing = GetHearing("123");
            hearing.UpdateStatus(BookingStatus.Failed, "test", "test");

            QueryHandlerMock
                .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
                .ReturnsAsync(hearing);

            CommandHandlerMock.Setup(x => x.Handle(It.IsAny<UpdateHearingStatusCommand>())).Callback(() => 
                hearing.UpdateStatus(BookingStatus.Cancelled, "email@hmcts.net", "Adjournment"));
            var result = await controller.CancelBooking(hearingId, request);

            result.Should().NotBeNull();
            var objectResult = (ConflictObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.Conflict);
        }
    }
}
