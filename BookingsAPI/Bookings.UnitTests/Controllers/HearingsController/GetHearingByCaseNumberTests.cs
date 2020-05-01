using Bookings.Api.Contract.Responses;
using Bookings.DAL.Queries;
using Bookings.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Testing.Common.Assertions;

namespace Bookings.UnitTests.Controllers.HearingsController
{
    public class GetHearingByCaseNumberTests : HearingsControllerTest
    {
        [Test]
        public async Task Should_return_bad_request_for_empty_given_casenumber()
        {
            var caseNumber = string.Empty;
            var result = await _controller.GetHearingByCaseNumber(caseNumber);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(caseNumber), $"Please provide a valid {nameof(caseNumber)}");
        }
        [Test]
        public async Task Should_return_empty_hearing_for_given_casenumber_if_no_data_found()
        {
            var caseNumber = "123";
            var hearing = GetHearing();
            _queryHandlerMock
                .Setup(x => x.Handle<GetHearingByCaseNumberQuery, VideoHearing>(It.IsAny<GetHearingByCaseNumberQuery>()))
                .ReturnsAsync(hearing);

            var result = await _controller.GetHearingByCaseNumber(caseNumber);
            result.Should().NotBeNull();
            var objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var hearingDetailsResponse = (HearingByCaseNumberResponse)objectResult.Value;
            hearingDetailsResponse.Should().NotBeNull();
            hearingDetailsResponse.CaseNumber.Should().Be("123");
            _queryHandlerMock
                .Verify(x => x.Handle<GetHearingByCaseNumberQuery, VideoHearing>(It.IsAny<GetHearingByCaseNumberQuery>()), Times.Once);
        }
        [Test]
        public async Task Should_return_hearing_details_for_given_casenumber()
        {
            var caseNumber = "123";
            var hearing = GetHearing();
            hearing.AudioRecordingRequired = false;
            _queryHandlerMock
                .Setup(x => x.Handle<GetHearingByCaseNumberQuery, VideoHearing>(It.IsAny<GetHearingByCaseNumberQuery>()))
                .ReturnsAsync((VideoHearing)null);

            var result = await _controller.GetHearingByCaseNumber(caseNumber);
            result.Should().NotBeNull();
            var objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var hearingDetailsResponse = (HearingByCaseNumberResponse)objectResult.Value;
            hearingDetailsResponse.Should().NotBeNull();
            _queryHandlerMock
                .Verify(x => x.Handle<GetHearingByCaseNumberQuery, VideoHearing>(It.IsAny<GetHearingByCaseNumberQuery>()), Times.Once);
        }
    }
}
