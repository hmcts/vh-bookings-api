using Bookings.Api.Contract.Responses;
using Bookings.DAL.Queries;
using Bookings.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Testing.Common.Assertions;

namespace Bookings.UnitTests.Controllers.HearingsController
{
    public class GetHearingsByCaseNumberTests : HearingsControllerTest
    {
        [Test]
        public async Task Should_return_bad_request_for_empty_case_number()
        {
            var caseNumber = string.Empty;
            var result = await _controller.GetHearingsByCaseNumber(caseNumber);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(caseNumber), $"Please provide a valid {nameof(caseNumber)}");
        }
        [Test]
        public async Task Should_return_an_empty_list_if_no_records_found_for_the_give_case_number()
        {
            var caseNumber = "TaxCase12345/33";

            _queryHandlerMock
                .Setup(x => x.Handle<GetHearingsByCaseNumberQuery, List<VideoHearing>>(It.IsAny<GetHearingsByCaseNumberQuery>()))
                .ReturnsAsync((List<VideoHearing>)null);

            var result = await _controller.GetHearingsByCaseNumber(caseNumber);

            result.Should().NotBeNull();
            var objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var hearingDetailsResponse = (List<HearingsByCaseNumberResponse>)objectResult.Value;
            hearingDetailsResponse.Should().NotBeNull();

            _queryHandlerMock
                .Verify(x => x.Handle<GetHearingsByCaseNumberQuery, List<VideoHearing>>(It.IsAny<GetHearingsByCaseNumberQuery>()), Times.Once);
        }
        [Test]
        public async Task Should_return_return_a_list_of_hearings_for_a_valid_case_number()
        {
            var caseNumber = "123";
            var hearingsByCaseNumber = new List<VideoHearing>() { GetHearing() };
            _queryHandlerMock
                .Setup(x => x.Handle<GetHearingsByCaseNumberQuery, List<VideoHearing>>(It.IsAny<GetHearingsByCaseNumberQuery>()))
                .ReturnsAsync(hearingsByCaseNumber);
            var result = await _controller.GetHearingsByCaseNumber(caseNumber);
            result.Should().NotBeNull();
            var objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var hearingDetailsResponse = (List<HearingsByCaseNumberResponse>)objectResult.Value;
            hearingDetailsResponse.Should().NotBeNull();
            hearingDetailsResponse[0].CaseNumber.Should().Be("123");
            _queryHandlerMock
                .Verify(x => x.Handle<GetHearingsByCaseNumberQuery, List<VideoHearing>>(It.IsAny<GetHearingsByCaseNumberQuery>()), Times.Once);

        }
    }
}
