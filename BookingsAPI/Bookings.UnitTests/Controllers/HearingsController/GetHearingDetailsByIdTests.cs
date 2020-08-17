using System;
using System.Net;
using System.Threading.Tasks;
using Bookings.Api.Contract.Responses;
using Bookings.DAL.Queries;
using Bookings.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Testing.Common.Assertions;

namespace Bookings.UnitTests.Controllers.HearingsController
{
    public class GetHearingDetailsByIdTests : HearingsControllerTest
    {
        [Test]
        public async Task Should_return_hearing_details_for_given_hearingid()
        {
            var hearingId = Guid.NewGuid();
             var hearing = GetHearing("123");

            QueryHandlerMock
             .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
             .ReturnsAsync(hearing);

            var result = await Controller.GetHearingDetailsById(hearingId);

            result.Should().NotBeNull();
            var objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var hearingDetailsResponse = (HearingDetailsResponse)objectResult.Value;
            hearingDetailsResponse.Should().NotBeNull();
            hearingDetailsResponse.CaseTypeName.Should().Be("Civil Money Claims");
            hearingDetailsResponse.HearingTypeName.Should().Be("Application to Set Judgment Aside");
            hearingDetailsResponse.Cases.Count.Should().Be(1);
            hearingDetailsResponse.Endpoints.Count.Should().Be(1);
            QueryHandlerMock.Verify(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()), Times.Once);

        }

        [Test]
        public async Task Should_return_badrequest_for_given_invalid_contact_email()
        {
            var hearingId = Guid.Empty;

            var result = await Controller.GetHearingDetailsById(hearingId);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
        }

        [Test]
        public async Task Should_return_notfound_with_no_video_hearing()
        {
            var hearingId = Guid.NewGuid();

            QueryHandlerMock
             .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
             .ReturnsAsync((VideoHearing)null);

            var result = await Controller.GetHearingDetailsById(hearingId);

            result.Should().NotBeNull();
            var objectResult = (NotFoundResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            QueryHandlerMock.Verify(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()), Times.Once);
        }
    }
}
