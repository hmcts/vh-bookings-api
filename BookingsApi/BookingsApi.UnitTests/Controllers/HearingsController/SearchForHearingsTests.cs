using BookingsApi.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Queries;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.DAL.Queries;

namespace BookingsApi.UnitTests.Controllers.HearingsController
{
    public class SearchForHearingsTests : HearingsControllerTests
    {
        [Test]
        public async Task Should_return_an_empty_list_if_no_records_found_for_the_give_case_number()
        {
            const string caseNumber = "TaxCase12345/33";

            var query = new SearchForHearingsQuery
            {
                CaseNumber = caseNumber
            };
            QueryHandlerMock
                .Setup(x => x.Handle<GetHearingsBySearchQuery, List<VideoHearing>>(It.IsAny<GetHearingsBySearchQuery>()))
                .ReturnsAsync((List<VideoHearing>)null);

            var result = await Controller.SearchForHearingsAsync(query);

            result.Should().NotBeNull();

            var objectResult = (OkObjectResult)result;

            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var hearingDetailsResponse = (List<AudioRecordedHearingsBySearchResponse>)objectResult.Value;

            hearingDetailsResponse.Should().NotBeNull();

            QueryHandlerMock
                .Verify(x => x.Handle<GetHearingsBySearchQuery, List<VideoHearing>>(It.IsAny<GetHearingsBySearchQuery>()), Times.Once);
        }

        [TestCase("123")]
        [TestCase("case-123/abc")]
        [TestCase("case-123\abc")]
        [TestCase("123\abc")]
        public async Task Should_return_return_a_list_of_hearings_for_a_valid_case_number(string caseNumber)
        {
            var query = new SearchForHearingsQuery
            {
                CaseNumber = caseNumber
            };
            var hearingsByCaseNumber = new List<VideoHearing> { GetHearing(caseNumber) };

            QueryHandlerMock
                .Setup(x => x.Handle<GetHearingsBySearchQuery, List<VideoHearing>>(It.IsAny<GetHearingsBySearchQuery>()))
                .ReturnsAsync(hearingsByCaseNumber);

            var result = await Controller.SearchForHearingsAsync(query);

            result.Should().NotBeNull();

            var objectResult = (OkObjectResult)result;

            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var hearingDetailsResponse = (List<AudioRecordedHearingsBySearchResponse>)objectResult.Value;

            hearingDetailsResponse.Should().NotBeNull();

            hearingDetailsResponse[0].CaseNumber.Should().Be(caseNumber);

            QueryHandlerMock
                .Verify(x => x.Handle<GetHearingsBySearchQuery, List<VideoHearing>>(It.IsAny<GetHearingsBySearchQuery>()), Times.Once);
        }
    }
}
