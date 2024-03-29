﻿using System.Net;
using BookingsApi.Contract.V1.Enums;
using BookingsApi.Domain;
using BookingsApi.DAL.Queries;
using Microsoft.AspNetCore.Mvc;
using Testing.Common.Assertions;

namespace BookingsApi.UnitTests.Controllers.HearingsController
{
    public class GetBookingStatusByIdTests : HearingsControllerTests
    {
        [Test]
        public async Task Should_return_booking_status_for_given_hearingid()
        {
            var hearingId = Guid.NewGuid();
            var hearing = GetHearing("123");

            QueryHandlerMock
             .Setup(x => x.Handle<GetHearingShellByIdQuery, VideoHearing>(It.IsAny<GetHearingShellByIdQuery>()))
             .ReturnsAsync(hearing);

            var result = await Controller.GetBookingStatusById(hearingId);

            result.Should().NotBeNull();
            var objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            objectResult.Value.Should().BeEquivalentTo(BookingStatus.Booked);
            QueryHandlerMock.Verify(x => x.Handle<GetHearingShellByIdQuery, VideoHearing>(It.IsAny<GetHearingShellByIdQuery>()), Times.Once);
        }

        [Test]
        public async Task Should_return_badrequest_for_given_invalid_hearingid()
        {
            var hearingId = Guid.Empty;

            var result = await Controller.GetBookingStatusById(hearingId);

            result.Should().NotBeNull();
            var objectResult = (ObjectResult)result;
            ((ValidationProblemDetails)objectResult.Value).ContainsKeyAndErrorMessage(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
            QueryHandlerMock.Verify(x => x.Handle<GetHearingShellByIdQuery, VideoHearing>(It.IsAny<GetHearingShellByIdQuery>()), Times.Never);
        }

        [Test]
        public async Task Should_return_notfound_with_no_video_hearing()
        {
            var hearingId = Guid.NewGuid();

            QueryHandlerMock
             .Setup(x => x.Handle<GetHearingShellByIdQuery, VideoHearing>(It.IsAny<GetHearingShellByIdQuery>()))
             .ReturnsAsync((VideoHearing)null);

            var result = await Controller.GetBookingStatusById(hearingId);

            result.Should().NotBeNull();
            var objectResult = (NotFoundResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            QueryHandlerMock.Verify(x => x.Handle<GetHearingShellByIdQuery, VideoHearing>(It.IsAny<GetHearingShellByIdQuery>()), Times.Once);
        }
    }
}
