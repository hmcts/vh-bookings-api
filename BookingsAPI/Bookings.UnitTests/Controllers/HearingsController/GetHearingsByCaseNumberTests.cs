using Bookings.Api.Contract.Responses;
using Bookings.DAL.Queries;
using Bookings.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
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
            var result = await Controller.GetHearingsByCaseNumber(caseNumber);

            result.Should().NotBeNull();
            
            var objectResult = (BadRequestObjectResult)result;
            
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(caseNumber), $"Please provide a valid {nameof(caseNumber)}");
        }
        
        [Test]
        public async Task Should_return_an_empty_list_if_no_records_found_for_the_give_case_number()
        {
            const string caseNumber = "TaxCase12345/33";

            QueryHandlerMock
                .Setup(x => x.Handle<GetHearingsByCaseNumberQuery, List<VideoHearing>>(It.IsAny<GetHearingsByCaseNumberQuery>()))
                .ReturnsAsync((List<VideoHearing>)null);

            var result = await Controller.GetHearingsByCaseNumber(caseNumber);

            result.Should().NotBeNull();
            
            var objectResult = (OkObjectResult)result;
            
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            
            var hearingDetailsResponse = (List<HearingsByCaseNumberResponse>)objectResult.Value;
            
            hearingDetailsResponse.Should().NotBeNull();

            QueryHandlerMock
                .Verify(x => x.Handle<GetHearingsByCaseNumberQuery, List<VideoHearing>>(It.IsAny<GetHearingsByCaseNumberQuery>()), Times.Once);
        }
        
        [TestCase("123")]
        [TestCase("case-123/abc")]
        [TestCase("case-123\abc")]
        [TestCase("123\abc")]
        public async Task Should_return_return_a_list_of_hearings_for_a_valid_case_number(string caseNumber)
        {
            var hearingsByCaseNumber = new List<VideoHearing> { GetHearing(caseNumber) };
            
            QueryHandlerMock
                .Setup(x => x.Handle<GetHearingsByCaseNumberQuery, List<VideoHearing>>(It.IsAny<GetHearingsByCaseNumberQuery>()))
                .ReturnsAsync(hearingsByCaseNumber);
            
            var result = await Controller.GetHearingsByCaseNumber(WebUtility.UrlEncode(caseNumber));
            
            result.Should().NotBeNull();
            
            var objectResult = (OkObjectResult)result;
            
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            
            var hearingDetailsResponse = (List<HearingsByCaseNumberResponse>)objectResult.Value;
            
            hearingDetailsResponse.Should().NotBeNull();
            
            hearingDetailsResponse[0].CaseNumber.Should().Be(caseNumber);
            
            QueryHandlerMock
                .Verify(x => x.Handle<GetHearingsByCaseNumberQuery, List<VideoHearing>>(It.IsAny<GetHearingsByCaseNumberQuery>()), Times.Once);
        }
    }
}
